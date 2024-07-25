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
    [SerializeField] private GameObject Q3BlockResult;
    public NetworkList<Q3_HoldData> NetworkQ3Blocks  = new();

    //question4
    [SerializeField] private GameObject Q4CauldronResult;
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

    [ServerRpc(RequireOwnership = false)]
    public void AddToNetworkQ3BlocksServerRpc(Q3_HoldData q3_data, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        NetworkQ3Blocks.Add(q3_data);
    }

    public void ShowResults(bool isHost)
    {
        float offsetX = 3;
        if (isHost) offsetX *= -1;

        //question3
        for (int i = 0; i < NetworkQ3Blocks.Count; i++)
        {
            var Q3BlockInstance = Instantiate(Q3BlockResult);
            Vector3 pos = NetworkQ3Blocks[i].PositionData;
            pos.x += offsetX;
            Q3BlockInstance.GetComponent<NetworkObject>().Spawn();
            Q3BlockInstance.GetComponent<Q3_Results>().OnSpawnClientRpc(NetworkQ3Blocks[i].SymbolData, pos);
        }

        //question4

    }


}
