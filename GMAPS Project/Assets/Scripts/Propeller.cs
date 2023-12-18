using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    // Public variable to control the rotation speed of the propeller
    public float speed;

    private void Update()
    {
        // Rotate the propeller locally around the y-axis based on the speed and time elapsed
        transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, Vector3.up);
    }
}
