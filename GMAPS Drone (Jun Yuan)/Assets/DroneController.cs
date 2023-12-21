using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float liftForce = 300;  // Force to lift the drone
    public float movementForce = 50;  // Force to move the drone forward, backward, left, and right
    public float rotationTorque = 25;  // Torque for turning the drone
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplyLift();
        ApplyMovement();
        ApplyRotation();
    }

    void ApplyLift()
    {
        // Lift - Adjusting for Earth's gravity
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * (liftForce - Physics.gravity.y * rb.mass));
        }
    }

    void ApplyMovement()
    {
        // Movement - Adjusting for realistic movement on Earth
        Vector3 moveDirection = Vector3.zero;
        moveDirection += transform.forward * Input.GetAxis("Vertical");
        moveDirection += transform.right * Input.GetAxis("Horizontal");
        rb.AddForce(moveDirection.normalized * movementForce);
    }

    void ApplyRotation()
    {
        // Rotation - Yaw control using the same keys as horizontal movement
        float turn = Input.GetAxis("Horizontal"); // Using Horizontal axis for yaw control
        rb.AddTorque(Vector3.up * turn * rotationTorque);
    }
}
