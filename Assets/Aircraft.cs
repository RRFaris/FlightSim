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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main.centerOfMass = centerMass;
        airDensity = CalculateAirDensity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        engine.ApplyThrust(throttle);
        CalculateState();
        
        foreach (Airfoil airfoil in airfoils)
        {
            airfoil.applyLift(airDensity, localVelocity);
        }
    }

    public void CalculateState()
    {
        // Calculates plane's local velocity 
        localVelocity = main.transform.InverseTransformDirection(main.linearVelocity);
    }
    
    public float CalculateAirDensity()
    {
        float airDensity = pressure / (specificGasConstant * temperature);
        return airDensity;
    }
}
