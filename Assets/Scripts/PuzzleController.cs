using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    // Enum to define the type of hand (left or right)
    public enum HandType : int { LeftHand, RightHand };

    [Header("Hand Properties")]
    public HandType handType; // The type of hand (left or right)

    [Header("Player Controller")]
    public MainPlayerController playerController; // Reference to the main player controller

    // Static array to hold all puzzle objects in the scene
    static protected PuzzleObject[] anchors_in_the_scene;
    protected bool is_hand_closed_previous_frame = false; // Track if the hand was closed in the previous frame
    protected PuzzleObject object_grasped = null; // Reference to the currently grasped object

    void Start()
    {
        // Find and store all puzzle objects in the scene
        if (anchors_in_the_scene == null)
            anchors_in_the_scene = GameObject.FindObjectsOfType<PuzzleObject>();
    }

    void Update()
    {
        // Check if the hand is currently closed
        bool hand_closed = is_hand_closed();
        if (hand_closed == is_hand_closed_previous_frame) return;
        is_hand_closed_previous_frame = hand_closed;

        if (hand_closed) // If the hand gets closed, try to grab an object
        {
            int best_object_id = -1;
            float best_object_distance = float.MaxValue;
            float object_distance;

            // Determine the best object to grab
            for (int i = 0; i < anchors_in_the_scene.Length; i++)
            {
                if (!anchors_in_the_scene[i].is_available()) continue;
                object_distance = Vector3.Distance(this.transform.position, anchors_in_the_scene[i].transform.position);
                if (object_distance < best_object_distance && object_distance <= anchors_in_the_scene[i].get_grasping_radius())
                {
                    best_object_id = i;
                    best_object_distance = object_distance;
                }
            }

            // If a suitable object is found, grab it
            if (best_object_id != -1)
            {
                object_grasped = anchors_in_the_scene[best_object_id];
                object_grasped.attach_to(this);
            }
        }
        else if (object_grasped != null) // If the hand gets opened, try to drop the object
        {
            object_grasped.detach_from(this);
        }
    }

    // Function to check if the hand is closed
    protected bool is_hand_closed()
    {
        if (handType == HandType.LeftHand)
        {
            return
                OVRInput.Get(OVRInput.Button.Three) &&
                OVRInput.Get(OVRInput.Button.Four) &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5 &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5;
        }
        else
        {
            return
                OVRInput.Get(OVRInput.Button.One) &&
                OVRInput.Get(OVRInput.Button.Two) &&
                OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5 &&
                OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5;
        }
    }
}
