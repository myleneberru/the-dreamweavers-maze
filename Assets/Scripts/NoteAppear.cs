using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteAppear : MonoBehaviour
{
    [SerializeField]
    private GameObject noteImage;  // The GameObject representing the note image

    // Triggered when another collider enters this object's collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Make the note image visible and attach it to the player's camera
            DisplayNote();
        }
    }

    // Triggered when another collider exits this object's collider
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Deactivate the note image and detach it from the player's camera
            HideNote();
        }
    }

    // Method to display the note image
    private void DisplayNote()
    {
        noteImage.SetActive(true);  // Activate the GameObject containing the note
    }

    // Method to hide the note image
    private void HideNote()
    {
        noteImage.SetActive(false);  // Deactivate the GameObject containing the note
    }
}

