using Unity.Netcode;
using UnityEngine;

public class TeleportOnSpawn : NetworkBehaviour
{
    [SerializeField] private Transform spawnPoint;


    private void Awake()
    {
        if (spawnPoint == null) return;
        transform.position = spawnPoint.position;
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint.position;
    }

    [ClientRpc]
    public void MoveOnSpawnClientRpc(Vector3 newPos)
    {
        transform.position = newPos;
    }

    [ClientRpc]
    public void RotateAndMoveClientRpc(Vector3 newPos, Vector3 newRot)
    {
        transform.position = newPos;
        transform.eulerAngles = newRot;
    }
}
