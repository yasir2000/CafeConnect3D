// NetworkPlayer.cs
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Player Settings")]
    [SyncVar] public string playerName = "Player";
    [SyncVar] public PlayerRole role = PlayerRole.Waiter;
    [SyncVar] public int playerScore = 0;

    [Header("Interaction")]
    public float interactionRange = 3f;
    public LayerMask interactableLayer;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Camera playerCamera;
    private CharacterController characterController;
    private IInteractable currentInteractable;
    private Customer currentCustomer;

    public enum PlayerRole
    {
        Waiter,
        Barista,
        Manager
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            playerCamera = Camera.main;
            characterController = GetComponent<CharacterController>();

            // Register with game manager
            if (GameManager.Instance != null)
            {
                CmdRegisterPlayer(playerName);
            }
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        HandleInteraction();
        CheckForInteractables();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            // Move relative to camera direction
            Vector3 forward = playerCamera.transform.forward;
            Vector3 right = playerCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * vertical + right * horizontal;

            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Rotate player to face movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Apply gravity
        characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(this);
            }
            else if (currentCustomer != null)
            {
                // Take order from customer
                CmdTakeOrder(currentCustomer.netId);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Open/close menu or order board
            UIManager.Instance?.ToggleOrderBoard();
        }
    }

    void CheckForInteractables()
    {
        // Raycast to find interactables
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
        Vector3 rayDirection = transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable?.OnInteractionExit(this);
                    currentInteractable = interactable;
                    currentInteractable.OnInteractionEnter(this);
                }
            }
            else
            {
                // Check for customer
                Customer customer = hit.collider.GetComponent<Customer>();
                if (customer != null && customer.currentState == Customer.CustomerState.WaitingToOrder)
                {
                    if (currentCustomer != customer)
                    {
                        currentCustomer = customer;
                        UIManager.Instance?.ShowInteractionPrompt("Press E to take order");
                    }
                }
                else
                {
                    if (currentCustomer != null)
                    {
                        currentCustomer = null;
                        UIManager.Instance?.HideInteractionPrompt();
                    }
                }
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnInteractionExit(this);
                currentInteractable = null;
            }
            if (currentCustomer != null)
            {
                currentCustomer = null;
                UIManager.Instance?.HideInteractionPrompt();
            }
        }
    }

    [Command]
    void CmdRegisterPlayer(string name)
    {
        playerName = name;
        GameManager.Instance?.RegisterPlayer(this);
    }

    [Command]
    void CmdTakeOrder(uint customerId)
    {
        Customer customer = FindObjectOfType<Customer>();
        Customer[] customers = FindObjectsOfType<Customer>();

        foreach (Customer c in customers)
        {
            if (c.netId == customerId)
            {
                c.TakeOrder(this);
                break;
            }
        }
    }

    [Command]
    void CmdCompleteOrder(uint orderId)
    {
        OrderManager.Instance?.CompleteOrder(orderId, this);
    }

    [Command]
    void CmdStartPreparingOrder(uint orderId)
    {
        OrderManager.Instance?.StartPreparingOrder(orderId, this);
    }

    // Public methods for OrderTicketUI to call
    public void CmdCompleteOrderPublic(uint orderId)
    {
        if (isLocalPlayer)
        {
            CmdCompleteOrder(orderId);
        }
    }

    public void CmdStartPreparingOrderPublic(uint orderId)
    {
        if (isLocalPlayer)
        {
            CmdStartPreparingOrder(orderId);
        }
    }

    public void AddScore(int points)
    {
        if (isServer)
        {
            playerScore += points;
        }
        else
        {
            CmdAddScore(points);
        }
    }

    [Command]
    void CmdAddScore(int points)
    {
        playerScore += points;
    }

    public void SetRole(PlayerRole newRole)
    {
        if (isServer)
        {
            role = newRole;
        }
        else
        {
            CmdSetRole(newRole);
        }
    }

    [Command]
    void CmdSetRole(PlayerRole newRole)
    {
        role = newRole;
    }

    void OnDrawGizmosSelected()
    {
        // Draw interaction range
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
        Gizmos.DrawRay(rayOrigin, transform.forward * interactionRange);
    }
}
