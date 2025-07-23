// UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject orderBoardPanel;
    public GameObject customerOrderPanel;

    [Header("Game UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI revenueText;
    public TextMeshProUGUI customersServedText;
    public TextMeshProUGUI playerCountText;

    [Header("Interaction UI")]
    public GameObject interactionPrompt;
    public TextMeshProUGUI interactionText;

    [Header("Order Board")]
    public Transform orderListParent;
    public GameObject orderTicketPrefab;

    [Header("Notifications")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;

    [Header("Customer Order UI")]
    public TextMeshProUGUI customerNameText;
    public Transform orderItemsParent;
    public GameObject orderItemPrefab;
    public TextMeshProUGUI orderTotalText;
    public Button confirmOrderButton;

    private Dictionary<uint, GameObject> activeOrderTickets = new Dictionary<uint, GameObject>();

    public static UIManager Instance { get; private set; }

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

    void Start()
    {
        ShowMainMenu();

        if (confirmOrderButton != null)
        {
            confirmOrderButton.onClick.AddListener(ConfirmCurrentOrder);
        }
    }

    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
        gameUIPanel?.SetActive(false);
        orderBoardPanel?.SetActive(false);
        customerOrderPanel?.SetActive(false);
    }

    public void ShowGameUI()
    {
        mainMenuPanel?.SetActive(false);
        gameUIPanel?.SetActive(true);
        orderBoardPanel?.SetActive(false);
        customerOrderPanel?.SetActive(false);
    }

    public void UpdateScore(int customersServed, float revenue)
    {
        if (customersServedText != null)
            customersServedText.text = $"Customers Served: {customersServed}";

        if (revenueText != null)
            revenueText.text = $"Revenue: ${revenue:F2}";
    }

    public void UpdatePlayerCount(int playerCount)
    {
        if (playerCountText != null)
            playerCountText.text = $"Players: {playerCount}";
    }

    public void ShowInteractionPrompt(string message)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            if (interactionText != null)
                interactionText.text = message;
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    public void ToggleOrderBoard()
    {
        if (orderBoardPanel != null)
        {
            bool isActive = orderBoardPanel.activeSelf;
            orderBoardPanel.SetActive(!isActive);
        }
    }

    public void DisplayNewOrder(uint orderId, Order order)
    {
        if (orderTicketPrefab != null && orderListParent != null)
        {
            GameObject ticket = Instantiate(orderTicketPrefab, orderListParent);
            activeOrderTickets[orderId] = ticket;

            OrderTicketUI ticketUI = ticket.GetComponent<OrderTicketUI>();
            if (ticketUI != null)
            {
                ticketUI.SetOrder(orderId, order);
            }
        }
    }

    public void RemoveOrderFromBoard(uint orderId)
    {
        if (activeOrderTickets.ContainsKey(orderId))
        {
            GameObject ticket = activeOrderTickets[orderId];
            activeOrderTickets.Remove(orderId);

            if (ticket != null)
                Destroy(ticket);
        }
    }

    public void UpdateOrderStatus(uint orderId, string status)
    {
        if (activeOrderTickets.ContainsKey(orderId))
        {
            GameObject ticket = activeOrderTickets[orderId];
            OrderTicketUI ticketUI = ticket.GetComponent<OrderTicketUI>();
            if (ticketUI != null)
            {
                ticketUI.UpdateStatus(status);
            }
        }
    }

    public void ShowCustomerReadyToOrder(uint customerId)
    {
        ShowNotification($"Customer #{customerId} is ready to order!");
    }

    public void ShowPlayerJoinedMessage(string playerName)
    {
        ShowNotification($"{playerName} joined the cafe!");
    }

    public void ShowOrderFailedMessage(uint orderId)
    {
        ShowNotification($"Order #{orderId} failed - customer left angry!");
    }

    public void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);

            // Auto-hide after 3 seconds
            Invoke(nameof(HideNotification), 3f);
        }
    }

    void HideNotification()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    public void ShowCustomerOrder(Customer customer)
    {
        if (customerOrderPanel != null)
        {
            customerOrderPanel.SetActive(true);

            Order order = customer.GetOrder();
            if (order != null)
            {
                DisplayCustomerOrder(customer.customerName, order);
            }
        }
    }

    void DisplayCustomerOrder(string customerName, Order order)
    {
        if (customerNameText != null)
            customerNameText.text = customerName;

        // Clear existing items
        foreach (Transform child in orderItemsParent)
        {
            Destroy(child.gameObject);
        }

        // Add order items
        foreach (var item in order.items)
        {
            GameObject itemObj = Instantiate(orderItemPrefab, orderItemsParent);
            TextMeshProUGUI itemText = itemObj.GetComponent<TextMeshProUGUI>();
            if (itemText != null)
            {
                itemText.text = $"{item.itemName} x{item.quantity} - ${item.GetItemTotal():F2}";
            }
        }

        if (orderTotalText != null)
            orderTotalText.text = $"Total: ${order.GetTotalPrice():F2}";
    }

    void ConfirmCurrentOrder()
    {
        // This would be called when player confirms taking the order
        customerOrderPanel?.SetActive(false);
    }

    public void HideCustomerOrder()
    {
        if (customerOrderPanel != null)
            customerOrderPanel.SetActive(false);
    }

    // Button handlers for main menu
    public void OnStartGameClicked()
    {
        // Connect to server or start hosting
        NetworkManager networkManager = FindObjectOfType<CafeNetworkManager>();
        if (networkManager != null)
        {
            networkManager.StartHost();
        }
    }

    public void OnJoinGameClicked()
    {
        // Join existing game
        NetworkManager networkManager = FindObjectOfType<CafeNetworkManager>();
        if (networkManager != null)
        {
            networkManager.StartClient();
        }
    }

    public void OnQuitGameClicked()
    {
        Application.Quit();
    }
}
