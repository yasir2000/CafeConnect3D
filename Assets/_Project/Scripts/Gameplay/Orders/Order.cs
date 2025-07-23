// Order.cs
using Mirror;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Order
{
    public int orderId;
    public uint playerId;
    public List<OrderItem> items;
    public float totalPrice;
    public OrderStatus status;
    public float orderTime;
    public float estimatedCompletionTime;

    public Order()
    {
        items = new List<OrderItem>();
        status = OrderStatus.Pending;
        orderTime = Time.time;
    }
}

[System.Serializable]
public class OrderItem
{
    public int menuItemId;
    public int quantity;
    public List<string> customizations;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    InProgress,
    Ready,
    Completed,
    Cancelled
}

// OrderManager.cs


public class OrderManager : NetworkBehaviour
{
    [Header("Order Management")]
    public List<Order> activeOrders;
    public List<Order> completedOrders;

    private int nextOrderId = 1000;
    private MenuManager menuManager;

    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();
        activeOrders = new List<Order>();
        completedOrders = new List<Order>();
    }

    [Command]
    public void CmdSubmitOrder(uint playerId, OrderItem[] orderItems)
    {
        Order newOrder = CreateOrder(playerId, orderItems);
        activeOrders.Add(newOrder);

        // Calculate total price and preparation time
        CalculateOrderDetails(newOrder);

        // Notify all clients about the new order
        RpcOrderSubmitted(newOrder);

        // Start order processing
        StartCoroutine(ProcessOrder(newOrder));
    }

    Order CreateOrder(uint playerId, OrderItem[] orderItems)
    {
        Order order = new Order
        {
            orderId = nextOrderId++,
            playerId = playerId,
            items = new List<OrderItem>(orderItems)
        };

        return order;
    }

    void CalculateOrderDetails(Order order)
    {
        float totalPrice = 0f;
        float totalPrepTime = 0f;

        foreach(OrderItem item in order.items)
        {
            MenuItem menuItem = menuManager.GetMenuItem(item.menuItemId);
            if(menuItem != null)
            {
                totalPrice += menuItem.price * item.quantity;
                totalPrepTime += menuItem.preparationTime * item.quantity;
            }
        }

        order.totalPrice = totalPrice;
        order.estimatedCompletionTime = Time.time + totalPrepTime;
    }

    [ClientRpc]
    void RpcOrderSubmitted(Order order)
    {
        // Update UI for all players
        FindObjectOfType<OrderDisplayUI>().AddOrder(order);

        // Show notification to the player who ordered
        if(NetworkClient.connection.identity.netId == order.playerId)
        {
            ShowOrderConfirmation(order);
        }
    }

    System.Collections.IEnumerator ProcessOrder(Order order)
    {
        // Update order status
        order.status = OrderStatus.Confirmed;
        RpcUpdateOrderStatus(order.orderId, OrderStatus.Confirmed);

        yield return new WaitForSeconds(5f); // Confirmation delay

        // Start preparation
        order.status = OrderStatus.InProgress;
        RpcUpdateOrderStatus(order.orderId, OrderStatus.InProgress);

        // Wait for preparation time
        float remainingTime = order.estimatedCompletionTime - Time.time;
        yield return new WaitForSeconds(remainingTime);

        // Order ready
        order.status = OrderStatus.Ready;
        RpcUpdateOrderStatus(order.orderId, OrderStatus.Ready);
        RpcNotifyOrderReady(order.orderId, order.playerId);
    }

    [ClientRpc]
    void RpcUpdateOrderStatus(int orderId, OrderStatus status)
    {
        Order order = activeOrders.Find(o => o.orderId == orderId);
        if(order != null)
        {
            order.status = status;
            FindObjectOfType<OrderDisplayUI>().UpdateOrderStatus(orderId, status);
        }
    }

    [ClientRpc]
    void RpcNotifyOrderReady(int orderId, uint playerId)
    {
        if(NetworkClient.connection.identity.netId == playerId)
        {
            // Show "Your order is ready!" notification
            FindObjectOfType<NotificationUI>().ShowNotification($"Order #{orderId} is ready for pickup!");
        }
    }
}
