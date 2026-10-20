using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCar : MonoBehaviour
{
    public Portal portal;  // Reference to the Portal script
    public AudioClip clicNoise;  // Audio clip to play when the red car reaches the target position
    private bool active = true;  // Flag to indicate if the car is active
    private PuzzleObject puzzle;  // Reference to the PuzzleObject component

    void Start()
    {
        puzzle = GetComponent<PuzzleObject>();  // Get the PuzzleObject component attached to this GameObject
    }

    void Update()
    {
        if (active)
        {
            // Check if the car's position on the z-axis exceeds 24.8
            if (transform.position.z > 24.8f)
            {
                portal.redCarDone = true;  // Mark the red car task as done in the Portal script
                puzzle.enabled = false;  // Disable the PuzzleObject component
                AudioSource.PlayClipAtPoint(clicNoise, transform.position);  // Play the click noise at the car's position
                active = false;  // Set the car to inactive
            }
        }
    }
}

