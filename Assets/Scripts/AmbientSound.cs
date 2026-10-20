
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip soundClip; // Assign the sound clip in the inspector

    private AudioSource audioSource;
	void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.loop = true; // Set loop to true
		audioSource.Play();
    }

}
