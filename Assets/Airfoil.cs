using System;
using UnityEngine;

public class Airfoil : MonoBehaviour
{
    public float avgChord = 1;
    public float span = 2;
    public float offset = 2;

    private float area;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void applyLift(Rigidbody main, float airDensity, Vector3 localVelocity)
    {
        area = avgChord * span;
        double liftMagnitude = Math.Pow(localVelocity.z, 2) * (1.60 * airDensity * 0.5 * area);
        Vector3 liftVector = Vector3.up * (float)liftMagnitude;
        main.AddForceAtPosition(liftVector, transform.position);
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
}
