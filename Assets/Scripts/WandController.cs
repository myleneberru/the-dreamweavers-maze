using UnityEngine;
using System.Collections.Generic;


public class WandController : MonoBehaviour
{

	public enum HandType : int { LeftHand, RightHand };
	[Header("Hand Properties")]
	public HandType handType;
	[Header("Player Controller")]
	public MainPlayerController playerController;
	static protected GrabbableObject[] anchors_in_the_scene;

	protected GrabbableObject object_in_hand = null;// object that is currently in hand
	protected WandObject wand_in_hand = null;// wand in hand
	protected PuzzleObject puzzle = null;//puzzle grabbed by the wand	
	public Transform previousTransform = null;
	protected bool is_hand_closed_previous_frame = false;

	public HandType get_handtype() { return handType; }
	public GrabbableObject get_object_in_hand() { return object_in_hand; }
	public WandObject get_wand_in_hand() { return wand_in_hand; }
	public AudioClip fireballNoise;
	public AudioClip failtoshootNoise;
	public AudioClip chargeNoise;
	public GameObject fireballPrefab;
	private float fireballSpeed = 8f;

	private Queue<Vector3> previousControllerPositions = new Queue<Vector3>();
	private int numPreviousPositionsToTrack = 15;
	private Vector3 previousControllerPosition;
	private float throwForce;

	void Start()
	{
		// Prevent multiple fetches of anchors
		if (anchors_in_the_scene == null) anchors_in_the_scene = GameObject.FindObjectsOfType<GrabbableObject>();

		//in case of scene changing
		UpdateAnchorsList();
	}

	static private void UpdateAnchorsList()
	{
		// Find all objects in the scene containing an ObjectAnchor component
		GrabbableObject[] anchors = GameObject.FindObjectsOfType<GrabbableObject>();

		// Update the anchors_in_the_scene list with these objects
		anchors_in_the_scene = anchors;
	}

	void Update()
	{
		if (object_in_hand != null) update_throwing_speed();


		bool hand_closed = is_hand_closed();

		if (hand_closed == is_hand_closed_previous_frame) ;//hand stays closed or opened-->do nothing


		else if (hand_closed && object_in_hand == null && wand_in_hand == null)//hand gets closed
		{
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;
			for (int i = 0; i < anchors_in_the_scene.Length; i++)
			{
				if (!anchors_in_the_scene[i].is_available()) continue;
				oject_distance = Vector3.Distance(this.transform.position, anchors_in_the_scene[i].transform.position);
				if (oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius())
				{
					best_object_id = i;
					best_object_distance = oject_distance;
				}
			}
			if (best_object_id != -1 && anchors_in_the_scene[best_object_id].get_my_type() != GrabbableObject.ObjectType.Wand) //check whether best object is NOT a wand
			{
				object_in_hand = anchors_in_the_scene[best_object_id];
				Debug.LogWarningFormat("{0} grasped {1}", this.transform.parent.name, object_in_hand.name);

				Rigidbody rigidbody = object_in_hand.GetComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				object_in_hand.attach_to(this);

				trigger_pulse();

				wand_in_hand = (WandObject)object_in_hand;
			}

		}
		else if (!hand_closed && object_in_hand != null && wand_in_hand == null) // hand gets opened->release object
		{
			Rigidbody rigidbody = object_in_hand.GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			Vector3 throwDirection = Camera.main.transform.forward.normalized;
			rigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
			object_in_hand.detach_from(this);


			trigger_pulse();

			object_in_hand = null;
		}
		is_hand_closed_previous_frame = hand_closed;//update hand closed from previous frame



		if (try_to_release_object_in_hand() && wand_in_hand != null)//release wand
		{
			Rigidbody rigidbody = object_in_hand.GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			Vector3 throwDirection = Camera.main.transform.forward.normalized;
			rigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
			object_in_hand.detach_from(this);

			trigger_pulse();

			object_in_hand = null;
			wand_in_hand.set_visibleRay(false);
			wand_in_hand = null;

		}

		if (try_to_grab_object() && wand_in_hand == null)//grab wand
		{
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;
			for (int i = 0; i < anchors_in_the_scene.Length; i++)
			{
				if (!anchors_in_the_scene[i].is_available()) continue;
				oject_distance = Vector3.Distance(this.transform.position, anchors_in_the_scene[i].transform.position);
				if (oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius())
				{
					best_object_id = i;
					best_object_distance = oject_distance;
				}
			}
			if (best_object_id != -1 && anchors_in_the_scene[best_object_id].get_my_type() == GrabbableObject.ObjectType.Wand) //check whether best object is a wand
			{
				object_in_hand = anchors_in_the_scene[best_object_id];
				Debug.LogWarningFormat("{0} grasped {1}", this.transform.parent.name, object_in_hand.name);

				Rigidbody rigidbody = object_in_hand.GetComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				object_in_hand.attach_to(this);

				trigger_pulse();

				wand_in_hand = (WandObject)object_in_hand;
			}
		}


		if (wand_in_hand != null)//wand interactions
		{
			wand_in_hand.set_visibleRay(visible_ray());//make ray visible if index trigger is pressed


			if (!wand_in_hand.is_charged() && try_to_charge_wand())
			{
				wand_in_hand.charge();//charge wand
				AudioSource.PlayClipAtPoint(chargeNoise, transform.position);
			}


			if (activate_wand() && puzzle == null)//grab puzzle
			{

				InteractiveObject interactiveObject = wand_in_hand.get_interactiveObject();//consider object pointed at by object_in_hand
				if (interactiveObject != null && interactiveObject.get_my_type() == InteractiveObject.ObjectType.Puzzle)//if it's a puzzle then proceed
				{
					puzzle = (PuzzleObject)interactiveObject;
					puzzle.attach_to_wand(this);
					start_vibration();

				}
				else if (wand_in_hand.is_charged())//shoot fireball
				{
					GameObject fireball = Instantiate(fireballPrefab, transform.position, transform.rotation);
					Rigidbody rb = fireball.GetComponent<Rigidbody>();
					rb.velocity = transform.forward * fireballSpeed;
					AudioSource.PlayClipAtPoint(fireballNoise, transform.position);

					wand_in_hand.uncharge();
				}
				else AudioSource.PlayClipAtPoint(failtoshootNoise, transform.position);

			}
			if (puzzle != null && try_to_release_puzzle())//release puuzle
			{
				puzzle.detach_from_wand(this);
				puzzle = null;

				stop_vibration();

			}
		}
		previousTransform = transform;


	}

