using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
	// Store the hand type to know which button should be pressed
	public enum HandType : int { LeftHand, RightHand };
	[Header("Hand Properties")]
	public HandType handType;

	// Audio sources for playing sounds when throwing or taking an object
	public AudioSource throwSound;
	public AudioSource takenSound;
	public AudioSource doorSound;
	public AudioSource releaseSound;

	// Static list to store all game objects containing an ObjectAnchor component
	static public ObjectAnchor[] anchors_in_the_scene;

	// Store previous positions of the controller over multiple frames
	private Queue<Vector3> previousControllerPositions = new Queue<Vector3>();
	// Number of previous positions to keep in memory
	private int numPreviousPositionsToTrack = 15;
	// Variable to store the previous controller position
	private Vector3 previousControllerPosition;
	// Variable to store the calculated throw force
	private float throwForce;

	// Store the previous state of triggers to detect edges
	protected bool is_hand_closed_previous_frame = false;

	// Store the object atached to this hand
	protected ObjectAnchor object_grasped = null;

	// Booleans to manage the state of object interaction
	private bool get_object = false;
	//private bool isObjectThrown = false;

	// Boolean to track if the player is holding the cagette
	public bool isHoldingCagette = false;


	void Start()
	{
		// Prevent multiple fetches of anchors
		if (anchors_in_the_scene == null) anchors_in_the_scene = GameObject.FindObjectsOfType<ObjectAnchor>();

		//in case of scene changing
		UpdateAnchorsList();
	}

	public void UpdateAnchorsList()
	{
		// Find all objects in the scene containing an ObjectAnchor component
		ObjectAnchor[] anchors = GameObject.FindObjectsOfType<ObjectAnchor>();

		// Update the anchors_in_the_scene list with these objects
		anchors_in_the_scene = anchors;
	}



	// This method checks that the hand is closed depending on the hand side
	protected bool is_hand_closed()
	{
		// Case of a left hand
		if (handType == HandType.LeftHand) return

		  OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5     // Check that the middle finger is pressing
		  && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5;   // Check that the index finger is pressing


		// Case of a right hand
		else return

			OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5   // Check that the middle finger is pressing
			&& OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5; // Check that the index finger is pressing
	}


	// Automatically called at each frame
	void Update()
	{

		if (get_object)
		{
			// Add the current controller position to the queue
			previousControllerPositions.Enqueue(transform.position);

			// Limit the number of positions in the queue to save memory
			while (previousControllerPositions.Count > numPreviousPositionsToTrack)
			{
				previousControllerPositions.Dequeue();
			}

			// Calculate the average displacement of the controller over previous frames
			Vector3 averageControllerDisplacement = Vector3.zero;
			foreach (Vector3 position in previousControllerPositions)
			{
				averageControllerDisplacement += position - previousControllerPosition;
			}
			averageControllerDisplacement /= previousControllerPositions.Count;

			// Calculate the average speed of the controller
			float controllerSpeed = averageControllerDisplacement.magnitude / Time.deltaTime;

			// Store the current controller position for the next frame
			previousControllerPosition = transform.position;

			// Adjust the throw force based on the controller speed
			throwForce = (float)(controllerSpeed * 0.5);
		}

		handle_controller_behavior();
	}



	// Method to handle the linking of object anchors to this hand controller
	protected void handle_controller_behavior()
	{

		// Check if there is a change in the grasping state (i.e. an edge) otherwise do nothing
		bool hand_closed = is_hand_closed();
		if (hand_closed == is_hand_closed_previous_frame) return;
		is_hand_closed_previous_frame = hand_closed;


		//==============================================//
		// Define the behavior when the hand get closed //
		//==============================================//
		if (hand_closed)
		{

			// Determine which object available is the closest from the left hand
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;

			// Iterate over objects to determine if we can interact with it
			for (int i = 0; i < anchors_in_the_scene.Length; i++)
			{

				// Skip object not available
				if (!anchors_in_the_scene[i].is_available()) continue;

				// Compute the distance to the object
				oject_distance = Vector3.Distance(this.transform.position, anchors_in_the_scene[i].transform.position);

				// Keep in memory the closest object
				if (oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius())
				{
					best_object_id = i;
					best_object_distance = oject_distance;
				}
			}

			// If the best object is in range grab it
			if (best_object_id != -1)
			{

				// Store in memory the object grasped
				object_grasped = anchors_in_the_scene[best_object_id];


				// Grab this object
				object_grasped.attach_to(this);
				get_object = true;

				//play a sound when an object is grabbed
				if (doorSound != null && object_grasped.CompareTag("handle"))
				{
					doorSound.Play();
				}

				if (takenSound != null && !object_grasped.CompareTag("handle"))
				{
					takenSound.Play();
				}

				//haptic feedback 
				if (handType == HandType.LeftHand)
				{
					HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.LTouch);
				}
				else
				{
					HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.RTouch);
				}

				// Check if the object is the cagette
				if (object_grasped.CompareTag("Basket"))
				{
					isHoldingCagette = true;
				}
			}
		}

		// Define the behavior when the hand get opened
		else if (object_grasped != null && get_object)
		{
			// Determine the direction to throw the object (forward direction of the main camera)
			Vector3 throwDirection = Camera.main.transform.forward.normalized;

			// Play the throw sound if it exists and an object is grasped
			if (throwSound != null && get_object && !object_grasped.CompareTag("handle"))
			{
				throwSound.Play();
			}

			if (releaseSound != null && object_grasped.CompareTag("handle")){
				releaseSound.Play();
			}

			// Get the Rigidbody component of the grasped object
			Rigidbody rb = object_grasped.GetComponent<Rigidbody>();
			if (rb != null)
			{
				// Make the rigidbody non-kinematic so physics affects it
				rb.isKinematic = false;
				// Apply force to the object in the throw direction with the calculated throw force
				rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
			}

			// Detach the object from the hand controller
			object_grasped.detach_from(this);
			// Indicate that the object is no longer being grasped
			get_object = false;


			// Check if the object is the cagette
			if (object_grasped.CompareTag("Basket"))
			{
				isHoldingCagette = false;
			}

		}
	}


}