using Unity.Netcode;
using UnityEngine;

public class Notification : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        NetworkManager.Singleton.OnClientConnectedCallback += PlaySoundOnClientJoin;
        FindFirstObjectByType<GameManager>().OnGameStateChangeAddListener(PlaySoundOnLabyrinthSpawn);
    }

    private void PlaySoundOnClientJoin(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            ulong hostClientId = NetworkManager.Singleton.LocalClientId;
            if (hostClientId == clientId) return;
        }
        audioSource.Play();
    }

    private void PlaySoundOnLabyrinthSpawn(GameState gameState)
    {
        if (gameState == GameState.Q1Labyrinth)
        {
            audioSource.Play();
        }
    }

}
