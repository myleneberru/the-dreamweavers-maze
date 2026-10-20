using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource fallSound;

    // Play sound if object enter in collision 
    private void OnCollisionEnter(Collision collision)
    {
        fallSound.Play();
    }
}
