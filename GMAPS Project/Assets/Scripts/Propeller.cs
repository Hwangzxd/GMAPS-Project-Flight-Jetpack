using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, Vector3.up);
    }
}
