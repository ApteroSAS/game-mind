using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour
{
    private InputSystem_Actions inputAction;

    private InputAction moveAction;
    private InputAction lookAction;

    private Rigidbody rb;
    private CapsuleCollider cc;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField][Range(0.05f, 1f)] private float rotateSpeed = 0.5f;
    [SerializeField][Range(4, 6)] private float jumpSrength = 5;
    
    private Transform head;
    private float groundOffset;
    private Vector3 groundPosition;

    private void Awake()
    {
        inputAction = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        groundOffset = cc.height / 2 * transform.localScale.y;
        groundPosition = transform.position;
        groundPosition.y -= groundOffset / 2;
        head = FindChildWithTag(transform, "PlayerHead");
    }

    private void OnEnable()
    {
        if (inputAction == null) Awake();

        moveAction = inputAction.Player.Move;
        moveAction.Enable();

        lookAction = inputAction.Player.Look;
        lookAction.Enable();

        inputAction.Player.Jump.performed += OnJump;
        inputAction.Player.Jump.Enable();

        Debug.Log("Controls enabled");
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();

        inputAction.Player.Jump.performed -= OnJump;
        inputAction.Player.Jump.Disable();

        Debug.Log("Controls disabled");
    }

    private void FixedUpdate()
    {
        RotateCharacter();
        MoveCharacter();
    }   

    private void MoveCharacter()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();

        Vector3 moveRight = moveDirection.x * moveSpeed * transform.right;
        Vector3 moveForward = moveDirection.y * moveSpeed * transform.forward;
        Vector3 vel = moveRight + moveForward + new Vector3(0, rb.linearVelocity.y, 0);
        rb.linearVelocity = vel;

        //transform.position += Time.fixedDeltaTime * moveSpeed * (transform.right * moveDirection.x);
        //transform.position += Time.fixedDeltaTime * moveSpeed * (transform.forward * moveDirection.y);
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

    private void OnJump(InputAction.CallbackContext context)
    {
        float sphereRadius = cc.radius;
        float checkDistance = 0.2f;
        bool grounded = Physics.SphereCast(groundPosition, sphereRadius, Vector3.down, out _, checkDistance);
        Debug.Log("Grounded: " + grounded);
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

}
