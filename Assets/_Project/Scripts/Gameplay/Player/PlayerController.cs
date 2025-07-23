using Mirror;
using UnityEngine;

namespace CafeConnect3D.Gameplay.Player
{
    /// <summary>
    /// Main player controller for CafeConnect3D
    /// Handles movement, interaction, and network synchronization
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float rotationSpeed = 720.0f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravity = -9.81f;
        
        [Header("Player Info")]
        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        public string playerName = "Unknown";
        
        [SyncVar(hook = nameof(OnPlayerColorChanged))]
        public Color playerColor = Color.white;
        
        // Components
        private CharacterController characterController;
        private Animator animator;
        private Camera playerCamera;
        
        // Movement state
        private Vector3 velocity;
        private bool isGrounded;
        
        // Network state
        private Vector3 lastPosition;
        private float networkSendRate = 20f; // 20 updates per second
        private float lastNetworkSendTime;
        
        #region Unity Lifecycle
        
        void Start()
        {
            InitializeComponents();
            SetupLocalPlayer();
        }
        
        void Update()
        {
            if (!isLocalPlayer) return;
            
            HandleInput();
            HandleMovement();
            HandleNetworkUpdates();
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeComponents()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            playerCamera = GetComponentInChildren<Camera>();
            
            lastPosition = transform.position;
        }
        
        private void SetupLocalPlayer()
        {
            if (isLocalPlayer)
            {
                // Enable camera for local player
                if (playerCamera != null)
                {
                    playerCamera.enabled = true;
                    Camera.main?.gameObject.SetActive(false);
                }
                
                // Setup cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                // Generate random name and color (temporary)
                CmdSetPlayerInfo($"Player_{Random.Range(1000, 9999)}", Random.ColorHSV());
            }
            else
            {
                // Disable camera for remote players
                if (playerCamera != null)
                {
                    playerCamera.enabled = false;
                }
            }
        }
        
        public void Initialize(uint connectionId)
        {
            // Called by NetworkManager when player is created
            Debug.Log($"[PlayerController] Player initialized with connection ID: {connectionId}");
        }
        
        #endregion
        
        #region Input Handling
        
        private void HandleInput()
        {
            // Movement input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            
            if (direction.magnitude >= 0.1f)
            {
                // Calculate movement direction relative to camera
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                Vector3 moveDir = Quaternion.AngleAxis(targetAngle, Vector3.up) * Vector3.forward;
                
                // Move character
                characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
                
                // Rotate character
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                
                // Update animation
                if (animator != null)
                {
                    animator.SetBool("IsWalking", true);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool("IsWalking", false);
                }
            }
            
            // Interaction input
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
            
            // Menu input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }
        }
        
        #endregion
        
        #region Movement
        
        private void HandleMovement()
        {
            // Ground check
            isGrounded = characterController.isGrounded;
            
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            // Jumping (if needed)
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
        
        #endregion
        
        #region Networking
        
        private void HandleNetworkUpdates()
        {
            // Send position updates to server
            if (Time.time - lastNetworkSendTime >= 1f / networkSendRate)
            {
                if (Vector3.Distance(transform.position, lastPosition) > 0.01f)
                {
                    CmdUpdatePosition(transform.position, transform.rotation);
                    lastPosition = transform.position;
                }
                lastNetworkSendTime = Time.time;
            }
        }
        
        [Command]
        private void CmdUpdatePosition(Vector3 position, Quaternion rotation)
        {
            // Server validates and broadcasts position
            if (IsValidPosition(position))
            {
                transform.position = position;
                transform.rotation = rotation;
                RpcUpdatePosition(position, rotation);
            }
        }
        
        [ClientRpc]
        private void RpcUpdatePosition(Vector3 position, Quaternion rotation)
        {
            if (isLocalPlayer) return;
            
            // Smoothly interpolate remote players
            StartCoroutine(InterpolatePosition(position, rotation));
        }
        
        private System.Collections.IEnumerator InterpolatePosition(Vector3 targetPos, Quaternion targetRot)
        {
            float duration = 1f / networkSendRate;
            float elapsed = 0f;
            
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
                
                yield return null;
            }
        }
        
        private bool IsValidPosition(Vector3 position)
        {
            // Add position validation logic here
            // For now, just check if position is within reasonable bounds
            return position.y > -10f && position.y < 100f;
        }
        
        [Command]
        private void CmdSetPlayerInfo(string name, Color color)
        {
            playerName = name;
            playerColor = color;
        }
        
        #endregion
        
        #region Interaction System
        
        private void TryInteract()
        {
            // Raycast to find interactable objects
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 3f))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract(this))
                {
                    interactable.Interact(this);
                }
            }
        }
        
        private void ToggleMenu()
        {
            // Toggle main menu or pause menu
            var uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ToggleMainMenu();
            }
        }
        
        #endregion
        
        #region Network Hooks
        
        private void OnPlayerNameChanged(string oldName, string newName)
        {
            // Update UI or nameplate when player name changes
            Debug.Log($"Player name changed from '{oldName}' to '{newName}'");
        }
        
        private void OnPlayerColorChanged(Color oldColor, Color newColor)
        {
            // Update player appearance when color changes
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = newColor;
            }
        }
        
        #endregion
    }
}