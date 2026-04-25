using UnityEngine;

public class Airplane : MonoBehaviour
{
    public Rigidbody main;
    public Vector3 centerMass;
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main.centerOfMass = centerMass;
        airDensity = CalculateAirDensity();
        leftWheel.brakeTorque  = 0;
        rightWheel.brakeTorque = 0;
        noseWheel.brakeTorque  = 0;
        
        // SetWheelForwardFriction(leftWheel,  0.3f);
        // SetWheelForwardFriction(rightWheel, 0.3f);
        // SetWheelForwardFriction(noseWheel,  0.3f);
    }

    // void SetWheelForwardFriction(WheelCollider wheel, float stiffness)
    // {
    //     WheelFrictionCurve fwd = wheel.forwardFriction;
    //     fwd.stiffness = stiffness;
    //     wheel.forwardFriction = fwd;
    // }

    
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        CalculateState();
        engine.ApplyThrust(throttle);
        
        float tinyTorque = 0.0001f;
        leftWheel.motorTorque = tinyTorque;
        rightWheel.motorTorque = tinyTorque;
        noseWheel.motorTorque = tinyTorque;
        // noseWheel.brakeTorque = 0;
        // rightWheel.brakeTorque = 0;
        // leftWheel.brakeTorque = 0;
        
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
