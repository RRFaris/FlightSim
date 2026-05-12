// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class CameraFollow : MonoBehaviour
// {
//     public Transform target;
//     public Vector3 offset = new Vector3(0, 3, 0);
//     public float smoothSpeed = 10f;
//     public float lookSensitivity = 3f;
//     public float returnSpeed = 15f;
//
//     [Header("Dynamic Distance")]
//     public float minDistance = 8f;
//     public float maxDistance = 20f;
//     public float maxSpeed = 63f;
//
//     [Header("First Person")]
//     public Vector3 firstPersonOffset = new Vector3(0, 1.5f, 1.5f); // adjust to sit inside cockpit
//     public bool isFirstPerson = false;
//
//     public Rigidbody planeRigidbody;
//
//     private float lookYaw = 0f;
//     private float lookPitch = 0f;
//     private bool isLooking = false;
//     private bool isReturning = false;
//     private Vector3 smoothedPosition;
//     private Vector3 smoothedOrbitPosition;
//     private Vector3 returnPosition;
//     private float orbitDistance;
//
//     void Start()
//     {
//         smoothedPosition = target.position + target.rotation * offset;
//         returnPosition = smoothedPosition;
//     }
//
//     void FixedUpdate()
//     {
//         if (isFirstPerson) return;
//
//         float speed = planeRigidbody != null ? planeRigidbody.linearVelocity.magnitude : 0f;
//         float t = Mathf.Clamp01(speed / maxSpeed);
//         float currentDistance = Mathf.Lerp(minDistance, maxDistance, t);
//
//         Vector3 dynamicOffset = new Vector3(offset.x, offset.y, -currentDistance);
//         Vector3 desiredPosition = target.position + target.rotation * dynamicOffset;
//         smoothedPosition = Vector3.Lerp(smoothedPosition, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
//
//         if (isLooking)
//         {
//             Quaternion rotation = Quaternion.Euler(lookPitch, lookYaw, 0f);
//             Vector3 orbitOffset = rotation * Vector3.back * orbitDistance;
//             Vector3 desiredOrbit = target.position + orbitOffset;
//             smoothedOrbitPosition = Vector3.Lerp(smoothedOrbitPosition, desiredOrbit, smoothSpeed * Time.fixedDeltaTime);
//         }
//
//         if (isReturning)
//         {
//             returnPosition = Vector3.Lerp(returnPosition, smoothedPosition, returnSpeed * Time.fixedDeltaTime);
//
//             if (Vector3.Distance(returnPosition, smoothedPosition) < 0.05f)
//             {
//                 isReturning = false;
//                 returnPosition = smoothedPosition;
//                 lookYaw = 0f;
//                 lookPitch = 0f;
//             }
//         }
//     }
//
//     void Update()
//     {
//         // toggle first person with C key
//         if (Keyboard.current.cKey.wasPressedThisFrame)
//         {
//             isFirstPerson = !isFirstPerson;
//             isLooking = false;
//             isReturning = false;
//             Cursor.lockState = CursorLockMode.None;
//         }
//
//         if (isFirstPerson)
//         {
//             // snap camera inside cockpit
//             transform.position = target.position + target.rotation * firstPersonOffset;
//             transform.rotation = target.rotation;
//             return;
//         }
//
//         if (Keyboard.current.leftShiftKey.isPressed)
//         {
//             isReturning = false;
//
//             if (!isLooking)
//             {
//                 Vector3 dir = transform.position - target.position;
//                 lookYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + 180f;
//                 lookPitch = Mathf.Asin(dir.normalized.y) * Mathf.Rad2Deg;
//                 orbitDistance = dir.magnitude;
//                 smoothedOrbitPosition = transform.position;
//                 isLooking = true;
//                 Cursor.lockState = CursorLockMode.Locked;
//             }
//
//             Vector2 mouseDelta = Mouse.current.delta.ReadValue();
//             lookYaw += mouseDelta.x * lookSensitivity;
//             lookPitch -= mouseDelta.y * lookSensitivity;
//             lookPitch = Mathf.Clamp(lookPitch, -60f, 60f);
//
//             transform.position = smoothedOrbitPosition;
//             transform.LookAt(target);
//         }
//         else
//         {
//             if (isLooking)
//             {
//                 isLooking = false;
//                 isReturning = true;
//                 returnPosition = transform.position;
//                 Cursor.lockState = CursorLockMode.None;
//             }
//
//             transform.position = isReturning ? returnPosition : smoothedPosition;
//             transform.LookAt(target);
//         }
//     }
// }


