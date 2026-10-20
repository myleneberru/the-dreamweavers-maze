using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishController : MonoBehaviour
{
    public LightController lightPoint;
    public AudioSource bellsSound;

    // Play bell sound if Car collides 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            lightPoint.SetTriggered();
            bellsSound.Play();
        }
        
    }
   
}
