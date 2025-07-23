// IInteractable.cs
using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CafeConnect3D.Gameplay.Player;

public interface IInteractable
{
    void Interact(PlayerController player);
    string GetInteractionPrompt();
    bool CanInteract(PlayerController player);

    // Optional interaction events
    void OnInteractionEnter(PlayerController player) { }
    void OnInteractionExit(PlayerController player) { }
}

// BaseInteractable.cs - Base class for interactive objects
public abstract class BaseInteractable : NetworkBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public string interactionPrompt = "Press E to interact";

    public abstract void Interact(PlayerController player);

    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public virtual bool CanInteract(PlayerController player)
    {
        return true;
    }

    public virtual void OnInteractionEnter(PlayerController player)
    {
        // Override in derived classes if needed
    }

    public virtual void OnInteractionExit(PlayerController player)
    {
        // Override in derived classes if needed
    }
}

// OrderCounter.cs

public class OrderCounter : NetworkBehaviour, IInteractable
{
    [Header("Counter Settings")]
    public Transform customerPosition;
    public GameObject menuUI;

    private PlayerController currentCustomer;

    public void Interact(PlayerController player)
    {
        if(!CanInteract(player)) return;

        // Move player to counter position
        StartCoroutine(MovePlayerToCounter(player));
    }

    public bool CanInteract(PlayerController player)
    {
        return currentCustomer == null;
    }

    public string GetInteractionPrompt()
    {
        return currentCustomer == null ? "Press E to Order" : "Counter Occupied";
    }

    System.Collections.IEnumerator MovePlayerToCounter(PlayerController player)
    {
        currentCustomer = player;

        // Disable player movement temporarily
        player.GetComponent<CharacterController>().enabled = false;

        // Animate player to counter
        float duration = 1f;
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = customerPosition.position;

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            float progress = t / duration;
            player.transform.position = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }

        player.transform.position = targetPos;
        player.transform.LookAt(transform);

        // Show menu UI
        if(player.isLocalPlayer)
        {
            ShowMenuUI(player);
        }

        // Re-enable movement
        player.GetComponent<CharacterController>().enabled = true;
    }

    void ShowMenuUI(PlayerController player)
    {
        MenuUI menuUIComponent = FindObjectOfType<MenuUI>();
        menuUIComponent.ShowMenu(player, this);
    }

    public void OnOrderCompleted()
    {
        currentCustomer = null;
    }
}
