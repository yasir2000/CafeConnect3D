// Order.cs
using Mirror;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order
{
    public uint customerId;
    public List<OrderItem> items;
    public float totalPrice;
    public OrderStatus status;
    public float orderTime;
    public string specialInstructions;

    public Order()
    {
        items = new List<OrderItem>();
        status = OrderStatus.Pending;
        orderTime = Time.time;
        specialInstructions = "";
        CalculateTotalPrice();
    }

    public void AddItem(MenuItem menuItem, int quantity)
    {
        OrderItem existingItem = items.Find(item => item.menuItemId == menuItem.id);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add(new OrderItem(menuItem.id, quantity, menuItem.name, menuItem.price));
        }
        CalculateTotalPrice();
    }

    public void RemoveItem(int menuItemId, int quantity = 1)
    {
        OrderItem item = items.Find(x => x.menuItemId == menuItemId);
        if (item != null)
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
        }
        CalculateTotalPrice();
    }

    public void CalculateTotalPrice()
    {
        totalPrice = 0f;
        foreach (OrderItem item in items)
        {
            totalPrice += item.price * item.quantity;
        }
    }

    public float GetTotalPrice()
    {
        return totalPrice;
    }

    public int GetTotalItems()
    {
        int total = 0;
        foreach (OrderItem item in items)
        {
            total += item.quantity;
        }
        return total;
    }

    public string SerializeOrder()
    {
        return JsonUtility.ToJson(this);
    }

    public static Order DeserializeOrder(string json)
    {
        return JsonUtility.FromJson<Order>(json);
    }

    public string GetOrderSummary()
    {
        string summary = $"Order for Customer #{customerId}\n";
        foreach (OrderItem item in items)
        {
            summary += $"- {item.itemName} x{item.quantity} (${item.price:F2} each)\n";
        }
        summary += $"Total: ${totalPrice:F2}";
        return summary;
    }
}

[System.Serializable]
public class OrderItem
{
    public int menuItemId;
    public string itemName;
    public int quantity;
    public float price;
    public List<string> customizations;

    public OrderItem(int id, int qty, string name, float itemPrice)
    {
        menuItemId = id;
        quantity = qty;
        itemName = name;
        price = itemPrice;
        customizations = new List<string>();
    }

    public void AddCustomization(string customization)
    {
        if (!customizations.Contains(customization))
        {
            customizations.Add(customization);
        }
    }

    public void RemoveCustomization(string customization)
    {
        customizations.Remove(customization);
    }

    public float GetItemTotal()
    {
        return price * quantity;
    }
}

public enum OrderStatus
{
    Pending,
    InProgress,
    Ready,
    Delivered,
    Cancelled
}
