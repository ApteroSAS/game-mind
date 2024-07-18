using System;
using System.Collections.Generic;
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

    //question3
    public List<Q3_HoldData> Q3Blocks = new();

    //question4
    public NetworkVariable<PlayerAttribute> NetworkAttribute = new NetworkVariable<PlayerAttribute>();
    public NetworkVariable<float> NetworkSexMeter = new NetworkVariable<float>();


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            AssignAttributeServerRpc();
            float randomFloat = UnityEngine.Random.Range(0.2f, 0.8f);
            NetworkSexMeter.Value = randomFloat;

            var playerSpawn = FindFirstObjectByType<GameManager>().playerSpawn;
            transform.position = playerSpawn.position;
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

}
