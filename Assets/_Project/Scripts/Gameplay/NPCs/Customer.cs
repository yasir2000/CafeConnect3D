// Customer.cs
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using System.Collections.Generic;

public class Customer : NetworkBehaviour
{
    [Header("Customer Settings")]
    public float orderDecisionTime = 10f;
    public float maxWaitTime = 60f;
    public float walkSpeed = 3.5f;

    [Header("Customer State")]
    [SyncVar] public string customerName;
    [SyncVar] public CustomerState currentState = CustomerState.Entering;
    [SyncVar] public float patience = 100f;
    [SyncVar] public bool hasOrdered = false;

    [Header("Visual")]
    public GameObject thoughtBubble;
    public TMPro.TextMeshPro nameText;

    private NavMeshAgent navAgent;
    private Order currentOrder;
    public Transform targetSeat;
    private float stateTimer;
    private MenuManager menuManager;
    private TableManager tableManager;

    public enum CustomerState
    {
        Entering,
        MovingToSeat,
        Seated,
        Deciding,
        WaitingToOrder,
        OrderTaken,
        WaitingForFood,
        Eating,
        Leaving,
        Left
    }

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = walkSpeed;
        menuManager = FindObjectOfType<MenuManager>();
        tableManager = FindObjectOfType<TableManager>();

        if (isServer)
        {
            GenerateCustomerName();
            InvokeRepeating(nameof(UpdatePatience), 1f, 1f);
        }

