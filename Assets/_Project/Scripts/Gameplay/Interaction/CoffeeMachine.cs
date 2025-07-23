// CoffeeMachine.cs
using UnityEngine;
using Mirror;
using CafeConnect3D.Gameplay.Player;

public class CoffeeMachine : BaseInteractable
{
    [Header("Coffee Machine Settings")]
    public float brewTime = 10f;
    public AudioClip brewingSound;
    public ParticleSystem steamEffect;

    [Header("Machine State")]
    [SyncVar] public bool isBrewing = false;
    [SyncVar] public float brewProgress = 0f;

    private AudioSource audioSource;
    private float brewStartTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Set interaction prompt through the base class property
        interactionPrompt = "Press E to brew coffee";
    }

    void Update()
    {
        if (isBrewing && isServer)
        {
            float elapsed = Time.time - brewStartTime;
            brewProgress = elapsed / brewTime;

            if (brewProgress >= 1f)
            {
                CompleteBrewing();
            }
        }
    }

    public override bool CanInteract(PlayerController player)
    {
        return !isBrewing;
    }

    public override string GetInteractionPrompt()
    {
        if (isBrewing)
            return $"Brewing... {(brewProgress * 100):F0}%";

        return "Press E to start brewing coffee";
    }

    public override void Interact(PlayerController player)
    {
        if (!CanInteract(player)) return;

        if (isServer)
        {
            StartBrewing();
        }
        else
        {
            CmdStartBrewing();
        }
    }

    [Command(requiresAuthority = false)]
    void CmdStartBrewing()
    {
        StartBrewing();
    }

    [Server]
    void StartBrewing()
    {
        isBrewing = true;
        brewProgress = 0f;
        brewStartTime = Time.time;

        RpcStartBrewingEffects();
    }

    [ClientRpc]
    void RpcStartBrewingEffects()
    {
        // Play brewing sound
        if (audioSource != null && brewingSound != null)
        {
            audioSource.clip = brewingSound;
            audioSource.Play();
        }

        // Start steam effects
        if (steamEffect != null)
        {
            steamEffect.Play();
        }
    }

    [Server]
    void CompleteBrewing()
    {
        isBrewing = false;
        brewProgress = 1f;

        RpcBrewingComplete();
    }

    [ClientRpc]
    void RpcBrewingComplete()
    {
        // Stop brewing sound
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Stop steam effects
        if (steamEffect != null)
        {
            steamEffect.Stop();
        }

        // Play completion sound
        AudioManager.Instance?.PlayBrewCompleteSound();
    }
}
