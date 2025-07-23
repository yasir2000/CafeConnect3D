// OrderManager.cs
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class OrderManager : NetworkBehaviour
{
    [Header("Order Management")]
    public Transform kitchenArea;
    public float orderPreparationTime = 30f;

    [Header("Order Display")]
    public GameObject orderTicketPrefab;
    public Transform orderBoard;

    private Dictionary<uint, Order> activeOrders = new Dictionary<uint, Order>();
    private Dictionary<uint, float> orderTimers = new Dictionary<uint, float>();

    public static OrderManager Instance { get; private set; }

    public event System.Action<Order> OnOrderReceived;
    public event System.Action<uint> OnOrderCompleted;
    public event System.Action<uint> OnOrderFailed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isServer) return;

        UpdateOrderTimers();
    }

    [Server]
    public void ReceiveOrder(Order order, NetworkPlayer orderTaker)
    {
        uint orderId = order.customerId;

        activeOrders[orderId] = order;
        orderTimers[orderId] = 0f;

        RpcDisplayNewOrder(orderId, order.SerializeOrder());
        OnOrderReceived?.Invoke(order);
    }

    [Server]
    void UpdateOrderTimers()
    {
        List<uint> ordersToRemove = new List<uint>();

        foreach (var kvp in orderTimers.ToList())
        {
            uint orderId = kvp.Key;
            float timer = kvp.Value;

            orderTimers[orderId] = timer + Time.deltaTime;

            // Check if order has taken too long
            if (timer > orderPreparationTime)
            {
                FailOrder(orderId);
                ordersToRemove.Add(orderId);
            }
        }

        foreach (uint orderId in ordersToRemove)
        {
            activeOrders.Remove(orderId);
            orderTimers.Remove(orderId);
        }
    }

    [Server]
    public void StartPreparingOrder(uint orderId, NetworkPlayer chef)
    {
        if (activeOrders.ContainsKey(orderId))
        {
            RpcOrderInPreparation(orderId, chef.playerName);
        }
    }

    [Server]
    public void CompleteOrder(uint orderId, NetworkPlayer chef)
    {
        if (activeOrders.ContainsKey(orderId))
        {
            Order completedOrder = activeOrders[orderId];

            // Find the customer and deliver food
            Customer customer = FindCustomerById(orderId);
            if (customer != null)
            {
                customer.ReceiveFood();
            }

            activeOrders.Remove(orderId);
            orderTimers.Remove(orderId);

            RpcOrderCompleted(orderId);
            OnOrderCompleted?.Invoke(orderId);
        }
    }

    [Server]
    void FailOrder(uint orderId)
    {
        if (activeOrders.ContainsKey(orderId))
        {
            Customer customer = FindCustomerById(orderId);
            if (customer != null)
            {
                // Customer leaves angry due to slow service
                customer.patience = 0f;
            }

            RpcOrderFailed(orderId);
            OnOrderFailed?.Invoke(orderId);
        }
    }

    [Server]
    Customer FindCustomerById(uint customerId)
    {
        Customer[] customers = FindObjectsOfType<Customer>();
        foreach (Customer customer in customers)
        {
            if (customer.netId == customerId)
            {
                return customer;
            }
        }
        return null;
    }

    [Server]
    public List<Order> GetActiveOrders()
    {
        return activeOrders.Values.ToList();
    }

    [Server]
    public Order GetOrder(uint orderId)
    {
        return activeOrders.ContainsKey(orderId) ? activeOrders[orderId] : null;
    }

    [Server]
    public float GetOrderProgress(uint orderId)
    {
        if (orderTimers.ContainsKey(orderId))
        {
            return orderTimers[orderId] / orderPreparationTime;
        }
        return 0f;
    }

    // Client RPCs for UI updates
    [ClientRpc]
    void RpcDisplayNewOrder(uint orderId, string serializedOrder)
    {
        Order order = Order.DeserializeOrder(serializedOrder);
        UIManager.Instance?.DisplayNewOrder(orderId, order);
        AudioManager.Instance?.PlayNewOrderSound();
    }

    [ClientRpc]
    void RpcOrderInPreparation(uint orderId, string chefName)
    {
        UIManager.Instance?.UpdateOrderStatus(orderId, $"Being prepared by {chefName}");
    }

    [ClientRpc]
    void RpcOrderCompleted(uint orderId)
    {
        UIManager.Instance?.RemoveOrderFromBoard(orderId);
        AudioManager.Instance?.PlayOrderCompleteSound();
    }

    [ClientRpc]
    void RpcOrderFailed(uint orderId)
    {
        UIManager.Instance?.RemoveOrderFromBoard(orderId);
        UIManager.Instance?.ShowOrderFailedMessage(orderId);
        AudioManager.Instance?.PlayOrderFailedSound();
    }
}
