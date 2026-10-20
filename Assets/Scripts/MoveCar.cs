using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float speed = 5f; // Speed of the car
    public Transform cameraTransform;

    public AudioSource minecartSound; // AudioSource component for playing sounds
    public AudioSource blockedSound;

    private bool isPlayerAttached = false;

    private bool start = false;
    private bool end = false;

    public void SetPlayerAttached(bool attached)
    {
        isPlayerAttached = attached; // Set whether the player is attached to the car
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // If enter in collision it respawns at the start
        if (other.CompareTag("Respawn"))
        {
            start = true;
        }

        // If enter in collision with finish it changes scene
        else if (other.CompareTag("Finish"))
        {
            end = true;
            ChangeSceneManager.singleton.ChangeScene();

        }
    }

    // Manages OnTriggerExit 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            start = false;

        }
        else if (other.CompareTag("Finish"))
        {
            end = false;
        }
    }


    void Update()
    {
        // If the player is not attached, stop any ongoing audio and return
        if (!isPlayerAttached)
        {
            if (minecartSound.isPlaying)
                minecartSound.Stop();
            return;
        }

        // Get the pressure values from both triggers
        float leftPressure = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float rightPressure = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        // Calculate the acceleration based on the maximum trigger pressure
        float acc = speed * Mathf.Max(leftPressure, rightPressure);

        // Calculate the forward vector of the attached camera, neutralizing the vertical component
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Neutralize the vertical component to ensure horizontal movement
        cameraForward.Normalize(); // Normalize the vector to maintain consistent speed

        // Determine the movement direction based on which trigger is pressed more
        Vector3 direction = Vector3.zero;

        if (leftPressure > rightPressure && start)
        {
            direction = Vector3.zero;
            HapticsController.singleton.TriggerHapticFeedback(1f, 5f, 0.5f, OVRInput.Controller.LTouch);
            blockedSound.Play();
        }
        else if (leftPressure < rightPressure && end)
        {
            direction = Vector3.zero;
            HapticsController.singleton.TriggerHapticFeedback(1f, 5f, 0.5f, OVRInput.Controller.RTouch);
            blockedSound.Play();
        }
        else if (leftPressure < rightPressure)
        {
            direction = cameraForward; // Move forwards if the right trigger is more pressed
        }
        else if (leftPressure > rightPressure)
        {
            direction = -cameraForward;
        }
     
        transform.position += direction * acc * Time.deltaTime;


        // Play sound when the car starts moving and stop when it's not moving
        if (direction != Vector3.zero)
        {
            if (!minecartSound.isPlaying)
                minecartSound.Play(); // Play sound only if it is not already playing
        }
        else if (minecartSound.isPlaying) // Stop sound if there is no movement
        {
            minecartSound.Stop();
        }
    }
}
