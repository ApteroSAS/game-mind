using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Q1_ButtonsInWorldSpace : NetworkBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] string[] textOfButton;

    private void Awake()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = textOfButton[i];
        }
    }

}
