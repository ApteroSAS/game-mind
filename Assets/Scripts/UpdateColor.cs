using UnityEngine;
using UnityEngine.UI;

public class UpdateColor : MonoBehaviour
{

    public void IsConnected(bool connected)
    {
        if (connected)
        {
            GetComponent<Image>().color = Color.green;
        } else GetComponent<Image>().color = Color.red;
    }
}
