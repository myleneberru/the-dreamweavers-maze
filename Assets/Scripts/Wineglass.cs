using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wineglass : MonoBehaviour
{
    private GameObject liquid;  // Reference to the liquid GameObject
    private bool isObjectInside = false;  // Flag to check if the potion object is inside the wineglass
    public Portal portal;  // Reference to the portal
    public AudioClip pouringNoise;  // Audio clip to play when the potion is poured

    // Start is called before the first frame update
    void Start()
    {
        Transform liquidTransform = transform.GetChild(0);  // Get the first child transform (assumed to be the liquid)
        liquid = liquidTransform.gameObject;  // Store the liquid GameObject
        liquid.SetActive(false);  // Initially set the liquid to inactive
    }

    // Update is called once per frame
    void Update()
    {
        // Intentionally left empty
    }

    // Triggered when another collider enters this object's collider
    void OnTriggerEnter(Collider other)
    {
        GrabbableObject grabbableObject = other.GetComponent<GrabbableObject>();  // Try to get the GrabbableObject component
        if (grabbableObject != null && grabbableObject.get_my_type() == GrabbableObject.ObjectType.Potion)
        {
            isObjectInside = true;  // Set the flag to true
            StartCoroutine(CheckDotProduct(grabbableObject));  // Start the coroutine to check the dot product
        }
    }

    // Triggered when another collider exits this object's collider
    void OnTriggerExit(Collider other)
    {
        GrabbableObject grabbableObject = other.GetComponent<GrabbableObject>();  // Try to get the GrabbableObject component
        if (grabbableObject != null && grabbableObject.get_my_type() == GrabbableObject.ObjectType.Potion)
        {
            isObjectInside = false;  // Set the flag to false
        }
    }

    // Coroutine to handle the potion pouring effect
    IEnumerator ExampleCoroutine(GameObject childObject)
    {
        yield return new WaitForSeconds(1);  // Wait for 1 second
        childObject.SetActive(false);  // Deactivate the child object (potion)
        liquid.SetActive(true);  // Activate the liquid
        portal.wineglassDone = true;  // Set the flag in the portal to indicate the wineglass task is done
    }

    // Coroutine to check the dot product between the potion's up direction and the global down direction
    IEnumerator CheckDotProduct(GrabbableObject grabbableObject)
    {
        while (isObjectInside)  // Continue while the potion object is inside
        {
            Vector3 localUp = grabbableObject.transform.up;  // Get the up direction of the potion
            Vector3 globalDown = Vector3.down;  // Define the global down direction
            float dotProduct = Vector3.Dot(localUp, globalDown);  // Calculate the dot product

            // Check if the dot product indicates the potion is being poured
            if (dotProduct > 0.8)
            {
                Transform childTransform = grabbableObject.transform.GetChild(0);  // Get the first child transform of the potion
                GameObject childObject = childTransform.gameObject;  // Store the child object
                Rigidbody rb = childObject.GetComponent<Rigidbody>();  // Get the Rigidbody component of the child object
                rb.isKinematic = false;  // Make the Rigidbody non-kinematic
                childObject.transform.SetParent(null);  // Detach the child object from its parent
                AudioSource.PlayClipAtPoint(pouringNoise, transform.position);  // Play the pouring sound

                // Start the coroutine to handle the potion pouring effect
                yield return StartCoroutine(ExampleCoroutine(childObject));
            }

            yield return null;  // Wait for the next frame
        }
    }
}
