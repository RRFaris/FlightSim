using System;
using UnityEngine;

[Serializable]
public struct AirfoilAnimation
{
    public Airfoil airfoil;
    public GameObject model;
    
    [Tooltip("Maximum visual deflection angle in degrees")]
    public float maxAngle;
    
    [Tooltip("Which axis the control surface rotates on")]
    public enum RotationAxis { X, Y, Z }
    public RotationAxis axis;
    
    [Tooltip("Smoothing speed - higher = snappier response")]
    public float smoothSpeed;

    [Tooltip("Check this for pure flap surfaces that don't also act as ailerons")]
    public bool isFlapOnly;
}

public class AirfoilAnimations : MonoBehaviour
{
    public AirfoilAnimation[] animations;

    void Update()
    {
        foreach (AirfoilAnimation animation in animations)
        {
            if (animation.model == null || animation.airfoil == null) continue;

            // Pure flap surfaces only use flapInput
            // Flaperons combine both
            float totalInput;
            if (animation.isFlapOnly)
                totalInput = animation.airfoil.flapInput;
            else
                totalInput = animation.airfoil.controlInput + animation.airfoil.flapInput;

            float targetAngle = animation.maxAngle * Mathf.Clamp(totalInput, -1f, 1f);

            Vector3 currentAngles = animation.model.transform.localEulerAngles;
            float currentAngle = 0f;
            float newAngle = 0f;

            switch (animation.axis)
            {
                case AirfoilAnimation.RotationAxis.X:
                    currentAngle = currentAngles.x > 180 ? currentAngles.x - 360 : currentAngles.x;
                    newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * animation.smoothSpeed);
                    animation.model.transform.localEulerAngles = new Vector3(newAngle, currentAngles.y, currentAngles.z);
                    break;

                case AirfoilAnimation.RotationAxis.Y:
                    currentAngle = currentAngles.y > 180 ? currentAngles.y - 360 : currentAngles.y;
                    newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * animation.smoothSpeed);
                    animation.model.transform.localEulerAngles = new Vector3(currentAngles.x, newAngle, currentAngles.z);
                    break;

                case AirfoilAnimation.RotationAxis.Z:
                    currentAngle = currentAngles.z > 180 ? currentAngles.z - 360 : currentAngles.z;
                    newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * animation.smoothSpeed);
                    animation.model.transform.localEulerAngles = new Vector3(currentAngles.x, currentAngles.y, newAngle);
                    break;
            }
        }
    }
}