using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public Rigidbody planeRigidbody;
    public TextMeshProUGUI speedText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Safety check to prevent errors if references are missing
        if (planeRigidbody != null && speedText != null)
        {
            // .magnitude gets the total length of the velocity vector (your true speed)
            float speedMS = planeRigidbody.linearVelocity.magnitude;
            
            // Update the text. "F1" formats the number to 1 decimal place (e.g., 45.2 m/s)
            speedText.text = $"Speed: {speedMS.ToString("F1")} m/s";
        }
    }
}
