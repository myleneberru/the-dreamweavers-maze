using UnityEngine;
using UnityEngine.Events;

public class DoorSound : MonoBehaviour
{
    private HingeJoint hingeJoint;  // Reference to the HingeJoint component
    [SerializeField] float doorOpenAngle = 1f;  // The angle at which the door is considered open
    [SerializeField] hingeJointDefaultState hingeJointDefault = hingeJointDefaultState.closed;  // The default state of the hinge joint (open or closed)

    // Enum to represent the default state of the hinge joint
    enum hingeJointDefaultState { open, closed }
    bool isDoorOpen, isDoorClosed = false;  // Flags to track the door's open and closed states

    [SerializeField] UnityEvent OnDoorOpenEvent;  // Event to invoke when the door opens
    [SerializeField] UnityEvent OnDoorCloseEvent;  // Event to invoke when the door closes

    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();  // Get the HingeJoint component attached to this object
        if (hingeJointDefault == hingeJointDefaultState.closed)
        {
            isDoorClosed = true;  // Set the door state to closed if the default state is closed
        }
    }

    private void FixedUpdate()
    {
        // Check if the door angle exceeds the open angle threshold
        if (hingeJoint.angle > doorOpenAngle)
        {
            if (isDoorOpen == false)
            {
                Debug.Log("DoorOpen");  // Log the door open event
                OnDoorCloseEvent.Invoke();  // Invoke the door close event
                isDoorOpen = true;  // Set the door open flag to true
                isDoorClosed = false;  // Set the door closed flag to false
            }
        }
        // Check if the door angle is below the open angle threshold
        else if (hingeJoint.angle < doorOpenAngle)
        {
            if (isDoorClosed == false)
            {
                Debug.Log("DoorClosed");  // Log the door closed event
                OnDoorOpenEvent.Invoke();  // Invoke the door open event
                isDoorClosed = true;  // Set the door closed flag to true
                isDoorOpen = false;  // Set the door open flag to false
            }
        }
    }
}
