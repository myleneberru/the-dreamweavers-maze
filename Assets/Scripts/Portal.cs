using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public GameObject portal;  // The portal GameObject to be activated
    public bool redCarDone = false;  // Flag to check if the red car task is done
    public bool wineglassDone = false;  // Flag to check if the wineglass task is done
    public AudioClip portalNoise;  // The sound to play when the portal is activated

    // Called at the start of the script
    void Start()
    {
        portal.SetActive(false);  // Initially set the portal to inactive
    }

    // Called once per frame
    void Update()
    {
        if (redCarDone && wineglassDone)
        {
            StartCoroutine(activate());  // Start the coroutine to activate the portal
        }
    }

    // Coroutine to activate the portal after a delay
    IEnumerator activate()
    {
        yield return new WaitForSeconds(2);  // Wait for 2 seconds
        AudioSource.PlayClipAtPoint(portalNoise, transform.position);  // Play the portal activation sound
        portal.SetActive(true);  // Activate the portal GameObject
    }
}
