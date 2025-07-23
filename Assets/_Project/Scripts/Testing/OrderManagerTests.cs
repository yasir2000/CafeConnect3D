// OrderManagerTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class OrderManagerTests
{
    private OrderManager orderManager;
    private MenuManager menuManager;

    [SetUp]
    public void Setup()
    {
        GameObject testObject = new GameObject();
        orderManager = testObject.AddComponent<OrderManager>();
        menuManager = testObject.AddComponent<MenuManager>();
    }

    [Test]
    public void CreateOrder_ValidItems_ReturnsOrder()
    {
        // Arrange
        OrderItem[] items = new OrderItem[]
        {
            new OrderItem { menuItemId = 1, quantity = 2 },
            new OrderItem { menuItemId = 2, quantity = 1 }
        };

        // Act
        var order = orderManager.CreateOrder(12345, items);

        // Assert
        Assert.IsNotNull(order);
        Assert.AreEqual(12345, order.playerId);
        Assert.AreEqual(2, order.items.Count);
    }

    [Test]
    public void CalculateOrderTotal_MultipleItems_ReturnsCorrectTotal()
    {
        // Test order calculation logic
        // Implementation here...
    }
}
