using Unity.Netcode;
using UnityEngine;

public class TeleportOnSpawn : NetworkBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        transform.position = spawnPoint.position;
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint.position;
    }

    [ClientRpc]
    public void MoveOnSpawnClientRpc(Vector3 newPos)
    {
        Debug.Log("I should be moving the podest");
        transform.position = newPos;
    }
}
