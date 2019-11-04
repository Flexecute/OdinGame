using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    private float zOffset=-20f;

    void LateUpdate()
    {
        if (target != null)
        {

            Vector3 goalPos = new Vector3(target.position.x, transform.position.y, target.position.z + zOffset);

            transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        }
    } 
}
