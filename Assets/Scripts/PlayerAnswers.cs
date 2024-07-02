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
    public NetworkVariable<float> NetworkSexMeter = new NetworkVariable<float>(0.5f);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            AssignAttributeServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
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

    [ServerRpc(RequireOwnership = false)]
    public void ChangeSexMeterValueByClientIdServerRpc(ulong targetClientId, float newAmount)
    {
        PlayerAnswers targetPlayer = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject.GetComponent<PlayerAnswers>();
        targetPlayer.NetworkSexMeter.Value = newAmount;
    }
}
