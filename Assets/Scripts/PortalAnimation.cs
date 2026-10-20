using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalAnimation : MonoBehaviour
{
    public string sceneToLoad = "Tutoriel";  // Name of the scene to load
    public float floatHeight = 0.2f;  // Height of the floating motion
    public float floatSpeed = 1f;  // Speed of the floating motion
    public float rotationSpeed = 50f;  // Speed of the rotation
    private Vector3 startPosition;  // Initial position of the portal

    // Called at the start of the script
    void Start()
    {
        startPosition = transform.position;  // Store the initial position of the portal
    }

    // Called once per frame
    void Update()
    {
        // Create a new position based on the initial position
        Vector3 newPosition = startPosition;
        // Apply a floating motion using a sine wave
        newPosition.y += Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        // Update the portal's position
        transform.position = newPosition;
        // Rotate the portal randomly within a unit sphere
        transform.Rotate(Random.insideUnitSphere * rotationSpeed * Time.deltaTime);
    }

    // Triggered when another collider enters this object's collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);  // Load the specified scene
        }
    }
}

