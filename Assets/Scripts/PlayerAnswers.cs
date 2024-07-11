using System;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Properties;
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
    public NetworkVariable<float> NetworkSexMeter = new NetworkVariable<float>();

    private float timer = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            AssignAttributeServerRpc();
            NetworkSexMeter.Value = 0.5f;

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

    [ClientRpc]
    private void TestClientRpc()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        //Debug.Log("I got triggered by client: " + clientId);
    }

    [ServerRpc (RequireOwnership = false)]
    private void TestServerRpc()
    {
        TestClientRpc();
    }

    private void Update()
    {
        if (!IsOwner) return;
        timer += Time.deltaTime;
        if(timer > 5)
        {
            timer -= 5;
            TestServerRpc();
        }
    }
}
