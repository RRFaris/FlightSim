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

    public Airfoil[] airfoils;

    public float temperature = 288.15f;         // In Kelvin (59 degrees Fahrenheit)
    public float specificGasConstant = 287.05f; // In J/(kg*K)
    public int pressure = 101325;               // In pascals
    
    public float airDensity;
    public Vector3 localVelocity;
    public Vector3 velocity;
    
    public Vector3 aircraftBodyArea; // cross-sectional area in X, Y, Z directions
    public Vector3 cbd;              // drag coefficient for each axis
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        CalculateState();
        engine.ApplyThrust(throttle);
        ApplyBodyDrag();
        
        float tinyTorque = 0.0001f;
        leftWheel.motorTorque = tinyTorque;
        rightWheel.motorTorque = tinyTorque;
        noseWheel.motorTorque = tinyTorque;
        
        
        foreach (Airfoil airfoil in airfoils)
        {
            Vector3 wingWorldVelocity = main.GetPointVelocity(airfoil.transform.position);
            Vector3 wingLocalVelocity = airfoil.transform.InverseTransformDirection(wingWorldVelocity);
            // airfoil.applyLift(main, airDensity, main.linearVelocity.normalized);
            airfoil.applyLift(main, airDensity, wingLocalVelocity);
        }
    }

    public void CalculateState()
    {
        // Calculates plane's local velocity 
        // Calculates the angle of attack
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
