// OrderTicketUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderTicketUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI orderIdText;
    public TextMeshProUGUI customerNameText;
    public TextMeshProUGUI itemsText;
    public TextMeshProUGUI totalPriceText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI timerText;
    public Button startButton;
    public Button completeButton;
    public Image progressBar;

    private uint orderId;
    private float orderStartTime;
    private bool isInProgress = false;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartOrder);

        if (completeButton != null)
            completeButton.onClick.AddListener(CompleteOrder);
    }

    void Update()
    {
        if (isInProgress)
        {
            UpdateTimer();
            UpdateProgressBar();
        }
    }

    public void SetOrder(uint id, Order order)
    {
        orderId = id;

        if (orderIdText != null)
            orderIdText.text = $"Order #{id}";

        if (customerNameText != null)
            customerNameText.text = $"Customer #{order.customerId}";

        if (itemsText != null)
        {
            string itemsList = "";
            foreach (var item in order.items)
            {
                itemsList += $"• {item.itemName} x{item.quantity}\n";
            }
            itemsText.text = itemsList;
        }

        if (totalPriceText != null)
            totalPriceText.text = $"${order.GetTotalPrice():F2}";

        UpdateStatus("Waiting");

        if (startButton != null)
            startButton.gameObject.SetActive(true);

        if (completeButton != null)
            completeButton.gameObject.SetActive(false);
    }

    public void UpdateStatus(string status)
    {
        if (statusText != null)
            statusText.text = status;
    }

    void StartOrder()
    {
        NetworkPlayer player = FindObjectOfType<NetworkPlayer>();
        if (player != null && player.isLocalPlayer)
        {
            // Send command to start preparing order
            player.CmdStartPreparingOrderPublic(orderId);
        }

        isInProgress = true;
        orderStartTime = Time.time;

        UpdateStatus("In Progress");

        if (startButton != null)
            startButton.gameObject.SetActive(false);

        if (completeButton != null)
            completeButton.gameObject.SetActive(true);
    }

    void CompleteOrder()
    {
        NetworkPlayer player = FindObjectOfType<NetworkPlayer>();
        if (player != null && player.isLocalPlayer)
        {
            // Send command to complete order
            player.CmdCompleteOrderPublic(orderId);
        }

        UpdateStatus("Completed");

        if (completeButton != null)
            completeButton.gameObject.SetActive(false);

        // This ticket will be removed by the OrderManager
    }

    void UpdateTimer()
    {
        if (timerText != null)
        {
            float elapsed = Time.time - orderStartTime;
            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            float elapsed = Time.time - orderStartTime;
            float maxTime = 30f; // Maximum time for order completion
            float progress = elapsed / maxTime;
            progressBar.fillAmount = Mathf.Clamp01(progress);

            // Change color based on progress
            if (progress < 0.5f)
                progressBar.color = Color.green;
            else if (progress < 0.8f)
                progressBar.color = Color.yellow;
            else
                progressBar.color = Color.red;
        }
    }
}
