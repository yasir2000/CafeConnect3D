// MenuUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Mirror;

public class MenuUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject menuPanel;
    public Transform menuItemsParent;
    public GameObject menuItemPrefab;
    public Button orderButton;
    public Text totalPriceText;
    public Text orderSummaryText;

    private List<MenuItemUI> menuItemUIs;
    private List<OrderItem> currentOrder;
    private float currentTotal;
    private PlayerController currentPlayer;
    private OrderCounter currentCounter;

    void Start()
    {
        menuItemUIs = new List<MenuItemUI>();
        currentOrder = new List<OrderItem>();

        orderButton.onClick.AddListener(SubmitOrder);
        menuPanel.SetActive(false);
    }

    public void ShowMenu(PlayerController player, OrderCounter counter)
    {
        currentPlayer = player;
        currentCounter = counter;
        currentOrder.Clear();
        currentTotal = 0f;

        CreateMenuItems();
        UpdateOrderSummary();

        menuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CreateMenuItems()
    {
        // Clear existing items
        foreach(Transform child in menuItemsParent)
        {
            Destroy(child.gameObject);
        }
        menuItemUIs.Clear();

        MenuManager menuManager = FindObjectOfType<MenuManager>();
        MenuItem[] allItems = menuManager.menuItems;

        foreach(MenuItem item in allItems)
        {
            GameObject itemGO = Instantiate(menuItemPrefab, menuItemsParent);
            MenuItemUI itemUI = itemGO.GetComponent<MenuItemUI>();
            itemUI.Initialize(item, this);
            menuItemUIs.Add(itemUI);
        }
    }

    public void AddItemToOrder(MenuItem item)
    {
        // Find existing item in order
        OrderItem existingItem = currentOrder.Find(oi => oi.menuItemId == item.id);

        if(existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            currentOrder.Add(new OrderItem
            {
                menuItemId = item.id,
                quantity = 1,
                customizations = new List<string>()
            });
        }

        currentTotal += item.price;
        UpdateOrderSummary();
    }

    public void RemoveItemFromOrder(MenuItem item)
    {
        OrderItem existingItem = currentOrder.Find(oi => oi.menuItemId == item.id);

        if(existingItem != null)
        {
            existingItem.quantity--;
            currentTotal -= item.price;

            if(existingItem.quantity <= 0)
            {
                currentOrder.Remove(existingItem);
            }

            UpdateOrderSummary();
        }
    }

    void UpdateOrderSummary()
    {
        totalPriceText.text = $"Total: ${currentTotal:F2}";

        string summary = "Order Summary:\n";
        MenuManager menuManager = FindObjectOfType<MenuManager>();

        foreach(OrderItem orderItem in currentOrder)
        {
            MenuItem menuItem = menuManager.GetMenuItem(orderItem.menuItemId);
            if(menuItem != null)
            {
                summary += $"{orderItem.quantity}x {menuItem.name} - ${menuItem.price * orderItem.quantity:F2}\n";
            }
        }

        orderSummaryText.text = summary;
        orderButton.interactable = currentOrder.Count > 0;
    }

    void SubmitOrder()
    {
        if(currentOrder.Count == 0) return;

        // Send order to server
        OrderManager orderManager = FindObjectOfType<OrderManager>();
        orderManager.CmdSubmitOrder(currentPlayer.netId, currentOrder.ToArray());

        // Close menu
        CloseMenu();

        // Release counter
        currentCounter.OnOrderCompleted();

        // Show order confirmation
        ShowOrderConfirmation();
    }

    void ShowOrderConfirmation()
    {
        string confirmationText = $"Order submitted!\nTotal: ${currentTotal:F2}\nEstimated wait time: {CalculateWaitTime()} minutes";
        FindObjectOfType<NotificationUI>().ShowNotification(confirmationText);
    }

    int CalculateWaitTime()
    {
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        float totalPrepTime = 0f;

        foreach(OrderItem orderItem in currentOrder)
        {
            MenuItem menuItem = menuManager.GetMenuItem(orderItem.menuItemId);
            if(menuItem != null)
            {
                totalPrepTime += menuItem.preparationTime * orderItem.quantity;
            }
        }

        return Mathf.CeilToInt(totalPrepTime / 60f); // Convert to minutes
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentOrder.Clear();
        currentTotal = 0f;
        currentPlayer = null;
        currentCounter = null;
    }
}

// MenuItemUI.cs


public class MenuItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image itemIcon;
    public Text itemName;
    public Text itemDescription;
    public Text itemPrice;
    public Button addButton;
    public Button removeButton;
    public Text quantityText;

    private MenuItem menuItem;
    private MenuUI menuUI;
    private int quantity = 0;

    public void Initialize(MenuItem item, MenuUI menu)
    {
        menuItem = item;
        menuUI = menu;

        itemIcon.sprite = item.icon;
        itemName.text = item.name;
        itemDescription.text = item.description;
        itemPrice.text = $"${item.price:F2}";

        addButton.onClick.AddListener(() => AddItem());
        removeButton.onClick.AddListener(() => RemoveItem());

        UpdateQuantityDisplay();
    }

    void AddItem()
    {
        quantity++;
        menuUI.AddItemToOrder(menuItem);
        UpdateQuantityDisplay();
    }

    void RemoveItem()
    {
        if(quantity > 0)
        {
            quantity--;
            menuUI.RemoveItemFromOrder(menuItem);
            UpdateQuantityDisplay();
        }
    }

    void UpdateQuantityDisplay()
    {
        quantityText.text = quantity.ToString();
        removeButton.interactable = quantity > 0;
    }
}
