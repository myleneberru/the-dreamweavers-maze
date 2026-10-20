using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowableObject : ObjectAnchor
{

	public float SPECIFIC_FALL_HEIGHT = -1.6f;
	private bool is_attached = false;


	public override void Start()
	{
		initial_transform_parent = transform.parent;

		//Add Rigidbody component if not already attached
		if (!GetComponent<Rigidbody>())
		{
			gameObject.AddComponent<Rigidbody>();
		}
	}

	public override void attach_to(HandController hand_controller)
	{
		is_attached = true;

		// Store the hand controller in memory
		this.hand_controller = hand_controller;

		// Set the object to be placed in the hand controller referential
		transform.SetParent(hand_controller.transform);

		// Activate or deactivate the Rigidbody based on the needs
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true; // Ensure Rigidbody is kinematic when attached to hand
		}

	}


	// This method overrides the detach_from method in the ObjectAnchor class
	public override void detach_from(HandController hand_controller)
	{
		is_attached = false;

		// Make sure that the right hand controller ask for the release
		if (this.hand_controller != hand_controller) return;

		// Detach the hand controller
		this.hand_controller = null;

		// Set the object to be placed in the original transform parent
		transform.SetParent(initial_transform_parent);

		// Activate or deactivate the Rigidbody based on the needs
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = false; // Re-enable Rigidbody when detached from hand
		}

		//if the object is under the ground
		if (transform.position.y <= SPECIFIC_FALL_HEIGHT)
		{
			// Define the respawn position
			Vector3 RespawnPos = new Vector3(transform.position.x, 0.5f, transform.position.z);

			//respawn on the floor
			this.transform.position = RespawnPos;
		}
	}

	public bool get_is_attached()
	{
		return is_attached;
	}

}