using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Q1Answer
{
    Open,
    Situationship,
    Polyamorous,
    Exclusive,
    Asexual,
}

public class Q1_ButtonsInWorldSpace : NetworkBehaviour
{
    [SerializeField] Button[] buttons;

    private void Awake()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Q1Answer q1Answer = (Q1Answer)i;

            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = q1Answer.ToString();
            buttons[i].onClick.AddListener(() => ChooseAnswerServerRpc(q1Answer));
            buttons[i].onClick.AddListener(Progress);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChooseAnswerServerRpc(Q1Answer q1answer, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        PlayerAnswers playerAnswers = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>();
        playerAnswers.NetworkQ1Answer.Value = q1answer;
    }

    private void Progress()
    {
        transform.position = new Vector3(transform.position.x, -20, transform.position.z);
        ProgressGame.Progress();
    }

}
