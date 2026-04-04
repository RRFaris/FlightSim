using UnityEngine;

public class Airplane : MonoBehaviour
{

    public Rigidbody main;

    public Vector3 centerMass;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main.centerOfMass = centerMass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
