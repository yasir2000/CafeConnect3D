// IInteractable.cs
using UnityEngine;

public interface IInteractable
{
    string GetInteractionText();
    void Interact(NetworkPlayer player);
    bool CanInteract(NetworkPlayer player);
    void OnInteractionEnter(NetworkPlayer player);
    void OnInteractionExit(NetworkPlayer player);
}

// Base interactable class for common functionality
public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public string interactionText = "Press E to interact";
    public bool requiresSpecificRole = false;
    public NetworkPlayer.PlayerRole requiredRole;
    public float cooldownTime = 0f;

    private float lastInteractionTime;

    public virtual string GetInteractionText()
    {
        return interactionText;
    }

    public virtual bool CanInteract(NetworkPlayer player)
    {
        if (Time.time - lastInteractionTime < cooldownTime)
            return false;

        if (requiresSpecificRole && player.role != requiredRole)
            return false;

        return true;
    }

    public virtual void OnInteractionEnter(NetworkPlayer player)
    {
        if (CanInteract(player))
        {
            UIManager.Instance?.ShowInteractionPrompt(GetInteractionText());
        }
    }

    public virtual void OnInteractionExit(NetworkPlayer player)
    {
        UIManager.Instance?.HideInteractionPrompt();
    }

    public abstract void Interact(NetworkPlayer player);

    protected void SetLastInteractionTime()
    {
        lastInteractionTime = Time.time;
    }
}
