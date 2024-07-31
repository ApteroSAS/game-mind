using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public enum PlayerAttribute
{
    Fire,
    Ice,
}

public class PlayerAnswers : NetworkBehaviour
{
    private static PlayerAttribute hostAttribute;

    //question1
    public NetworkVariable<Q1Answer> NetworkQ1Answer = new();

    //question3
    public NetworkList<Q3_HoldData> NetworkQ3Blocks  = new();

    //question4
    public NetworkVariable<PlayerAttribute> NetworkAttribute = new();
    public NetworkVariable<float> NetworkSexMeter = new();


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            AssignAttributeServerRpc();
            float randomFloat = UnityEngine.Random.Range(0.2f, 0.8f);
            NetworkSexMeter.Value = randomFloat;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AssignAttributeServerRpc()
    {
        if (!IsServer) return;

        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            int randomInt = UnityEngine.Random.Range(0, 2);
            hostAttribute = randomInt == 0 ? PlayerAttribute.Ice : PlayerAttribute.Fire;
            NetworkAttribute.Value = hostAttribute;
        }
        else
        {
            NetworkAttribute.Value = hostAttribute == PlayerAttribute.Ice ? PlayerAttribute.Fire : PlayerAttribute.Ice;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddToNetworkQ3BlocksServerRpc(Q3_HoldData q3_data, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        NetworkQ3Blocks.Add(q3_data);
    }
}
