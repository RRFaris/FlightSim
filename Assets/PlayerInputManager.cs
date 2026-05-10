using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public Airplane airplane;
    public bool isActive = true;

    [Header("Keyboard Throttle Settings")]
    public KeyCode throttleDown = KeyCode.S;
    public KeyCode throttleUp   = KeyCode.W;

    [Header("Keyboard Control Surfaces")]
    public KeyCode pitchUpKey   = KeyCode.DownArrow;
    public KeyCode pitchDownKey = KeyCode.UpArrow;
    public KeyCode rollLeftKey  = KeyCode.A;
    public KeyCode rollRightKey = KeyCode.D;
    public KeyCode yawLeftKey   = KeyCode.Q;
    public KeyCode yawRightKey  = KeyCode.E;

    [Header("Other")]
    public KeyCode brakeToggle = KeyCode.B;
    public KeyCode flapsKey    = KeyCode.F;

    private float throttle = 0f;
    private float flaps    = 0f;
    private bool brakesOn  = false;

    // Tracks whether a HOTAS is connected
    private bool hotasConnected = false;

    void Start()
    {
        hotasConnected = Input.GetJoystickNames().Length > 0 
                         && Input.GetJoystickNames()[0] != "";
        if (hotasConnected)
            Debug.Log("HOTAS detected: " + Input.GetJoystickNames()[0]);
        else
            Debug.Log("No HOTAS detected, using keyboard.");
    }

    void Update()
    {
        if (!isActive) return;

        float pitchInput = 0f;
        float rollInput  = 0f;
        float yawInput   = 0f;

        if (hotasConnected)
        {
            // --- HOTAS Input ---
            pitchInput = -Input.GetAxis("Joystick Axis 2"); // stick forward/back
            rollInput  = -Input.GetAxis("Joystick Axis 1"); // stick left/right
            yawInput   = -Input.GetAxis("Joystick Axis 3"); // twist rudder

            // Throttle — remap from -1/1 to 0/1
            float rawThrottle = Input.GetAxis("Joystick Axis 4");
            throttle = 1f - ((rawThrottle + 1f) / 2f);

            // Brake button — button 0 is trigger on T-Flight HOTAS X
            if (Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                brakesOn = !brakesOn;
                airplane.leftWheel.brakeTorque  = brakesOn ? 5000f : airplane.rollingResistance;
                airplane.rightWheel.brakeTorque = brakesOn ? 5000f : airplane.rollingResistance;
                airplane.noseWheel.brakeTorque  = brakesOn ? 5000f : airplane.rollingResistance;
            }

            // Flaps button — button 1 on T-Flight HOTAS X
            float flapTarget = Input.GetKey(KeyCode.JoystickButton1) ? 1f : 0f;
            flaps = Mathf.Lerp(flaps, flapTarget, Time.deltaTime / 2f);
        }
        else
        {
            Debug.Log("Pitch up pressed: " + Input.GetKey(pitchUpKey));
            Debug.Log("Roll right pressed: " + Input.GetKey(rollRightKey));
            // --- Keyboard Fallback ---
            if (Input.GetKey(throttleUp))
                throttle += Time.deltaTime * 0.3f;
            if (Input.GetKey(throttleDown))
                throttle -= Time.deltaTime * 0.3f;

            if (Input.GetKey(pitchUpKey))   pitchInput =  1f;
            if (Input.GetKey(pitchDownKey)) pitchInput = -1f;
            if (Input.GetKey(rollRightKey)) rollInput  =  1f;
            if (Input.GetKey(rollLeftKey))  rollInput  = -1f;
            if (Input.GetKey(yawRightKey))  yawInput   =  1f;
            if (Input.GetKey(yawLeftKey))   yawInput   = -1f;

            if (Input.GetKeyDown(brakeToggle))
            {
                brakesOn = !brakesOn;
                airplane.leftWheel.brakeTorque  = brakesOn ? 5000f : airplane.rollingResistance;
                airplane.rightWheel.brakeTorque = brakesOn ? 5000f : airplane.rollingResistance;
                airplane.noseWheel.brakeTorque  = brakesOn ? 5000f : airplane.rollingResistance;
            }

            float flapTarget = Input.GetKey(flapsKey) ? 1f : 0f;
            flaps = Mathf.Lerp(flaps, flapTarget, Time.deltaTime / 2f);
        }

        throttle = Mathf.Clamp01(throttle);
        airplane.throttle = throttle;

        // --- Send to Airfoils (same for both input methods) ---
        foreach (Airfoil airfoil in airplane.airfoils)
        {
            if (airfoil.airfoilType == AirfoilType.aileron)
            {
                float side = airfoil.offset > 0 ? 1f : -1f;
                airfoil.controlInput = rollInput * side;
                airfoil.flapInput    = flaps;
            }
            if (airfoil.airfoilType == AirfoilType.elevator)
                airfoil.controlInput = pitchInput;
            if (airfoil.airfoilType == AirfoilType.rudder)
                airfoil.controlInput = yawInput;
        }
    }
}