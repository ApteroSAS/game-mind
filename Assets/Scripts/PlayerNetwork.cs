using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerNetwork : NetworkBehaviour
{
    private InputSystem_Actions inputAction;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interAction;

    private Rigidbody rb;
    private CapsuleCollider cc;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField][Range(0.05f, 1f)] private float rotateSpeed = 0.5f;
    [SerializeField][Range(4, 6)] private float jumpStrength = 5;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool noobControls = true;

    private bool IsMovementEnabled = true;
    private Transform head;
    private Transform povCamera;
    private float groundOffset;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        base.OnNetworkSpawn();

        if(!noobControls) inputAction = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        groundOffset = cc.height / 2 * transform.localScale.y;
        head = FindChildWithTag(transform, "PlayerHead");
        povCamera = FindChildWithTag(transform, "MainCamera");
        povCamera.gameObject.SetActive(true);
    }

    private void EditorStuff()
    {
        inputAction = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        if (noobControls) return;
        if (inputAction == null) EditorStuff();

        moveAction = inputAction.Player.Move;
        moveAction.Enable();

        lookAction = inputAction.Player.Look;
        lookAction.Enable();

        interAction = inputAction.Player.Interact;
        interAction.Enable();

        //inputAction.Player.Interact.performed += OnInteract;
        //inputAction.Player.Interact.Enable();

        inputAction.Player.Jump.performed += OnJump;
        inputAction.Player.Jump.Enable();

        Debug.Log("Controls enabled");
    }

    private void OnDisable()
    {
        if (noobControls) return;
        moveAction.Disable();
        lookAction.Disable();
        interAction.Disable();

        //inputAction.Player.Interact.performed -= OnInteract;
        //inputAction.Player.Interact.Disable();

        inputAction.Player.Jump.performed -= OnJump;
        inputAction.Player.Jump.Disable();

        Debug.Log("Controls disabled");
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!IsMovementEnabled) return;
        if (noobControls)
        {
            RotateCharacterNoob(true);
            MoveCharacterNoob();
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (!IsMovementEnabled) return;

        if (noobControls)
        {
            RotateCharacterNoob(false);
            InteractNoob();
        }
        else
        {
            RotateCharacter();
            MoveCharacter();
            Interact();
        }
    }

    private void MoveCharacter()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();

        //Vector3 moveRight = moveDirection.x * moveSpeed * transform.right;
        //Vector3 moveForward = moveDirection.y * moveSpeed * transform.forward;
        //Vector3 vel = moveRight + moveForward + new Vector3(0, rb.linearVelocity.y, 0);
        //rb.linearVelocity = vel;

        transform.position += Time.fixedDeltaTime * moveSpeed * (transform.right * moveDirection.x);
        transform.position += Time.fixedDeltaTime * moveSpeed * (transform.forward * moveDirection.y);
    }

    private void MoveCharacterNoob()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            transform.position += Time.deltaTime * moveSpeed * transform.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            transform.position += Time.deltaTime * moveSpeed * -transform.forward;
        if (Input.GetKey(KeyCode.A))
            transform.position += Time.deltaTime * moveSpeed * -transform.right;
        if (Input.GetKey(KeyCode.D))
            transform.position += Time.deltaTime * moveSpeed * transform.right;
    }

    private void RotateCharacter()
    {
        Vector2 lookDirection = lookAction.ReadValue<Vector2>();
        transform.eulerAngles += rotateSpeed * new Vector3(0, lookDirection.x, 0);

        float vertical = lookDirection.y;
        float xAxis = head.localEulerAngles.x;
        if (xAxis > 180) xAxis -= 360;
        xAxis -= vertical * rotateSpeed;

        if (xAxis >= 60) xAxis = 60;
        if (xAxis <= -60) xAxis = -60;

        head.localEulerAngles = new Vector3(xAxis, 0, 0);
    }

    private void RotateCharacterNoob(bool isInUpdate)
    {
        if (isInUpdate)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                transform.Rotate(0, -45, 0);
            if (Input.GetKeyDown(KeyCode.E))
                transform.Rotate(0, 45, 0);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                transform.Rotate(0, -rotateSpeed, 0); 
            if (Input.GetKey(KeyCode.RightArrow))
                transform.Rotate(0, rotateSpeed, 0);
        }


    }
    
    private void Interact()
    {
        float isClicked = interAction.ReadValue<float>();
        if (isClicked == 0) return;

        Physics.Raycast(povCamera.transform.position, transform.forward, out RaycastHit hit, 3f);
        if (hit.transform == null) return;

        Interactable interactable = hit.transform.GetComponentInParent<Interactable>();
        if (interactable == null) return;

        interactable.Interact(gameObject);
    }

    private void InteractNoob()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.nearClipPlane;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            Physics.Raycast(povCamera.transform.position, mouseWorldPosition - povCamera.transform.position, out RaycastHit hit, 5f);
            if (hit.transform == null) return;

            Interactable interactable = hit.transform.GetComponentInParent<Interactable>();
            if (interactable == null) return;

            interactable.Interact(gameObject);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        bool grounded = IsGrounded();
        if (grounded)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpStrength;
            rb.linearVelocity = vel;
        }
    }

    Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
            // Recursively check children of this child
            Transform result = FindChildWithTag(child, tag);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    private bool IsGrounded()
    {
        Vector3 spherePosition = transform.position + Vector3.down * (groundOffset + 0.1f); // Slightly below the character
        float sphereRadius = cc.radius * 0.9f; // Slightly smaller than the collider radius

        bool grounded = Physics.CheckSphere(spherePosition, sphereRadius, groundLayer);
        Debug.DrawRay(spherePosition, Vector3.down * 0.1f, grounded ? Color.green : Color.red, 1f);
        return grounded;
    }

    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log("TestServerRpc " + OwnerClientId);
    }

    public void SetMovementEnabled(bool active)
    {
        IsMovementEnabled = active;
    }

}
