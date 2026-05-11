using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI hudText;
    public Rigidbody planeRigidbody;
    public Airplane airplane;
    public Transform planeTransform;

    public float stallSpeed = 55f;

    private Vector3 lastVelocity;
    private float gForce;

    void FixedUpdate()
    {
        Vector3 acceleration = (planeRigidbody.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
        gForce = (acceleration - Physics.gravity).magnitude / 9.81f;
        lastVelocity = planeRigidbody.linearVelocity;
    }

    void Update()
    {
        float speedMS = planeRigidbody.linearVelocity.magnitude;
        float speedMPH = speedMS * 2.23694f;
        float altitudeFeet = planeRigidbody.transform.position.y * 3.28084f;
        float throttlePercent = airplane.throttle * 100f;

        float radarAltFeet = 0f;
        RaycastHit hit;
        if (Physics.Raycast(planeTransform.position, Vector3.down, out hit))
            radarAltFeet = hit.distance * 3.28084f;

        float pitch = planeTransform.eulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        pitch = -pitch;

        bool isStalling = (speedMPH < stallSpeed && radarAltFeet > 20f) ||
                          (pitch > 30f && speedMPH < stallSpeed * 1.5f && radarAltFeet > 20f);

        hudText.text = $"THR:<pos=100>{throttlePercent:F0}  %\n" +
                       $"SPD:<pos=100>{speedMPH:F0}  mph\n" +
                       $"ALT:<pos=100>{altitudeFeet:F0}  ft\n" +
                       $"RALT:<pos=100>{radarAltFeet:F0}  ft\n\n" +
                       $"PITCH:<pos=100>{pitch:F1}°\n" +
                       $"G:<pos=100>{gForce:F1}\n" +
                       (isStalling ? "<color=red>!! STALL !!</color>" : "");
    }
}