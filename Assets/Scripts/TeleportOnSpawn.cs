using UnityEngine;

public class TeleportOnSpawn : MonoBehaviour
{
    [SerializeField] Transform spawn;

    private void Awake()
    {
        transform.position = spawn.position;
    }


}
