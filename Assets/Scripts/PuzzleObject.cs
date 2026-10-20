using UnityEngine;
using System.Collections;

public class PuzzleObject : InteractiveObject
{
    [Header("Grasping Properties")]
    public float graspingRadius = 0.1f;  // Radius within which the object can be grasped

    public enum DirectionOfMovement : int { x, y, z };  // Enum for the direction of movement
    [Header("Direction of Movement Properties")]
    public DirectionOfMovement directionOfMovement;  // Direction in which the object can move

    protected Transform initial_transform_parent;  // Initial parent transform of the object
    protected PuzzleController controller = null;  // Reference to the PuzzleController
    protected WandController wand_controller = null;  // Reference to the WandController

    // Check if the object is available to be grasped
    public bool is_available() { return (controller == null) && (wand_controller == null); }

    // Get the grasping radius of the object
    public float get_grasping_radius() { return graspingRadius; }

    void Start()
    {
        myType = ObjectType.Puzzle;  // Set the type of this interactive object to Puzzle
        initial_transform_parent = transform.parent;  // Store the initial parent transform
    }

    // Attach this object to the specified PuzzleController
    public void attach_to(PuzzleController controller)
    {
        this.controller = controller;
        Transform newTransform = new GameObject().transform;
        newTransform.position = controller.transform.position;
        newTransform.rotation = Quaternion.identity;
        transform.SetParent(newTransform);

        StartCoroutine(UpdatePosition(newTransform));
    }

    // Detach this object from the specified PuzzleController
    public void detach_from(PuzzleController controller)
    {
        if (this.controller != controller) return;
        this.controller = null;
        transform.SetParent(initial_transform_parent);
    }

    // Attach this object to the specified WandController
    public void attach_to_wand(WandController controller)
    {
        this.wand_controller = controller;
        Transform newTransform = new GameObject().transform;
        newTransform.position = controller.transform.position;
        newTransform.rotation = controller.transform.rotation;
        transform.SetParent(newTransform);
        StartCoroutine(UpdatePosition2(newTransform));
    }

    // Detach this object from the specified WandController
    public void detach_from_wand(WandController controller)
    {
        if (this.wand_controller != controller) return;
        this.wand_controller = null;
        transform.SetParent(initial_transform_parent);
    }

    // Coroutine to update the position of the object based on the PuzzleController's position
    private IEnumerator UpdatePosition(Transform newTransform)
    {
        while (true)
        {
            Vector3 targetPosition = newTransform.position;
            Vector3 originalPosition = newTransform.position;

            RaycastHit hit;

            // Adjust the target position based on the direction of movement
            if (directionOfMovement == DirectionOfMovement.x) targetPosition.x = controller.transform.position.x;
            if (directionOfMovement == DirectionOfMovement.y) targetPosition.y = controller.transform.position.y;
            if (directionOfMovement == DirectionOfMovement.z) targetPosition.z = controller.transform.position.z;

            Vector3 direction = targetPosition - originalPosition;

            // Check for collisions along the direction of movement
            if (Physics.Raycast(originalPosition, direction.normalized, out hit, direction.magnitude))
            {
                // If a collision is detected, adjust the target position to the point of collision
                targetPosition = hit.point;
            }

            newTransform.position = targetPosition;
            yield return null;  // Wait for the next frame
        }
    }

    // Coroutine to update the position of the object based on the WandController's position
    private IEnumerator UpdatePosition2(Transform newTransform)
    {
        while (true)
        {
            Vector3 targetPosition = newTransform.position;
            Vector3 originalPosition = newTransform.position;
            float distance = 0;

            // Calculate the distance based on the direction of movement
            if (directionOfMovement == DirectionOfMovement.x)
            {
                distance = Mathf.Abs(this.transform.position.z - wand_controller.transform.position.z);
            }
            if (directionOfMovement == DirectionOfMovement.y)
            {
                distance = Mathf.Max(Mathf.Abs(this.transform.position.x - wand_controller.transform.position.x), Mathf.Abs(this.transform.position.z - wand_controller.transform.position.z));
            }
            if (directionOfMovement == DirectionOfMovement.z)
            {
                distance = Mathf.Abs(this.transform.position.x - wand_controller.transform.position.x);
            }
            distance += 1f;

            Vector3 forwardDirection = wand_controller.transform.forward;
            Vector3 newPosition = wand_controller.transform.position + forwardDirection * distance;

            Transform temp = new GameObject().transform;
            temp.position = newPosition;
            temp.rotation = wand_controller.transform.rotation;

            RaycastHit hit;

            // Adjust the target position based on the direction of movement
            if (directionOfMovement == DirectionOfMovement.x) targetPosition.x = temp.position.x;
            if (directionOfMovement == DirectionOfMovement.y) targetPosition.y = temp.position.y;
            if (directionOfMovement == DirectionOfMovement.z) targetPosition.z = temp.position.z;

            Vector3 direction = targetPosition - originalPosition;

            // Check for collisions along the direction of movement
            if (Physics.Raycast(originalPosition, direction.normalized, out hit, direction.magnitude))
            {
                // If a collision is detected, adjust the target position to the point of collision
                targetPosition = hit.point;
            }

            if (direction.magnitude < 0.1f)
            {
                newTransform.position = targetPosition;
            }
            else
            {
                newTransform.position = originalPosition + 0.1f * direction.normalized;
            }

            yield return null;  // Wait for the next frame
        }
    }
}
