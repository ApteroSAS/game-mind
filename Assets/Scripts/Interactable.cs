using Unity.Netcode;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    public abstract void Interact(GameObject gameObject);
}
