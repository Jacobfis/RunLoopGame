using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Jump")]
    public float jumpHeight = 1.5f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    private Vector3 velocity;

    void Update()
    {
        if (controller == null)
            return;

        bool isGrounded = controller.isGrounded;

        // Holder spilleren på jorden
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Hop
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            isGrounded)
        {
            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        // WASD
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;

            if (Keyboard.current.aKey.isPressed)
                input.x -= 1f;
        }

        // Forhindrer hurtigere diagonal movement
        input = Vector2.ClampMagnitude(input, 1f);

        // =====================================
        // SPRINT
        // =====================================

        bool isSprinting =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed;

        float currentSpeed =
            isSprinting ? sprintSpeed : walkSpeed;

        // =====================================

        Vector3 move =
            transform.right * input.x +
            transform.forward * input.y;

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMovement =
            move * currentSpeed +
            Vector3.up * velocity.y;

        controller.Move(
            finalMovement * Time.deltaTime
        );
    }

    public void ResetMovement()
    {
        velocity = Vector3.zero;
    }
}