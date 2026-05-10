using UnityEngine;

public class Engine : MonoBehaviour
{
    public Rigidbody main;
    public float engineForce;

    [Header("Propeller")]
    public Transform propeller;
    public float maxRPM = 2400f;        // Cessna 172 max RPM
    public float smoothSpeed = 2f;      // how fast RPM ramps up/down

    private float currentRPM = 0f;
    private float currentThrottle = 0f;

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
    }

    void Update()
    {
        if (propeller == null) return;

        // Smoothly ramp RPM toward target
        float targetRPM = currentThrottle * maxRPM;
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, Time.deltaTime * smoothSpeed);

        // Convert RPM to degrees per second and rotate
        float degreesPerSecond = currentRPM / 60f * 360f;
        propeller.Rotate(Vector3.forward, degreesPerSecond * Time.deltaTime);
    }

    public void ApplyThrust(float thrust)
    {
        currentThrottle = thrust;
        main.AddForce(transform.forward * (engineForce * thrust), ForceMode.Force);
    }
}