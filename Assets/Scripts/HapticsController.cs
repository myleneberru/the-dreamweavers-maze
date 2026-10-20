using UnityEngine;

public class HapticsController : MonoBehaviour
{
    public static HapticsController singleton;  // Singleton instance of the HapticsController
    private bool isVibrating = false;  // Flag to track if vibration is currently active

    void Start()
    {
        // Ensure there is only one instance of the HapticsController
        if (singleton && singleton != this)
            Destroy(this);  // Destroy this instance if another exists
        else
            singleton = this;  // Assign this as the singleton instance
    }

    // Trigger haptic feedback on the specified controller
    public void TriggerHapticFeedback(float freq, float strenght, float duration, OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(freq, strenght, controller);  // Set the vibration frequency and strength
        Invoke("StopHapticFeedback", duration);  // Schedule to stop the haptic feedback after the duration
    }

    // Stop haptic feedback on both left and right controllers
    void StopHapticFeedback()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);  // Stop vibration on the left controller
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);  // Stop vibration on the right controller
    }

    // Start vibration on the specified controller
    public void StartVibration(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(0.5f, 0.5f, controller);  // Set the vibration to half frequency and strength
        isVibrating = true;  // Set the vibrating flag to true
    }

    // Stop vibration on the specified controller
    public void StopVibration(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(0f, 0f, controller);  // Stop the vibration
        isVibrating = false;  // Set the vibrating flag to false
    }
}
