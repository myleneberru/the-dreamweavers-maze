using UnityEngine;

public class ObjectAnchor : MonoBehaviour
{

	[Header("Grasping Properties")]
	public float graspingRadius = 0.1f;

	// Store initial transform parent
	protected Transform initial_transform_parent;
	public virtual void Start()
	{
		initial_transform_parent = transform.parent;

	}

	void Update()
	{
		// Find the GameObject with "Player" Tag
		GameObject player = GameObject.FindWithTag("Player");

		if (player != null)
		{
			// Check if the player enters within a radius of 5 around the object
			if (Vector3.Distance(transform.position, player.transform.position) <= 5f)
			{
				// Activate Outline script if desactivated 
				Outline outline = GetComponent<Outline>();
				if (outline != null && !outline.enabled)
				{
					outline.enabled = true;
				}
			}
			else
			{
				// Desactivate Outline script if activated 
				Outline outline = GetComponent<Outline>();
				if (outline != null && outline.enabled)
				{
					outline.enabled = false;
				}
			}
		}
	}

	// Store the hand controller this object will be attached to
	protected HandController hand_controller = null;

	public virtual void attach_to(HandController hand_controller)
	{
		// Store the hand controller in memory
		this.hand_controller = hand_controller;

		// Set the object to be placed in the hand controller referential
		transform.SetParent(hand_controller.transform);

	}

	public virtual void detach_from(HandController hand_controller)
	{
		// Make sure that the right hand controller ask for the release
		if (this.hand_controller != hand_controller) return;

		// Detach the hand controller
		this.hand_controller = null;

		// Set the object to be placed in the original transform parent
		transform.SetParent(initial_transform_parent);

	}

	public bool is_available() { return hand_controller == null; }

	public float get_grasping_radius() { return graspingRadius; }
}