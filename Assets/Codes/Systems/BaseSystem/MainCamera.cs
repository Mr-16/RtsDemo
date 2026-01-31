using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float scrollSpeed = 20f;
    public float smoothTime = 0.15f;

    public float minHeight = 5f;
    public float maxHeight = 80f;

    private Vector3 targetPos;
    private Vector3 velocity; // SmoothDamp ÄÚ²¿ÓÃ

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        HandleInput();
        SmoothMove();
    }

    void HandleInput()
    {
        Vector2 move = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) move.y += 1;
        if (Keyboard.current.sKey.isPressed) move.y -= 1;
        if (Keyboard.current.aKey.isPressed) move.x -= 1;
        if (Keyboard.current.dKey.isPressed) move.x += 1;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;

        Vector3 dir = forward * move.y + right * move.x;
        targetPos += dir * moveSpeed * Time.deltaTime;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetPos.y -= scroll * scrollSpeed * Time.deltaTime;
            targetPos.y = Mathf.Clamp(targetPos.y, minHeight, maxHeight);
        }
    }

    void SmoothMove()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
