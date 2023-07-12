using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt; 
    void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    public void LateUpdate()
    {
        Vector3 deltaPos = lookAt.position - transform.position;
        transform.position += new Vector3(deltaPos.x, deltaPos.y, 0f);
    }
}