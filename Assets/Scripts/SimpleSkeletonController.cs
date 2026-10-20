using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSkeletonController : MonoBehaviour
{
    Animator animator;  // Reference to the Animator component
    private float activationDistance = 2f;  // Distance within which the skeleton is activated
    private bool has_fallen = false;  // Flag to check if the skeleton has fallen
    public AudioClip skeletonNoise;  // Audio clip to play when the skeleton falls
    private OVRPlayerController player;  // Reference to the player

    SkinnedMeshRenderer skinnedMesh;  // Reference to the SkinnedMeshRenderer component

    // Enum to define the type of skeleton
    public enum SkeletonType { Type1, Type2 }
    public SkeletonType myType;  // The type of this skeleton

    public GameObject wand;  // The wand GameObject associated with the skeleton
    public GameObject chain;  // The chain GameObject associated with the skeleton
    private Rigidbody wandRigidbody;  // Rigidbody component of the wand

    // Called at the start of the script
    void Start()
    {
        if (myType == SkeletonType.Type1)
        {
            player = GameObject.FindObjectOfType<OVRPlayerController>();  // Find the player
            animator = GetComponent<Animator>();  // Get the Animator component
            wand.transform.SetParent(transform);  // Set the wand as a child of the skeleton
            wandRigidbody = wand.GetComponent<Rigidbody>();  // Get the Rigidbody component of the wand
            wandRigidbody.isKinematic = true;  // Make the wand kinematic

            // Set all Rigidbody components in the chain to be kinematic
            foreach (Transform child in chain.transform)
            {
                Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                if (childRigidbody != null)
                {
                    childRigidbody.isKinematic = true;
                }
            }
        }
        if (myType == SkeletonType.Type2)
        {
            animator.SetTrigger("lying");  // Set the skeleton to the lying state
        }
    }

    // Called once per frame
    void Update()
    {
        if (myType == SkeletonType.Type1)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);  // Calculate the distance to the player

            // Check if the player is within the activation distance and the skeleton has not fallen yet
            if (distanceToPlayer < activationDistance && !has_fallen)
            {
                has_fallen = true;  // Set the fallen flag to true
                animator.SetTrigger("knockdown");  // Trigger the knockdown animation
                AudioSource.PlayClipAtPoint(skeletonNoise, transform.position);  // Play the skeleton noise

                // Trigger haptic feedback on both controllers
                HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.2f, OVRInput.Controller.LTouch);
                HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.2f, OVRInput.Controller.RTouch);
                StartCoroutine(StartPulseAfterDelay(0.7f));  // Start a coroutine to trigger additional haptic feedback after a delay

                wand.transform.SetParent(null);  // Detach the wand from the skeleton
                wandRigidbody.isKinematic = false;  // Make the wand non-kinematic

                // Set all Rigidbody components in the chain to be non-kinematic
                foreach (Transform child in chain.transform)
                {
                    Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
                    if (childRigidbody != null)
                    {
                        childRigidbody.isKinematic = false;
                    }
                }
            }
        }
    }

    // Coroutine to trigger additional haptic feedback after a delay
    IEnumerator StartPulseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        // Trigger haptic feedback on both controllers
        HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.LTouch);
        HapticsController.singleton.TriggerHapticFeedback(0.5f, 0.5f, 0.5f, OVRInput.Controller.RTouch);
    }
}