        if (nameText != null)
            nameText.text = customerName;
    }

    void Update()
    {
        if (!isServer) return;

        stateTimer += Time.deltaTime;
        UpdateCustomerBehavior();
    }

    [Server]
    void GenerateCustomerName()
    {
        string[] names = { "Alex", "Sam", "Jordan", "Casey", "Taylor", "Morgan", "Riley", "Avery", "Quinn", "Blake" };
        customerName = names[Random.Range(0, names.Length)] + " #" + Random.Range(100, 999);
    }

    [Server]
    public void Initialize(Transform seat = null)
    {
        // Use TableManager to find available seat if none provided
        if (seat == null && tableManager != null)
        {
            seat = tableManager.GetRandomAvailableTable();
        }

        if (seat != null)
        {
            targetSeat = seat;
            currentState = CustomerState.MovingToSeat;
            navAgent.SetDestination(seat.position);
        }
        else
        {
            // Fallback: create default target position
            Vector3 fallbackSeat = transform.position + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            currentState = CustomerState.MovingToSeat;
            navAgent.SetDestination(fallbackSeat);
        }
    }

    [Server]
    void UpdateCustomerBehavior()
    {
        switch (currentState)
        {
            case CustomerState.MovingToSeat:
                if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
                {
                    currentState = CustomerState.Seated;
                    stateTimer = 0f;
                    RpcSitDown();
                }
                break;

            case CustomerState.Seated:
                if (stateTimer >= 2f)
                {
                    currentState = CustomerState.Deciding;
                    stateTimer = 0f;
                    RpcShowThoughtBubble(true);
                }
                break;

            case CustomerState.Deciding:
                if (stateTimer >= orderDecisionTime)
                {
                    GenerateOrder();
                    currentState = CustomerState.WaitingToOrder;
                    stateTimer = 0f;
                    RpcShowThoughtBubble(false);
                    RpcShowOrderReady();
                }
                break;

            case CustomerState.WaitingToOrder:
                // Wait for player interaction
                break;

            case CustomerState.OrderTaken:
                if (stateTimer >= 1f)
                {
                    currentState = CustomerState.WaitingForFood;
                    stateTimer = 0f;
                }
                break;

            case CustomerState.WaitingForFood:
                // Wait for order completion
                break;

            case CustomerState.Eating:
                if (stateTimer >= 15f)
                {
                    LeaveShop();
                }
                break;
        }

        // Check patience
        if (patience <= 0f && currentState != CustomerState.Leaving && currentState != CustomerState.Left)
        {
            LeaveShopAngry();
        }
    }

    [Server]
    void GenerateOrder()
    {
        if (menuManager == null) return;

        List<MenuItem> availableItems = menuManager.GetAvailableMenuItems();
        if (availableItems.Count == 0) return;

        currentOrder = new Order();
        currentOrder.customerId = netId;

        // Generate 1-3 items for the order
        int itemCount = Random.Range(1, 4);
        for (int i = 0; i < itemCount; i++)
        {
            MenuItem randomItem = availableItems[Random.Range(0, availableItems.Count)];
            currentOrder.AddItem(randomItem, 1);
        }

        hasOrdered = true;
    }

    [Server]
    public Order GetOrder()
    {
        return currentOrder;
    }

    [Server]
    public void TakeOrder(NetworkPlayer player)
    {
        if (currentState == CustomerState.WaitingToOrder && hasOrdered)
        {
            currentState = CustomerState.OrderTaken;
            stateTimer = 0f;

            // Give order to kitchen/order manager
            OrderManager.Instance?.ReceiveOrder(currentOrder, player);
            RpcOrderTaken();
        }
    }

    [Server]
    public void ReceiveFood()
    {
        if (currentState == CustomerState.WaitingForFood)
        {
            currentState = CustomerState.Eating;
            stateTimer = 0f;
            patience = 100f; // Reset patience while eating
            RpcStartEating();

            // Pay and complete transaction
            float orderValue = currentOrder.GetTotalPrice();
            GameManager.Instance?.CustomerServed(this, orderValue);
        }
    }

    [Server]
    void LeaveShop()
    {
        currentState = CustomerState.Leaving;
        Transform exitPoint = GameObject.FindGameObjectWithTag("Exit")?.transform;
        if (exitPoint != null)
        {
            navAgent.SetDestination(exitPoint.position);
        }
        RpcLeaveShop(false);

        Invoke(nameof(DestroyCustomer), 5f);
    }

    [Server]
    void LeaveShopAngry()
    {
        currentState = CustomerState.Leaving;
        Transform exitPoint = GameObject.FindGameObjectWithTag("Exit")?.transform;
        if (exitPoint != null)
        {
            navAgent.SetDestination(exitPoint.position);
        }
        RpcLeaveShop(true);

        Invoke(nameof(DestroyCustomer), 5f);
    }

    [Server]
    void UpdatePatience()
    {
        if (currentState == CustomerState.WaitingToOrder || currentState == CustomerState.WaitingForFood)
        {
            patience -= Random.Range(1f, 3f);
            patience = Mathf.Clamp(patience, 0f, 100f);
        }
    }

    [Server]
    void DestroyCustomer()
    {
        NetworkServer.Destroy(gameObject);
    }

    // Client RPCs for visual feedback
    [ClientRpc]
    void RpcSitDown()
    {
        // Play sit animation
        GetComponent<Animator>()?.SetTrigger("Sit");
    }

    [ClientRpc]
    void RpcShowThoughtBubble(bool show)
    {
        if (thoughtBubble != null)
            thoughtBubble.SetActive(show);
    }

    [ClientRpc]
    void RpcShowOrderReady()
    {
        // Show visual indicator that customer is ready to order
        UIManager.Instance?.ShowCustomerReadyToOrder(netId);
    }

    [ClientRpc]
    void RpcOrderTaken()
    {
        // Visual feedback for order taken
        AudioManager.Instance?.PlayOrderTakenSound();
    }

    [ClientRpc]
    void RpcStartEating()
    {
        // Play eating animation
        GetComponent<Animator>()?.SetTrigger("Eat");
    }

    [ClientRpc]
    void RpcLeaveShop(bool angry)
    {
        if (angry)
        {
            // Show angry emote
            AudioManager.Instance?.PlayAngryCustomerSound();
        }
        else
        {
            // Show happy emote
            AudioManager.Instance?.PlayHappyCustomerSound();
        }
    }
}
