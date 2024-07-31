using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerNetwork : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform head;
    [SerializeField] private Transform povCamera;

    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotateSpeed = 10f;

    private bool IsMovementEnabled = false;
    private GameObject currentHitObject;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();

        transform.position = GetComponent<TeleportOnSpawn>().GetSpawnPoint();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!IsMovementEnabled) return;

        MoveCharacter();
        RotateCharacter();
        Interact();
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;

        rb.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
    }

    private void RotateCharacter()
    {
        Vector3 currentRotation = transform.eulerAngles;
        float newYRotation = currentRotation.y;

        if (Input.GetKeyDown(KeyCode.Q))
            newYRotation -= 45;
        if (Input.GetKeyDown(KeyCode.E))
            newYRotation += 45;

        if (Input.GetKey(KeyCode.LeftArrow))
            newYRotation -= rotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            newYRotation += rotateSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(currentRotation.x, newYRotation, currentRotation.z);
    }


    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            Physics.Raycast(povCamera.transform.position, mouseWorldPosition - povCamera.transform.position, out RaycastHit hit, 5f);

            if (hit.transform == null) return;
            currentHitObject = hit.transform.gameObject;
            IInteractable interactable = currentHitObject.GetComponent<IInteractable>();
            if (interactable != null) interactable.Interact();
        }
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (currentHitObject == null) return;
            IInteractable interactable = currentHitObject.GetComponent<IInteractable>(); 
            if (interactable != null) interactable.StopInteract();
            currentHitObject = null;
        }
    }

    public void EnableCameraAndMovement(bool active)
    {
        IsMovementEnabled = active;
        povCamera.gameObject.SetActive(active);
    }

}
