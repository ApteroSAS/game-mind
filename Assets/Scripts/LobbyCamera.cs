using UnityEngine;

public class LobbyCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1;

    void Update()
    {
        Vector3 currentRotation = transform.eulerAngles;

        float newYRotation = currentRotation.y + rotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(currentRotation.x, newYRotation, currentRotation.z);
    }
}