using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, 0);
    public float smoothSpeed = 10f;
    public float lookSensitivity = 3f;
    public float returnSpeed = 15f;

    [Header("Dynamic Distance")]
    public float minDistance = 8f;
    public float maxDistance = 20f;
    public float maxSpeed = 63f;

    [Header("First Person / Cockpit")]
    public Vector3 firstPersonOffset = new Vector3(0, 1.5f, 1.5f);
    public bool isFirstPerson = false;
    // Limits for cockpit looking
    public float cockpitPitchLimit = 70f;
    public float cockpitYawLimit = 90f;

    public Rigidbody planeRigidbody;

    private float lookYaw = 0f;
    private float lookPitch = 0f;
    private bool isLooking = false;
    private bool isReturning = false;
    private Vector3 smoothedPosition;
    private Vector3 smoothedOrbitPosition;
    private Vector3 returnPosition;
    private float orbitDistance;

    void Start()
    {
        smoothedPosition = target.position + target.rotation * offset;
        returnPosition = smoothedPosition;
    }

    void FixedUpdate()
    {
        // Don't run third-person physics logic if we are in the cockpit
        if (isFirstPerson) return;

        float speed = planeRigidbody != null ? planeRigidbody.linearVelocity.magnitude : 0f;
        float t = Mathf.Clamp01(speed / maxSpeed);
        float currentDistance = Mathf.Lerp(minDistance, maxDistance, t);

        Vector3 dynamicOffset = new Vector3(offset.x, offset.y, -currentDistance);
        Vector3 desiredPosition = target.position + target.rotation * dynamicOffset;
        smoothedPosition = Vector3.Lerp(smoothedPosition, desiredPosition, smoothSpeed * Time.fixedDeltaTime);

        if (isLooking)
        {
            Quaternion rotation = Quaternion.Euler(lookPitch, lookYaw, 0f);
            Vector3 orbitOffset = rotation * Vector3.back * orbitDistance;
            Vector3 desiredOrbit = target.position + orbitOffset;
            smoothedOrbitPosition = Vector3.Lerp(smoothedOrbitPosition, desiredOrbit, smoothSpeed * Time.fixedDeltaTime);
        }

        if (isReturning)
        {
            returnPosition = Vector3.Lerp(returnPosition, smoothedPosition, returnSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(returnPosition, smoothedPosition) < 0.05f)
            {
                isReturning = false;
                returnPosition = smoothedPosition;
                lookYaw = 0f;
                lookPitch = 0f;
            }
        }
    }

    void Update()
    {
        // Toggle first person
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isFirstPerson = !isFirstPerson;
            // Reset angles when switching modes
            lookYaw = 0f;
            lookPitch = 0f;
            isLooking = false;
            isReturning = false;
            
            if(isFirstPerson) Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;
        }

        if (isFirstPerson)
        {
            HandleCockpitView();
        }
        else
        {
            HandleThirdPersonView();
        }
    }

    void HandleCockpitView()
    {
        // Get mouse input
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        lookYaw += mouseDelta.x * lookSensitivity;
        lookPitch -= mouseDelta.y * lookSensitivity;

        // Clamp the view so you can't look behind your own seat too far
        lookYaw = Mathf.Clamp(lookYaw, -cockpitYawLimit, cockpitYawLimit);
        lookPitch = Mathf.Clamp(lookPitch, -cockpitPitchLimit, cockpitPitchLimit);

        // Position the camera at the cockpit offset
        transform.position = target.position + target.rotation * firstPersonOffset;

        // Combine the plane's rotation with our local "look" rotation
        Quaternion localLookRotation = Quaternion.Euler(lookPitch, lookYaw, 0f);
        transform.rotation = target.rotation * localLookRotation;
    }

    void HandleThirdPersonView()
    {
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            isReturning = false;

            if (!isLooking)
            {
                Vector3 dir = transform.position - target.position;
                lookYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + 180f;
                lookPitch = Mathf.Asin(dir.normalized.y) * Mathf.Rad2Deg;
                orbitDistance = dir.magnitude;
                smoothedOrbitPosition = transform.position;
                isLooking = true;
                Cursor.lockState = CursorLockMode.Locked;
            }

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            lookYaw += mouseDelta.x * lookSensitivity;
            lookPitch -= mouseDelta.y * lookSensitivity;
            lookPitch = Mathf.Clamp(lookPitch, -60f, 60f);

            transform.position = smoothedOrbitPosition;
            transform.LookAt(target);
        }
        else
        {
            if (isLooking)
            {
                isLooking = false;
                isReturning = true;
                returnPosition = transform.position;
                Cursor.lockState = CursorLockMode.None;
            }

            transform.position = isReturning ? returnPosition : smoothedPosition;
            transform.LookAt(target);
        }
    }
}