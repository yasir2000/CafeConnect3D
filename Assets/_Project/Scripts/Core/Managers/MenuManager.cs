// MenuManager.cs
using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Configuration")]
    public List<MenuItem> availableMenuItems = new List<MenuItem>();

    [Header("Menu Categories")]
    public List<MenuCategory> categories = new List<MenuCategory>();

    public static MenuManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDefaultMenu();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDefaultMenu()
    {
        if (availableMenuItems.Count == 0)
        {
            CreateDefaultMenuItems();
        }
    }

    void CreateDefaultMenuItems()
    {
        // Coffee drinks
        availableMenuItems.Add(new MenuItem(1, "Espresso", "Strong coffee shot", 2.50f, MenuCategory.Coffee, 3f));
        availableMenuItems.Add(new MenuItem(2, "Cappuccino", "Espresso with steamed milk", 4.00f, MenuCategory.Coffee, 5f));
        availableMenuItems.Add(new MenuItem(3, "Latte", "Espresso with lots of steamed milk", 4.50f, MenuCategory.Coffee, 5f));
        availableMenuItems.Add(new MenuItem(4, "Americano", "Espresso with hot water", 3.00f, MenuCategory.Coffee, 4f));
        availableMenuItems.Add(new MenuItem(5, "Mocha", "Chocolate coffee", 5.00f, MenuCategory.Coffee, 6f));

        // Tea
        availableMenuItems.Add(new MenuItem(6, "Earl Grey Tea", "Classic black tea", 2.00f, MenuCategory.Tea, 3f));
        availableMenuItems.Add(new MenuItem(7, "Green Tea", "Healthy green tea", 2.00f, MenuCategory.Tea, 3f));
        availableMenuItems.Add(new MenuItem(8, "Chai Latte", "Spiced tea with milk", 3.50f, MenuCategory.Tea, 4f));

        // Pastries
        availableMenuItems.Add(new MenuItem(9, "Croissant", "Buttery pastry", 3.00f, MenuCategory.Pastry, 2f));
        availableMenuItems.Add(new MenuItem(10, "Muffin", "Blueberry muffin", 2.50f, MenuCategory.Pastry, 1f));
        availableMenuItems.Add(new MenuItem(11, "Danish", "Sweet pastry", 3.50f, MenuCategory.Pastry, 2f));

        // Sandwiches
        availableMenuItems.Add(new MenuItem(12, "Club Sandwich", "Classic club sandwich", 8.00f, MenuCategory.Food, 8f));
        availableMenuItems.Add(new MenuItem(13, "Bagel", "Everything bagel with cream cheese", 4.00f, MenuCategory.Food, 3f));
        availableMenuItems.Add(new MenuItem(14, "Panini", "Grilled sandwich", 7.00f, MenuCategory.Food, 7f));

        // Cold drinks
        availableMenuItems.Add(new MenuItem(15, "Iced Coffee", "Cold brew coffee", 3.50f, MenuCategory.Cold, 2f));
        availableMenuItems.Add(new MenuItem(16, "Frappuccino", "Blended coffee drink", 5.50f, MenuCategory.Cold, 6f));
        availableMenuItems.Add(new MenuItem(17, "Iced Tea", "Cold tea", 2.50f, MenuCategory.Cold, 2f));
        availableMenuItems.Add(new MenuItem(18, "Smoothie", "Fruit smoothie", 4.50f, MenuCategory.Cold, 5f));
    }

    public List<MenuItem> GetAvailableMenuItems()
    {
        return availableMenuItems;
    }

    public List<MenuItem> GetMenuItemsByCategory(MenuCategory category)
    {
        return availableMenuItems.FindAll(item => item.category == category);
    }

    public MenuItem GetMenuItemById(int id)
    {
        return availableMenuItems.Find(item => item.id == id);
    }

    public List<MenuItem> GetRandomMenuItems(int count)
    {
        List<MenuItem> randomItems = new List<MenuItem>();
        List<MenuItem> tempList = new List<MenuItem>(availableMenuItems);

        for (int i = 0; i < count && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            randomItems.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return randomItems;
    }

    public void AddMenuItem(MenuItem newItem)
    {
        if (!availableMenuItems.Exists(item => item.id == newItem.id))
        {
            availableMenuItems.Add(newItem);
        }
    }

    public void RemoveMenuItem(int itemId)
    {
        availableMenuItems.RemoveAll(item => item.id == itemId);
    }

    public float GetAveragePrice()
    {
        if (availableMenuItems.Count == 0) return 0f;

        float total = 0f;
        foreach (MenuItem item in availableMenuItems)
        {
            total += item.price;
        }
        return total / availableMenuItems.Count;
    }
}
