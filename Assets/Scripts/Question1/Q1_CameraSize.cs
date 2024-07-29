using UnityEngine;

public class Q1_CameraSize : MonoBehaviour
{
    public void ChangeCameraSize()
    {
        GetComponent<Camera>().orthographicSize = 17;
    }
}
