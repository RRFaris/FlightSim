using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPresets : MonoBehaviour
{
    public Rigidbody planeRigidbody;
    public Transform planeTransform;

    [Header("Preset 1 - On Runway")]
    public Vector3 preset1Position = new Vector3(0, 10, 0);
    public Vector3 preset1Rotation = new Vector3(0, 0, 0);

    [Header("Preset 2 - Approach to Runway")]
    public Vector3 preset2Position = new Vector3(0, 100, 500);
    public Vector3 preset2Rotation = new Vector3(-5, 180, 0);

    [Header("Preset 3 - High Altitude Cruise")]
    public Vector3 preset3Position = new Vector3(0, 500, 0);
    public Vector3 preset3Rotation = new Vector3(0, 90, 0);

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            ApplyPreset(preset1Position, preset1Rotation);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            ApplyPreset(preset2Position, preset2Rotation);
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            ApplyPreset(preset3Position, preset3Rotation);
    }

    void ApplyPreset(Vector3 position, Vector3 rotation)
    {
        // stop all motion first
        planeRigidbody.linearVelocity = Vector3.zero;
        planeRigidbody.angularVelocity = Vector3.zero;

        // teleport
        planeTransform.position = position;
        planeTransform.rotation = Quaternion.Euler(rotation);
    }
}
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class SpawnPresets : MonoBehaviour
// {
//     public Rigidbody planeRigidbody;
//     public Transform planeTransform;
//     public Airplane airplane;
//
//     [Header("Preset 1 - On Runway")]
//     public Vector3 preset1Position;
//     public Vector3 preset1Rotation;
//
//     [Header("Preset 2 - Approach to Runway")]
//     public Vector3 preset2Position;
//     public Vector3 preset2Rotation;
//
//     void Update()
//     {
//         if (Keyboard.current.digit1Key.wasPressedThisFrame)
//             ApplyPreset(preset1Position, preset1Rotation, 0f, 0f);
//         if (Keyboard.current.digit2Key.wasPressedThisFrame)
//             ApplyPreset(preset2Position, preset2Rotation, 27.8f, 0.5f);
//     }
//
//     void ApplyPreset(Vector3 position, Vector3 rotation, float initialSpeed, float throttle)
//     {
//         planeRigidbody.linearVelocity = Vector3.zero;
//         planeRigidbody.angularVelocity = Vector3.zero;
//
//         planeTransform.position = position;
//         planeTransform.rotation = Quaternion.Euler(rotation);
//
//         if (initialSpeed > 0f)
//             planeRigidbody.linearVelocity = -planeTransform.forward * initialSpeed;
//
//         airplane.throttle = throttle;
//     }
// }