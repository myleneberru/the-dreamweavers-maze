using UnityEngine;

public class CauldronController : MonoBehaviour
{
    public GameObject potion;
    public Transform potionSpawnPoint;
    public AudioSource wrongIngredientSource;
    public AudioSource cauldronAudioSource;
    public AudioSource potionAudioSource;

    private bool hasPumpkin = false;
    private bool hasCrystal = false;
    private bool hasPurple_Mushroom = false;
    private bool hasApple = false;
    private HandController handcontroller;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pumpkin") && !hasPumpkin)
        {
            hasPumpkin = true;
            other.gameObject.SetActive(false);

            if (cauldronAudioSource != null)
            {
                cauldronAudioSource.Play();
            }
        }
        else if (other.CompareTag("Crystal") && !hasCrystal)
        {
            hasCrystal = true;
            other.gameObject.SetActive(false);

            if (cauldronAudioSource != null)
            {
                cauldronAudioSource.Play();
            }
        }
        else if (other.CompareTag("Purple_Mushroom") && !hasPurple_Mushroom)
        {
            hasPurple_Mushroom = true;
            other.gameObject.SetActive(false);

            if (cauldronAudioSource != null)
            {
                cauldronAudioSource.Play();
            }
        }
        else if (other.CompareTag("Apple") && !hasApple)
        {
            hasApple = true;
            other.gameObject.SetActive(false);

            if (cauldronAudioSource != null)
            {
                cauldronAudioSource.Play();
            }
        }
        else
        {
            PlayWrongIngredientSound();
        }

        if (hasPumpkin && hasCrystal && hasPurple_Mushroom && hasApple)
        {
            if (potionAudioSource != null)
            {
                potionAudioSource.Play();
            }

            potion.transform.position = potionSpawnPoint.position;
            potion.SetActive(true);

            HandController[] handcontrollers = GameObject.FindObjectsOfType<HandController>();
            foreach (HandController handcontroller in handcontrollers)
            {
                handcontroller.UpdateAnchorsList();
            }
        }
    }

    private void PlayWrongIngredientSound()
    {
        if (wrongIngredientSource != null)
        {
            wrongIngredientSource.Play();
        }
    }
}