using UnityEngine;

public class winscreenCAM : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;
    public float moveAmount = 0.5f;

    [Header("Idle Rotation Settings")]
    public float rotationSpeed = 10f;
    public float rotationAmount = 5f;

    [Header("Toggle Rotation Settings")]
    public float toggleRotationSpeed = 2f; // Speed of 90° toggle rotation

    private Vector3 startPos;
    private Quaternion startRotation;

    private float targetYAngle = 0f;    // 0 or 90
    private float currentYAngle = 0f;   // Smooth interpolation

    private bool isToggled = false;

    void Start()
    {
        startPos = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        HandleToggleInput();
        ApplyIdleMovement();
        ApplyCombinedRotation();
    }

    void HandleToggleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            isToggled = !isToggled;
            targetYAngle = isToggled ? 90f : 0f;
        }
    }

    void ApplyIdleMovement()
    {
        float x = Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        float y = Mathf.Sin(Time.time * moveSpeed * 0.5f) * moveAmount * 0.5f;
        float z = Mathf.Cos(Time.time * moveSpeed) * moveAmount;

        transform.position = startPos + new Vector3(x, y, z) * 0.1f;
    }

    void ApplyCombinedRotation()
    {
        // Smoothly interpolate Y rotation toward target
        currentYAngle = Mathf.Lerp(currentYAngle, targetYAngle, Time.deltaTime * toggleRotationSpeed);

        // Base rotation with toggle Y angle
        Quaternion baseRotation = Quaternion.Euler(0f, currentYAngle, 0f);

        // Idle rotation sway
        float idleX = Mathf.Sin(Time.time * rotationSpeed * 0.1f) * rotationAmount;
        float idleY = Mathf.Cos(Time.time * rotationSpeed * 0.1f) * rotationAmount;
        Quaternion idleRotation = Quaternion.Euler(idleX, idleY, 0f);

        // Combine toggle rotation and idle sway
        transform.rotation = startRotation * baseRotation * idleRotation;
    }
}
