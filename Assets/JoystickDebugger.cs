using UnityEngine;

public class JoystickDebugger : MonoBehaviour
{
    void Update()
    {
        for (int i = 1; i <= 4; i++)
        {
            float val = Input.GetAxisRaw("Joystick Axis " + i);
            if (Mathf.Abs(val) > 0.1f)
                Debug.Log("Axis " + i + " = " + val);
        }
    }
}