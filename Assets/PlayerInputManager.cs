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
    public KeyCode brakeToggle         = KeyCode.B;
    public KeyCode flapsKey            = KeyCode.F;
    public KeyCode flapsJoystickButton = KeyCode.JoystickButton4;

    private float throttle      = 0f;
    private float flaps         = 0f;
    private float flapTarget    = 0f;
    private bool flapsDeployed  = false;
    private bool brakesOn       = false;
    private bool hotasConnected = false;
    private bool firstFrame     = true;

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

        // --- HOTAS Input (runs when connected) ---
        if (hotasConnected)
        {
            pitchInput = -Input.GetAxis("Joystick Axis 2");
            rollInput  = -Input.GetAxis("Joystick Axis 1");
            yawInput   = -Input.GetAxis("Joystick Axis 3");

            float rawThrottle = Input.GetAxis("Joystick Axis 4");
            if (Mathf.Abs(rawThrottle) > 0.01f)
                throttle = (-rawThrottle + 1f) / 2f;
            
            // float rawThrottle = Input.GetAxis("Joystick Axis 4");
            // throttle = (-rawThrottle + 1f) / 2f;
        }

        // --- Keyboard (ALWAYS runs, works alongside HOTAS) ---
        if (Input.GetKey(throttleUp))
            throttle += Time.deltaTime * 0.3f;
        if (Input.GetKey(throttleDown))
            throttle -= Time.deltaTime * 0.3f;

        if (Input.GetKey(pitchUpKey))   pitchInput = -1f;
        if (Input.GetKey(pitchDownKey)) pitchInput = 1f;
        if (Input.GetKey(rollRightKey)) rollInput  = -1f;
        if (Input.GetKey(rollLeftKey))  rollInput  = 1f;
        if (Input.GetKey(yawRightKey))  yawInput   = -1f;
        if (Input.GetKey(yawLeftKey))   yawInput   = 1f;

        // --- Brakes (always works) ---
        if (Input.GetKeyDown(brakeToggle) || Input.GetKeyDown(KeyCode.JoystickButton0))
            airplane.brakesOn = !airplane.brakesOn;

        // --- Flaps toggle (always works) ---
        if (Input.GetKeyDown(flapsKey) || Input.GetKeyDown(flapsJoystickButton))
        {
            flapsDeployed = !flapsDeployed;
            flapTarget = flapsDeployed ? 1f : 0f;
        }
        flaps = Mathf.Lerp(flaps, flapTarget, Time.deltaTime * 2f);

        // --- Clamp and apply ---
        throttle = Mathf.Clamp01(throttle);
        airplane.throttle = throttle;

        // --- Send to Airfoils ---
        foreach (Airfoil airfoil in airplane.airfoils)
        {
            if (airfoil.airfoilType == AirfoilType.aileron)
            {
                float side = airfoil.offset > 0 ? 1f : -1f;
                airfoil.controlInput = rollInput * side;
                airfoil.flapInput    = flaps;
            }
            if (airfoil.airfoilType == AirfoilType.flap)
                airfoil.flapInput = flaps;
            if (airfoil.airfoilType == AirfoilType.elevator)
                airfoil.controlInput = pitchInput;
            if (airfoil.airfoilType == AirfoilType.rudder)
                airfoil.controlInput = yawInput;
        }

        // Must be at the very end of Update
        firstFrame = false;
    }
}