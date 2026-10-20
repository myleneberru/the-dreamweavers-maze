using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [Header("Grasping Properties")]
    public float graspingRadius = 1f;  // Radius within which the object can be grasped
    public enum ObjectType { Candle, Wand, Potion };  // Enum for different object types
    public ObjectType myType;  // Type of this object

    protected Transform initial_transform_parent;  // Original parent transform of the object
    protected WandController wandController = null;  // Reference to the WandController currently holding this object

    // Get the WandController currently holding this object
    public WandController get_wand_controller() { return wandController; }

    // Check if the object is available to be grasped
    public bool is_available() { return wandController == null; }

    // Get the radius within which the object can be grasped
    public float get_grasping_radius() { return graspingRadius; }

    // Get the type of this object
    public ObjectType get_my_type() { return myType; }

    private GameObject player;  // Reference to the player object

    void Start()
    {
        initial_transform_parent = transform.parent;  // Store the initial parent transform
        player = GameObject.FindWithTag("Player");  // Find the player object by tag
    }

    void Update()
    {
        // Update method left empty intentionally
    }

    // Attach this object to the specified WandController
    public void attach_to(WandController wandController)
    {
        this.wandController = wandController;  // Set the WandController reference
        transform.position = wandController.transform.position;  // Match position with the WandController
        transform.rotation = Quaternion.LookRotation(wandController.transform.forward, wandController.transform.up);  // Align rotation with the WandController

        // If the object is of type Wand, adjust its rotation
        if (myType == ObjectType.Wand)
        {
            transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
        }

        transform.SetParent(wandController.transform);  // Set the WandController as the parent transform
    }

    // Detach this object from the specified WandController
    public void detach_from(WandController wandController)
    {
        // Ensure the WandController is the same as the one holding this object
        if (this.wandController != wandController) return;

        this.wandController = null;  // Clear the WandController reference
        transform.SetParent(initial_transform_parent);  // Reset the parent transform to the initial parent
    }
}
