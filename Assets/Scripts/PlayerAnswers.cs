using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public enum PlayerAttribute
{
    Fire,
    Ice,
}



public class PlayerAnswers : NetworkBehaviour
{
    private static PlayerAttribute hostAttribute;

    public NetworkVariable<PlayerAttribute> NetworkAttribute = new NetworkVariable<PlayerAttribute>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            AssignAttributeServerRpc();
        }
        else if (IsClient && IsOwner)
        {
            AssignAttributeClientRpc();
        }
    }

    [ServerRpc]
    public void AssignAttributeServerRpc()
    {
        if (!IsServer) return;

        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            // This is the host
            int randomInt = UnityEngine.Random.Range(0, 2);
            hostAttribute = randomInt == 0 ? PlayerAttribute.Ice : PlayerAttribute.Fire;
            NetworkAttribute.Value = hostAttribute;
        }
        else
        {
            // This is the second player
            NetworkAttribute.Value = hostAttribute == PlayerAttribute.Ice ? PlayerAttribute.Fire : PlayerAttribute.Ice;
        }

        Debug.Log($"Player {OwnerClientId} assigned attribute: {NetworkAttribute.Value}");
    }

    [ClientRpc]
    public void AssignAttributeClientRpc()
    {
        if (!IsOwner) return;

        if (IsServer)
        {
            // Assign attribute directly on the server
            AssignAttributeServerRpc();
        }
        else
        {
            // Request attribute assignment from the server
            AssignAttributeServerRpc();
        }
    }
}
