using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum PlayerAttribute
{
    Fire,
    Ice,
}

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
    [SerializeField][Range(4, 6)] private float jumpSrength = 5;
    [SerializeField] private LayerMask groundLayer;

    private Transform head;
    private Transform povCamera;
    private float groundOffset;

    // Network variable to sync attribute across network
    public NetworkVariable<PlayerAttribute> NetworkAttribute = new NetworkVariable<PlayerAttribute>();


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        base.OnNetworkSpawn();

        inputAction = new InputSystem_Actions();
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
        moveAction.Disable();
        lookAction.Disable();
        interAction.Disable();

        //inputAction.Player.Interact.performed -= OnInteract;
        //inputAction.Player.Interact.Disable();

        inputAction.Player.Jump.performed -= OnJump;
        inputAction.Player.Jump.Disable();

        Debug.Log("Controls disabled");
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
            

        RotateCharacter();
        MoveCharacter();
        Interact();
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        bool grounded = IsGrounded();
        if (grounded)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpSrength;
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

    [ServerRpc (RequireOwnership =false)]
    public void AssignAttributeServerRpc()
    {
        if (!IsServer) return;

        var playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        if (playerCount % 2 == 0)
        {
            NetworkAttribute.Value = PlayerAttribute.Ice;
        }
        else
        {
            NetworkAttribute.Value = PlayerAttribute.Fire;
        }

        Debug.Log($"Player {OwnerClientId} assigned attribute: {NetworkAttribute.Value}");
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

}
