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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main.centerOfMass = centerMass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ApplyForces();
    }

    public void ApplyForces()
    {
        engine.throttle = throttle;
    }
    
}
