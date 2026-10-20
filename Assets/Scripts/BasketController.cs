using UnityEngine;
using System.Collections.Generic;

public class BasketController : MonoBehaviour
{
    public Transform basket; // Reference to the basket object
    private List<Transform> objectsInBasket = new List<Transform>(); // List to hold objects in the basket
    private ThrowableObject basketThrowable; // Reference to the ThrowableObject script attached to the basket
    private Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>(); // Dictionary to store original scales of objects


    void Start()
    {
        basketThrowable = this.GetComponent<ThrowableObject>(); // Get the ThrowableObject component attached to the basket

    }


    void OnTriggerEnter(Collider other)
    {
        // Check if the object is not already in the list and it's not the player
        if (!objectsInBasket.Contains(other.transform) && (other.CompareTag("Pumpkin") || other.CompareTag("Watermelon") || other.CompareTag("Yellow_Mushroom") || other.CompareTag("Purple_Mushroom") || other.CompareTag("Crystal") || other.CompareTag("Apple")))
        {
            // Add the object to the list
            objectsInBasket.Add(other.transform);

            // Save the original scale of the object
            originalScales[other.transform] = other.transform.localScale;

            // If the basket is attached, set the object as a child of the basket
            if (IsBasketAttached())
            {
                other.transform.SetParent(basket);
            }
        }
    }

    void Update()
    {
        if (basketThrowable != null)
        {
            if (IsBasketAttached())
            {

                // If the basket is attached, attach the objects to the basket
                foreach (var obj in objectsInBasket)
                {
                    if (obj != null)
                    {
                        obj.transform.SetParent(basket);
                    }
                }
            }
            else
            {
                // If the basket is no longer attached, detach the objects from the basket
                foreach (var obj in objectsInBasket)
                {
                    if (obj != null && obj.parent == basket)
                    {
                        // Reassign the original scale of the object
                        obj.SetParent(null);
                        obj.transform.localScale = originalScales[obj.transform];
                    }
                }
            }

            // Check if the basket is rotated
            if (IsRotated())
            {
                // If the basket is rotated, detach all child objects from the basket
                foreach (var obj in objectsInBasket)
                {
                    if (obj != null && obj.parent == basket)
                    {
                        obj.SetParent(null);
                        obj.transform.localScale = originalScales[obj.transform];
                    }
                }
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        // Check if the object is in the list
        if (objectsInBasket.Contains(other.transform))
        {
            // Remove the object from the list
            objectsInBasket.Remove(other.transform);

            // If the object is still a child of the basket, detach it and reassign its original scale
            if (other.transform.parent == basket)
            {
                other.transform.SetParent(null);
                other.transform.localScale = originalScales[other.transform];
            }
        }
    }


    private bool IsBasketAttached()
    {
        // Return the attachment state of the basket
        return basketThrowable != null && basketThrowable.get_is_attached();
    }

    // Function to check if the object is rotated
    private bool IsRotated()
    {
        Quaternion localRotation = this.transform.rotation;

        Vector3 localUp = localRotation * Vector3.up;

        Vector3 globalDown = Vector3.down;

        float dotProduct = Vector3.Dot(localUp, globalDown);

        return Mathf.Abs(dotProduct) < 0.8f;
    }
}