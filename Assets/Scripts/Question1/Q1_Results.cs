using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Q1_Results : NetworkBehaviour
{

    [ClientRpc]
    public void OnSpawnClientRpc(Vector3 pos, Q1Answer q1Answer)
    {
        transform.position = pos;
        GetComponentInChildren<TextMeshProUGUI>().text = q1Answer.ToString();
    }
}
