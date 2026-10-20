using System.Collections;
using UnityEngine;

public class PotionManager : ThrowableObject

	
{
    // Audio source to play the drinking sound
    public AudioSource drinking;


    // Update is called once per frame
    void Update()
    {
        if (IsRotated())
        {
            if (drinking != null)
            {
                // Play the drinking sound
                drinking.Play();
                // Start a coroutine to wait and change the scene after the sound finishes
                StartCoroutine(WaitAndChangeScene(drinking.clip.length));
            }
            else
            {
                // Change the scene instantaneously if there's no sound
                ChangeSceneManager.singleton.ChangeScene();
            }
        }
    }


    // Coroutine to wait for a delay before changing the scene
    private IEnumerator WaitAndChangeScene(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        // Change the scene using the singleton manager
        ChangeSceneManager.singleton.ChangeScene();
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

