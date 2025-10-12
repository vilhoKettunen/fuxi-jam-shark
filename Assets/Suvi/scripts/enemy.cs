using UnityEngine;

public class enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Distance the object moves left and right from its starting position.")]
    public float moveDistance = 5f;

    [Tooltip("Speed of the left-right movement.")]
    public float moveSpeed = 2f;

    // Private variables
    private Vector3 startPos;
    private bool movingRight = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float movement = moveSpeed * Time.deltaTime;

        if (movingRight)
        {
            transform.Translate(Vector3.right * movement);
            if (transform.position.x >= startPos.x + moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * movement);
            if (transform.position.x <= startPos.x - moveDistance)
            {
                movingRight = true;
            }
        }
    }
}
