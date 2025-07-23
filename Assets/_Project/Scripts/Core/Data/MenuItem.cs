// MenuItem.cs
using UnityEngine;
using Mirror;
using System.Collections.Generic;

// MenuItem.cs
using UnityEngine;
using Mirror;
using System.Collections.Generic;

[System.Serializable]
public class MenuItem
{
    public int id;
    public string name;
    public string description;
    public float price;
    public Sprite icon;
    public MenuCategory category;
    public float preparationTime;
}

public enum MenuCategory
{
    Coffee,
    Tea,
    Pastries,
    Sandwiches,
    Desserts
}

// MenuManager.cs
public class MenuManager : NetworkBehaviour
{
    [Header("Menu Data")]
    public MenuItem[] menuItems;

    private Dictionary<int, MenuItem> menuDict;

    void Awake()
    {
        InitializeMenu();
    }

    void InitializeMenu()
    {
        menuDict = new Dictionary<int, MenuItem>();

        // Sample menu items
        menuItems = new MenuItem[]
        {
            new MenuItem { id = 1, name = "Espresso", price = 2.50f, category = MenuCategory.Coffee, preparationTime = 30f },
            new MenuItem { id = 2, name = "Cappuccino", price = 3.75f, category = MenuCategory.Coffee, preparationTime = 45f },
            new MenuItem { id = 3, name = "Croissant", price = 2.25f, category = MenuCategory.Pastries, preparationTime = 15f },
            new MenuItem { id = 4, name = "Blueberry Muffin", price = 3.00f, category = MenuCategory.Pastries, preparationTime = 10f }
        };

        foreach(MenuItem item in menuItems)
        {
            menuDict[item.id] = item;
        }
    }

    public MenuItem GetMenuItem(int id)
    {
        return menuDict.ContainsKey(id) ? menuDict[id] : null;
    }

    public MenuItem[] GetMenuByCategory(MenuCategory category)
    {
        return System.Array.FindAll(menuItems, item => item.category == category);
    }
}
