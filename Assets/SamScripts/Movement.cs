using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;
    public float tiltAngle = 15f;
    public float tiltSmooth = 5f;

    [Header("Jump Settings")]
    public float jumpHeight = 8f;
    public int maxJumps = 2;
    public float gravity = -20f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public bool dashUnlocked = false; // unlocked via power-up

    [Header("Audio Settings")]
    public AudioClip jumpSound;
    public AudioClip dashSound;

    private CharacterController controller;
    private AudioSource audioSource;

    private Vector3 velocity;
    private int jumpCount = 0;
    private bool isFacingRight = true;

    private float targetYRotation = 0f;
    private float currentYRotation = 0f;
    private float targetTilt = 0f;
    private float currentTilt = 0f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        HandleDash();
        HandleMovement();
        HandleJump();
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        if (isDashing) return; // Skip normal movement while dashing

        float inputX = Input.GetAxisRaw("Horizontal");
        Vector3 move = new Vector3(inputX, 0f, 0f).normalized;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Determine facing direction
        if (inputX != 0)
        {
            bool movingRight = inputX > 0;
            if (movingRight != isFacingRight)
            {
                isFacingRight = movingRight;
                targetYRotation = isFacingRight ? 0f : 180f;
            }
        }

        // Smoothly rotate to face direction
        currentYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, rotationSpeed * Time.deltaTime);

        // Determine target tilt
        if (Mathf.Abs(inputX) > 0.1f)
        {
            targetTilt = (isFacingRight ? -1f : 1f) * tiltAngle * Mathf.Sign(inputX);
        }
        else
        {
            targetTilt = 0f;
        }

        // Smooth tilt
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSmooth * Time.deltaTime);

        // Combine facing rotation (Y) and tilt (Z)
        Quaternion faceRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        Quaternion tiltRotation = Quaternion.Euler(0f, 0f, currentTilt);
        transform.rotation = faceRotation * tiltRotation;
    }

    void HandleJump()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            velocity.y = -1f;
        }

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps && !isDashing)
        {
            velocity.y = jumpHeight;
            jumpCount++;

            // Play jump sound
            if (jumpSound != null)
                audioSource.PlayOneShot(jumpSound);
        }
    }

    void ApplyGravity()
    {
        if (!isDashing)
            velocity.y += gravity * Time.deltaTime;
    }

    void HandleDash()
    {
        // Cooldown timer
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // Start dash
        if (dashUnlocked && !isDashing && dashCooldownTimer <= 0 && Input.GetButtonDown("Dash"))
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            if (dashSound != null)
                audioSource.PlayOneShot(dashSound);

            // Set dash velocity in facing direction
            float dashDir = isFacingRight ? 1f : -1f;
            velocity = new Vector3(dashDir * dashSpeed, 0f, 0f);
        }

        // Continue dash
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                velocity = Vector3.zero;
            }
        }
    }

    public void UnlockDash()
    {
        dashUnlocked = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DashPowerup"))
        {
            UnlockDash();
            Destroy(other.gameObject);
        }
    }
}
