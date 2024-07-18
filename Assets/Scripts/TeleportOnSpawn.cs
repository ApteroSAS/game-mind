using UnityEngine;

public class TeleportOnSpawn : MonoBehaviour
{
    [SerializeField] private Transform spawn;

    private void Awake()
    {
        transform.position = spawn.position;
    }

    public Transform GetSpawn()
    {
        return spawn;
    }


}
