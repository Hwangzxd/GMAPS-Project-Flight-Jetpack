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

    [SerializeField] AnimationCurve liftAOACurve;

    public float throttleIncrement = 0.1f;
    public float maxThrust = 200f;
    public float sensitivity = 200f;
    public float lift = 135f;

    private float minVolume = 0f;
    private float maxVolume = 0.2f;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    public Vector3 Velocity { get; private set; }
    public Vector3 LocalVelocity { get; private set; }
    public float AngleOfAttack { get; private set; }

    private float sensitivityModifier
    {
        get
        {
            return (rb.mass / 10f) * sensitivity;
        }
    }

    Rigidbody rb;
    AudioSource engineSound;
    Propeller propeller;

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
        roll = Input.GetAxis("Horizontal");
        pitch = Input.GetAxis("Vertical");
        yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.Space))
        {
            IncreaseThrottle();
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            DecreaseThrottle();
        }

        engineSound.volume = Mathf.Clamp(throttle * 0.01f, minVolume, maxVolume);

        throttle = Mathf.Clamp(throttle, 0f, 100f);

        propeller.speed = throttle * 20f;
    }

    private void Update()
    {
        HandleInputs();
        UpdateHUD();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }

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

    void Thrust()
    {
        // Apply forward force to the plane
        rb.AddForce(transform.forward * maxThrust * throttle);
    }

    void Pitch()
    {
        // Apply rotational force on the x-axis to turn the plane up and down
        rb.AddTorque(transform.right * pitch * sensitivityModifier);
    }

    void Roll()
    {
        // Apply rotational force on the z-axis to roll the plane left and right
        rb.AddTorque(-transform.forward * roll * sensitivityModifier);
    }

    void Yaw()
    {
        // Apply rotational force on the y-axis to turn the plane left and right
        rb.AddTorque(transform.up * yaw * sensitivityModifier);
    }

    void Lift()
    {
        //// Apply upward force to lift the plane upwards
        //rb.AddForce(Vector3.up * rb.velocity.magnitude * lift);

        //// Evaluate the lift curve based on the angle of attack
        //float angleOfAttack = Vector3.Angle(Vector3.up, transform.forward);
        //float liftForce = liftAOACurve.Evaluate(angleOfAttack) * lift;

        //// Apply upward force to lift the plane upwards
        //rb.AddForce(Vector3.up * liftForce);

        if (LocalVelocity.sqrMagnitude < 1f) return;

        var liftForce = CalculateLift(
            AngleOfAttack, Vector3.right,
            lift,
            liftAOACurve
        );

        rb.AddForce(liftForce);
    }

    void CalculateAngleOfAttack()
    {
        if (LocalVelocity.sqrMagnitude < 0.1f)
        {
            AngleOfAttack = 0;
            return;
        }

        AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z);
    }

    void CalculateState(float dt)
    {
        var invRotation = Quaternion.Inverse(rb.rotation);
        Velocity = rb.velocity;
        LocalVelocity = invRotation * Velocity;  //transform world velocity into local space

        CalculateAngleOfAttack();
    }

    Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve)
    {
        var liftVelocity = Vector3.ProjectOnPlane(LocalVelocity, rightAxis);    //project velocity onto YZ plane
        var v2 = liftVelocity.sqrMagnitude;                                     //square of velocity

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        var liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        var liftForce = v2 * liftCoefficient * liftPower;

        //lift is perpendicular to velocity
        var liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        var lift = liftDirection * liftForce;

        return lift;
    }

    private void FixedUpdate()
    {
        //// Apply forward force to the plane
        //rb.AddForce(transform.forward * maxThrust * throttle);
        //// Apply rotational force on the x-axis to turn the plane up and down
        //rb.AddTorque(transform.right * pitch * sensitivityModifier);
        //// Apply rotational force on the z-axis to roll the plane left and right
        //rb.AddTorque(-transform.forward * roll * sensitivityModifier);
        //// Apply rotational force on the y-axis to turn the plane left and right
        //rb.AddTorque(transform.up * yaw * sensitivityModifier);
        //// Apply upward force to lift the plane upwards
        //rb.AddForce(Vector3.up * rb.velocity.magnitude * lift);

        float dt = Time.fixedDeltaTime;

        //calculate at start, to capture any changes that happened externally
        CalculateState(dt);

        Thrust();
        Pitch();
        Roll();
        Yaw();
        Lift();

        //calculate again, so that other systems can read this plane's state
        CalculateState(dt);
    }

    void IncreaseThrottle()
    {
        throttle += throttleIncrement;
    }

    void DecreaseThrottle()
    {
        throttle -= throttleIncrement;
    }

    private void UpdateHUD()
    {
        stats.text = "Throttle: " + throttle.ToString("F0") + "%\n"
            + "Airspeed: " + (rb.velocity.magnitude * 3.6f).ToString("F0") + "km/h\n"
            + "Altitude: " + transform.position.y.ToString("F0") + "m";
    }
}
