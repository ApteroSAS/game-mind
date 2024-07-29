using UnityEngine;

public class Q1_Labyrinth : MonoBehaviour
{
    [SerializeField] Transform teleportSpot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProgressGame.Progress();

            TeleportOnSpawn tel = other.gameObject.GetComponent<TeleportOnSpawn>();
            Vector3 newPos = tel.GetSpawnPoint();
            other.transform.position = newPos;
            other.transform.localEulerAngles = Vector3.zero;

        }
    }

    public Transform GetTeleportSpot()
    {
        return teleportSpot;
    }
}
