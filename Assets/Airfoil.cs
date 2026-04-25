using System;
using UnityEngine;

public class Airfoil : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 localVelocity;
    public float angleOfAttack;
    
    public float avgChord = 1;
    public float span = 2;
    public float offset = 2;

    private float area;
    public AnimationCurve liftCurve;
    
    [Range(-1, 1)] public float controlInput = 0;
    public float maxControlDeflection = 15f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void applyLift(Rigidbody main, float airDensity, Vector3 localVelocity)
    {
        CalculateState(main);
        
        // Guard against divide by 0 error when using .normalize
        if (localVelocity.sqrMagnitude < 0.1f) return;
        
        // float liftCoefficient = liftCurve.Evaluate(angleOfAttack);
        
        float effectiveAoA = angleOfAttack + (controlInput * maxControlDeflection);
        float liftCoefficient = liftCurve.Evaluate(effectiveAoA);
        
        area = avgChord * span;
        double liftMagnitude = localVelocity.sqrMagnitude * (liftCoefficient * airDensity * 0.5 * area);

        Vector3 localLiftDirection = Vector3.Cross(localVelocity, Vector3.right).normalized;
        Vector3 liftVector = transform.TransformDirection(localLiftDirection) * (float)liftMagnitude;
        
        float dragCoefficient = 0.05f + Mathf.Abs(Mathf.Sin(angleOfAttack * Mathf.Deg2Rad)); 
        float dragMagnitude = localVelocity.sqrMagnitude * (dragCoefficient * airDensity * 0.5f * area);
        
        Vector3 dragVector = transform.TransformDirection(-localVelocity.normalized) * dragMagnitude;
        
        main.AddForceAtPosition(liftVector + dragVector, transform.position + (transform.right * offset));
        Debug.DrawRay(transform.position + (transform.right * offset),liftVector, Color.red);
        
    }
    
    
    private void OnDrawGizmos()
    {
        DrawFoilShape();
    }

    // Update is called once per frame
    public void DrawFoilShape()
    {
        // Creates the points
        Vector3 point1 = new Vector3(-span/2 + offset, 0, avgChord/2);
        Vector3 point2 = new Vector3(span/2 + offset, 0, avgChord/2);
        Vector3 point3 = new Vector3(span/2 + offset, 0, -avgChord/2);
        Vector3 point4 = new Vector3(-span/2 + offset, 0, -avgChord/2);
        
        // Transforms points from local space to world space
        point1 = transform.TransformPoint(point1);
        point2 = transform.TransformPoint(point2);
        point3 = transform.TransformPoint(point3);
        point4 = transform.TransformPoint(point4);

        // Draws lines between the points
        Gizmos.color = Color.white;
        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);
        Gizmos.DrawLine(point2, point4);
    }

    public void CalculateState(Rigidbody main)
    {
        var invRotation = Quaternion.Inverse(transform.rotation);
        velocity = main.GetPointVelocity(transform.position);
        localVelocity = invRotation * velocity;
        CalculateAngleOfAttack();
    }

    public void CalculateAngleOfAttack()
    {
        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;
    }
}
