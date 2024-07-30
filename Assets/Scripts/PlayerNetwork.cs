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
    [SerializeField][Range(0.5f, 3)] private float rotateSpeed = 0.5f;

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

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (!IsMovementEnabled) return;

        RotateCharacterInFixedUpdate();
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
        if (Input.GetKeyDown(KeyCode.Q))
            transform.Rotate(0, -45, 0);
        if (Input.GetKeyDown(KeyCode.E))
            transform.Rotate(0, 45, 0);
    }

    private void RotateCharacterInFixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(0, -rotateSpeed, 0);
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(0, rotateSpeed, 0);
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
