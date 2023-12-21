using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stats;
    [SerializeField] CinemachineVirtualCamera thirdPersonCam;
    [SerializeField] CinemachineVirtualCamera lookBackCam;

    Rigidbody rb;
    AudioSource engineSound;
    Propeller propeller;

    public float throttleIncrement = 0.1f;
    public float maxThrust = 200f;
    public float sensitivity = 200f;
    public float lift = 135f;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;
    private float minVolume = 0f;
    private float maxVolume = 0.2f;

    // Sensitivity modifier property for fine-tuning sensitivity based on mass
    private float sensitivityModifier
    {
        get
        {
            return (rb.mass / 10f) * sensitivity;
        }
    }

    private void OnEnable()
    {
        CameraManager.Register(thirdPersonCam);
        CameraManager.Register(lookBackCam);
        CameraManager.SwitchCamera(thirdPersonCam);
    }

    private void OnDisable()
    {
        CameraManager.Unregister(thirdPersonCam);
        CameraManager.Unregister(lookBackCam);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
        propeller = FindObjectOfType<Propeller>();
    }

    private void HandleInputs()
    {
        // Get input for roll, pitch, and yaw
        roll = Input.GetAxis("Horizontal");
        pitch = Input.GetAxis("Vertical");
        yaw = Input.GetAxis("Yaw");

        // Adjust throttle based on key inputs
        if (Input.GetKey(KeyCode.Space))
        {
            IncreaseThrottle();
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            DecreaseThrottle();
        }

        // Adjust engine sound volume based on throttle, clamp volume between minVolume and maxVolume
        engineSound.volume = Mathf.Clamp(throttle * 0.01f, minVolume, maxVolume);

        // Clamp throttle value between 0 and 100
        throttle = Mathf.Clamp(throttle, 0f, 100f);

        // Set propeller speed based on throttle
        propeller.speed = throttle * 20f;
    }

    private void Update()
    {
        HandleInputs();
        UpdateHUD();

        // Reload scene when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }

        // Switch cameras when C is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (CameraManager.IsActiveCamera(thirdPersonCam))
            {
                CameraManager.SwitchCamera(lookBackCam);
            }
            else if (CameraManager.IsActiveCamera(lookBackCam))
            {
                CameraManager.SwitchCamera(thirdPersonCam);
            }
        }
    }

    void IncreaseThrottle()
    {
        throttle += throttleIncrement;
    }

    void DecreaseThrottle()
    {
        throttle -= throttleIncrement;
    }

    void Thrust()
    {
        // Apply thrust force to the plane
        rb.AddForce(transform.forward * maxThrust * throttle);
    }

    void Pitch()
    {
        // Apply rotational force on the x-axis to pitch the plane up and down
        rb.AddTorque(transform.right * pitch * sensitivityModifier);
    }

    void Roll()
    {
        // Apply rotational force on the z-axis to roll the plane left and right
        rb.AddTorque(-transform.forward * roll * sensitivityModifier);
    }

    void Yaw()
    {
        // Apply rotational force on the y-axis to rotate the plane left and right
        rb.AddTorque(transform.up * yaw * sensitivityModifier);
    }

    void Lift()
    {
        // Apply upward force to lift the plane upwards
        rb.AddForce(Vector3.up * rb.velocity.magnitude * lift);
    }

    private void FixedUpdate()
    {
        Thrust();
        Pitch();
        Roll();
        Yaw();
        Lift();
    }

    private void UpdateHUD()
    {
        stats.text = "Throttle: " + throttle.ToString("F0") + "%\n"
            + "Airspeed: " + (rb.velocity.magnitude * 3.6f).ToString("F0") + "km/h\n"
            + "Altitude: " + transform.position.y.ToString("F0") + "m";
    }
}
