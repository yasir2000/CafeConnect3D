// Order.cs
using Mirror;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Order
{
    public int orderId;
    public uint playerId;
    public uint customerId;  // Add customerId for compatibility
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

    // Add missing methods
    public void AddItem(OrderItem item)
    {
        items.Add(item);
        RecalculateTotal();
    }

    public float GetTotalPrice()
    {
        return totalPrice;
    }

    public string SerializeOrder()
    {
        return JsonUtility.ToJson(this);
    }

    public static Order DeserializeOrder(string json)
    {
        return JsonUtility.FromJson<Order>(json);
    }

    private void RecalculateTotal()
    {
        totalPrice = 0f;
        foreach(var item in items)
        {
            totalPrice += item.GetItemTotal();
        }
    }
}

[System.Serializable]
public class OrderItem
{
    public int menuItemId;
    public string itemName;      // Add itemName property
    public int quantity;
    public float pricePerItem;   // Add price tracking
    public List<string> customizations;

    public OrderItem()
    {
        customizations = new List<string>();
    }

    public OrderItem(int menuItemId, string itemName, int quantity, float pricePerItem)
    {
        this.menuItemId = menuItemId;
        this.itemName = itemName;
        this.quantity = quantity;
        this.pricePerItem = pricePerItem;
        this.customizations = new List<string>();
    }

    public float GetItemTotal()
    {
        return pricePerItem * quantity;
    }
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