	protected void trigger_pulse()
	{
		if (handType == HandType.LeftHand)
		{
			HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.LTouch);
		}
		else
		{
			HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.RTouch);
		}
	}
	protected void stop_vibration()
	{
		if (handType == HandType.LeftHand)
		{
			HapticsController.singleton.StopVibration(OVRInput.Controller.LTouch);
		}
		else
		{
			HapticsController.singleton.StopVibration(OVRInput.Controller.RTouch);
		}
	}
	protected void start_vibration()
	{
		if (handType == HandType.LeftHand)
		{
			HapticsController.singleton.StartVibration(OVRInput.Controller.LTouch);
		}
		else
		{
			HapticsController.singleton.StartVibration(OVRInput.Controller.RTouch);
		}
	}
	protected bool is_hand_closed()
	{
		if (handType == HandType.LeftHand) return
		  OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5     // Check that the middle finger is pressing
		  && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5;   // Check that the index finger is pressing
		else return
			OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5   // Check that the middle finger is pressing
			&& OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5; // Check that the index finger is pressing
	}

	protected void update_throwing_speed()
	{
		previousControllerPositions.Enqueue(transform.position);
		while (previousControllerPositions.Count > numPreviousPositionsToTrack)
		{
			previousControllerPositions.Dequeue();
		}
		Vector3 averageControllerDisplacement = Vector3.zero;
		foreach (Vector3 position in previousControllerPositions)
		{
			averageControllerDisplacement += position - previousControllerPosition;
		}
		averageControllerDisplacement /= previousControllerPositions.Count;
		float controllerSpeed = averageControllerDisplacement.magnitude / Time.deltaTime;
		previousControllerPosition = transform.position;
		throwForce = (float)(controllerSpeed * 0.5);
	}
	protected bool try_to_grab_object() //grabbing object_in_hand
	{
		if (handType == HandType.LeftHand) return OVRInput.Get(OVRInput.Button.Three);
		else return OVRInput.Get(OVRInput.Button.One);
	}
	protected bool try_to_release_object_in_hand()//releasing object_in_hand
	{
		if (handType == HandType.LeftHand) return
		  OVRInput.Get(OVRInput.Button.Four);
		else return
			OVRInput.Get(OVRInput.Button.Two);
	}
	protected bool try_to_grab_puzzle()//grab puzzle piece
	{
		if (wand_in_hand != null)// && wand_in_hand.is_charged())
		{
			if (handType == HandType.LeftHand)
			{
				return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);
			}
			else
			{
				return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
			}
		}
		else
		{
			return false;
		}
	}
	protected bool try_to_release_puzzle()//release puzzle piece
	{
		if (handType == HandType.LeftHand && wand_in_hand.is_charged())
		{
			return !(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger));
		}
		else
		{
			return !(OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger));
		}

	}
	protected bool try_to_charge_wand()//charge wand
	{
		if (handType == HandType.LeftHand) return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
		else return OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
	}
	protected bool activate_wand()//light torch
	{
		if (handType == HandType.LeftHand) return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);
		else return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
	}
	protected bool visible_ray() //make ray visible
	{
		if (handType == HandType.LeftHand) return
		  OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
		else return
			OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
	}
}