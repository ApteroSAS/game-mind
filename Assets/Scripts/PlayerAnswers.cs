using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum PlayerAttribute
{
    Fire,
    Ice,
}

public class PlayerAnswers : NetworkBehaviour
{
    private static PlayerAttribute hostAttribute;
    private List<GameObject> spawnedInstances = new();

    //question1
    [SerializeField] private GameObject Q1Result;
    public NetworkVariable<Q1Answer> NetworkQ1Answer = new();
    private bool sameAnswer = false;

    //question3
    [SerializeField] private GameObject Q3BlockResult;
    [SerializeField] private GameObject Q3PodestPrefab;
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
        float offsetX = 3.5f;
        if (isHost) offsetX *= -1;

        //question1
        var Q1Instance = Instantiate(Q1Result);
        Vector3 q1pos = ApplyOffsetToVector3(Q1Instance.GetComponent<TeleportOnSpawn>().GetSpawnPoint(), offsetX);
        Q1Instance.GetComponent<NetworkObject>().Spawn();
        Q1Instance.GetComponent<Q1_Results>().OnSpawnClientRpc(q1pos, NetworkQ1Answer.Value, sameAnswer);

        //question3
        var Q3PodestInstance = Instantiate(Q3PodestPrefab);
        Vector3 podestPos = ApplyOffsetToVector3(Q3PodestInstance.GetComponent<TeleportOnSpawn>().GetSpawnPoint(), offsetX);
        Q3PodestInstance.GetComponent<NetworkObject>().Spawn();
        Q3PodestInstance.GetComponent<TeleportOnSpawn>().MoveOnSpawnClientRpc(podestPos);

        for (int i = 0; i < NetworkQ3Blocks.Count; i++)
        {
            var Q3BlockInstance = Instantiate(Q3BlockResult);
            Vector3 blockPos = ApplyOffsetToVector3(NetworkQ3Blocks[i].PositionData, offsetX);
            Q3BlockInstance.GetComponent<NetworkObject>().Spawn();
            Q3BlockInstance.GetComponent<Q3_Results>().OnSpawnClientRpc(NetworkQ3Blocks[i].SymbolData, blockPos);
        }

        //question4
        var Q4CauldronInstance = Instantiate(Q4CauldronResult);
        Vector3 cauldronPos = ApplyOffsetToVector3(Q4CauldronInstance.transform.position, offsetX);
        Q4CauldronInstance.GetComponent<NetworkObject>().Spawn();
        Q4CauldronInstance.GetComponent<Q4_Results>().OnSpawnClientRpc(NetworkSexMeter.Value, isHost, cauldronPos);
    }

    private Vector3 ApplyOffsetToVector3(Vector3 origin, float offsetX)
    {
        Vector3 newPos = origin;
        newPos.x += offsetX;
        return newPos;
    }

    public void SetSameAnswer(bool answer)
    {
        sameAnswer = answer;
    }


}
