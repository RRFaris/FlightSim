using UnityEngine;

public class Airplane : MonoBehaviour
{
    public Rigidbody main;
    public Transform centerMassTransform;
    public Engine engine;

    [Range(0, 1)] public float throttle;

    [Header("Landing Gear")]
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public WheelCollider noseWheel;
    public float rollingResistance;
    public bool brakesOn = false; // ADD THIS
    public bool flapsDeployed = false; // Added by the oliver

    public Airfoil[] airfoils;

    public float temperature = 288.15f;
    public float specificGasConstant = 287.05f;
    public int pressure = 101325;

    public float airDensity;
    public Vector3 localVelocity;
    public Vector3 velocity;

    public Vector3 aircraftBodyArea;
    public Vector3 cbd;

    void Start()
    {
        main.centerOfMass = main.transform.InverseTransformPoint(centerMassTransform.position);
        airDensity = CalculateAirDensity();
        leftWheel.brakeTorque  = 0;
        rightWheel.brakeTorque = 0;
        noseWheel.brakeTorque  = 0;
    }

    void ApplyBodyDrag()
    {
        float xDrag = 0;
        float yDrag = 0;
        float zDrag = 0;

        if (localVelocity.x != 0)
            xDrag = cbd.x * (0.5f * airDensity * (localVelocity.x * localVelocity.x) * aircraftBodyArea.x * (-localVelocity.x / Mathf.Abs(localVelocity.x)));
        if (localVelocity.y != 0)
            yDrag = cbd.y * (0.5f * airDensity * (localVelocity.y * localVelocity.y) * aircraftBodyArea.y * (-localVelocity.y / Mathf.Abs(localVelocity.y)));
        if (localVelocity.z != 0)
            zDrag = cbd.z * (0.5f * airDensity * (localVelocity.z * localVelocity.z) * aircraftBodyArea.z * (-localVelocity.z / Mathf.Abs(localVelocity.z)));

        Vector3 drag = new Vector3(xDrag, yDrag, zDrag);
        main.AddForce(transform.TransformVector(drag));
    }

    // ADD THIS METHOD
    void ApplyRollingResistance()
    {
        float brakeTorque = brakesOn ? 5000f : rollingResistance;
        leftWheel.brakeTorque  = brakeTorque;
        rightWheel.brakeTorque = brakeTorque;
        noseWheel.brakeTorque  = brakeTorque;
    }

    void Update() { }

    void FixedUpdate()
    {
        CalculateState();
        engine.ApplyThrust(throttle);
        ApplyBodyDrag();
        ApplyRollingResistance(); // ADD THIS

        float tinyTorque = 0.0001f;
        leftWheel.motorTorque = tinyTorque;
        rightWheel.motorTorque = tinyTorque;
        noseWheel.motorTorque = tinyTorque;

        foreach (Airfoil airfoil in airfoils)
        {
            Vector3 wingWorldVelocity = main.GetPointVelocity(airfoil.transform.position);
            Vector3 wingLocalVelocity = airfoil.transform.InverseTransformDirection(wingWorldVelocity);
            airfoil.applyLift(main, airDensity, wingLocalVelocity);
        }
    }

    public void CalculateState()
    {
        var invRotation = Quaternion.Inverse(main.rotation);
        velocity = main.linearVelocity;
        localVelocity = invRotation * velocity;
        airDensity = CalculateAirDensity();
    }

    public float CalculateAirDensity()
    {
        float airDensity = pressure / (specificGasConstant * temperature);
        return airDensity;
    }
}