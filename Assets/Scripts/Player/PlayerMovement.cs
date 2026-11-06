using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;
    private Vector2 dashDirection;
    private float dashTimeLeft;
    private float nextDashTime;
    public Vector2 FacingDirection => lastDirection;
    public bool IsInvincible => dashTimeLeft > 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1;

        moveInput = moveInput.normalized;

        if (dashTimeLeft > 0f)
            return;

        if (moveInput != Vector2.zero)
            lastDirection = moveInput;

        if (Keyboard.current.spaceKey.wasPressedThisFrame
            && Time.time >= nextDashTime)
        {
            dashDirection = lastDirection;
            dashTimeLeft = dashDuration;
            nextDashTime = Time.time + dashCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (dashTimeLeft > 0f)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            dashTimeLeft -= Time.fixedDeltaTime;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }
}