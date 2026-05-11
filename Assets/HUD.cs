using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI hudText;
    public Rigidbody planeRigidbody;

    void Update()
    {
        float speedMS = planeRigidbody.linearVelocity.magnitude;
        float speedMPH = speedMS * 2.23694f;
        float altitudeFeet = planeRigidbody.transform.position.y * 3.28084f;
        float verticalSpeedFPM = planeRigidbody.linearVelocity.y * 196.85f;

        hudText.text = $"Speed: {speedMPH:F0} mph\n" +
                       $"Altitude: {altitudeFeet:F0} ft\n";
    }
}
