using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumController : MonoBehaviour
{
    public float speed = 1.5f;  // Speed of the pendulum's oscillation
    public float limit = 75f;  // Maximum angle of the pendulum's swing
    public bool shiftOscillation = false;  // Whether to randomly shift the oscillation
    private float random = 0;  // Variable to store the random shift value

    // Called when the script instance is being loaded
    void Awake() 
    {
        if (shiftOscillation)
            random = Random.Range(0f, 2f);  // Generate a random value to shift the oscillation
    }

    // Called once per frame
    void Update()
    {
        float angle = limit * Mathf.Sin(Time.time * speed + random);  // Calculate the angle of oscillation

        transform.localRotation = Quaternion.Euler(180, 90, angle);  // Apply the rotation to the pendulum
    }    
}
