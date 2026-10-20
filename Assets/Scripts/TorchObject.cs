using UnityEngine;

public class TorchObject : InteractiveObject
{
    public GameObject testcube;  // Reference to a test cube object in the scene
    private bool is_on = false;  // Flag to track if the torch is on or off
    private AudioSource audioSource;  // Reference to the audio source component

    void Start()
    {
        myType = ObjectType.Torch;  // Set the type of this interactive object to 'Torch'
        testcube.SetActive(false);  // Initially hide the test cube
        is_on = false;  // Initially set the torch as off
        audioSource = GetComponent<AudioSource>();  // Get the audio source component attached to this object
    }

    void OnTriggerEnter(Collider other)
    {
        if (!is_on)  // Check if the torch is currently off
        {
            GrabbableObject grabbableObject = other.GetComponent<GrabbableObject>();  // Try to get a GrabbableObject component from the collider
            if (grabbableObject != null && grabbableObject.get_my_type() == GrabbableObject.ObjectType.Candle)  // Check if the collider is a Candle
            {
                WandController wandController = grabbableObject.get_wand_controller();  // Get the wand controller associated with the candle
                is_on = true;  // Set the torch as on
                testcube.SetActive(true);  // Show the test cube

                // Trigger haptic feedback on the appropriate hand
                if (wandController.get_handtype() == WandController.HandType.LeftHand)
                {
                    HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.LTouch);  // Left hand haptic feedback
                }
                else
                {
                    HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.RTouch);  // Right hand haptic feedback
                }

                audioSource.Play();  // Play the audio source
            }
        }
        if (other.CompareTag("Fireball"))  // Check if the collider has the tag 'Fireball'
        {
            other.gameObject.SetActive(false);  // Deactivate the fireball object
            toggle_visibility();  // Toggle the visibility of the test cube and torch state
        }
    }

    public void toggle_visibility()
    {
        testcube.SetActive(!testcube.activeSelf);  // Toggle the active state of the test cube
        is_on = !is_on;  // Toggle the torch state
        if (is_on) audioSource.Play();  // Play the audio source if the torch is now on
        else audioSource.Stop();  // Stop the audio source if the torch is now off
    }
}
