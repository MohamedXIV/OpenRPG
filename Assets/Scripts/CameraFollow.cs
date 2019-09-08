using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    //[SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + offset; // * smoothSpeed * Time.deltaTime;
    }
}
