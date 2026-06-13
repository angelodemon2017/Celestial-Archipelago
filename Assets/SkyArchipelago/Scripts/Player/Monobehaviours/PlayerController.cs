using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody _rb;
    private Vector2 _currentMoveInput;
    private bool _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // ====================== IInputRequester ======================

    public void ProcessMovement(Vector2 moveInput)
    {
        _currentMoveInput = moveInput;
    }

    public void ProcessJump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = transform.right * _currentMoveInput.x + transform.forward * _currentMoveInput.y;
        Vector3 targetVelocity = moveDirection.normalized * moveSpeed;
        targetVelocity.y = _rb.linearVelocity.y;

        _rb.linearVelocity = targetVelocity;
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void OnDestroy()
    {

    }
}