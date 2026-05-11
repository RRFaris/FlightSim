// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class FollowCamera : MonoBehaviour
// {
//     public Transform target;
//     public Vector3 offset = new Vector3(0, 5, -15);
//     public float smoothSpeed = 10f;
//     public float lookSensitivity = 3f;
//     public float returnSpeed = 15f;
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
//         Vector3 desiredPosition = target.position + target.rotation * offset;
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