using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Q1_Results : NetworkBehaviour
{
    [SerializeField] GameObject easyLabyrinth;
    [SerializeField] GameObject hardLabyrinth;


    [ClientRpc]
    public void OnSpawnClientRpc(Vector3 pos, Q1Answer q1Answer, bool sameAnswer)
    {
        transform.position = pos;
        GetComponentInChildren<TextMeshProUGUI>().text = q1Answer.ToString();

        pos.y += 2;
        if (sameAnswer)
        {
            var labyrinthInstance = Instantiate(easyLabyrinth);
            labyrinthInstance.transform.position = pos;
        }
        else
        {
            var labyrinthInstance = Instantiate(hardLabyrinth);
            labyrinthInstance.transform.position = pos;
        }
    }
}
