using UnityEngine;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera")]
        [Tooltip("Assign your player camera here (usually the main camera).")]
        public Camera playerCamera;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float mouseSensitivity = 2f;
        public float gravity = -9.81f;

        private CharacterController controller;
        private Vector3 velocity;
        private float xRotation = 0f;

        void Start()
        {
            controller = GetComponent<CharacterController>();

            // If no camera is assigned, automatically find the main one
            if (playerCamera == null)
                playerCamera = Camera.main;

            // Lock the cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            MovePlayer();
            RotateCamera();
        }

        void MovePlayer()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            controller.Move(move * moveSpeed * Time.deltaTime);

            // Apply gravity
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        void RotateCamera()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Rotate camera vertically
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // Rotate player horizontally
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}
