// MenuItem.cs
using UnityEngine;
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

    // Default constructor
    public MenuItem() { }

    // Constructor for MenuManager initialization
    public MenuItem(int id, string name, string description, float price, MenuCategory category, float preparationTime)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.price = price;
        this.category = category;
        this.preparationTime = preparationTime;
    }
}

public enum MenuCategory
{
    Coffee,
    Tea,
    Pastries,
    Pastry,      // Add alias for compatibility
    Sandwiches,
    Desserts,
    Food,        // Add Food category
    Cold         // Add Cold category
}
