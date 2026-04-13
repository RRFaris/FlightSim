using UnityEngine;

public class Engine : MonoBehaviour
{
    public Rigidbody main;
    public float engineForce;

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
    }

    public void ApplyThrust(float thrust)
    {
        // main.AddForce(transform.forward * (engineForce * throttle), ForceMode.Force);
        // Adds force specifically where the engine is placed in the aircraft
        main.AddForceAtPosition(transform.forward * (engineForce * thrust), transform.position);
    }
}
