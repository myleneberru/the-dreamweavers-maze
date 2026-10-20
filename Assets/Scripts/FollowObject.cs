using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;  // The target transform that this object will follow
    Rigidbody rb;  // Reference to the Rigidbody component

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get the Rigidbody component attached to this object
    }

    void Update()
    {
        rb.MovePosition(target.transform.position);  // Move this object to the target's position
    }
}
