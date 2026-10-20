using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumCollision : MonoBehaviour
{
    public Transform respawnPoint;  // The point to which the object will be moved upon collision
    public AudioSource collisionSound;  // The sound to play on collision

    // Triggered when another collider enters this object's collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pendulum"))
        {
            collisionSound.Play();  // Play the collision sound

            // Trigger haptic feedback on both the right and left controllers
            HapticsController.singleton.TriggerHapticFeedback(1f, 5f, 1f, OVRInput.Controller.RTouch);
            HapticsController.singleton.TriggerHapticFeedback(1f, 5f, 1f, OVRInput.Controller.LTouch);

            // Move the object to the respawn point and set its rotation to the respawn point's rotation
            transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        }
    }
}
