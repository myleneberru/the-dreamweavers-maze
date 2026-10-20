using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LavaCollision : MonoBehaviour
{
    public AudioSource collisionSound;
    public float fadeDuration = 1.0f;
    public Material fadeMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lava"))
        {
            collisionSound.Play(); // Play sound 
            HapticsController.singleton.TriggerHapticFeedback(1f, 5f, 1f, OVRInput.Controller.RTouch); // Trigger haptic feedback for both hands 
            HapticsController.singleton.TriggerHapticFeedback(1f, 5f, 1f, OVRInput.Controller.LTouch);

            StartCoroutine(FadeOutAndLoad("FinalCave"));
        }
    }
    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        // Create a fade screen 
        GameObject fadeScreen = new GameObject("FadeScreen");
        fadeScreen.AddComponent<MeshRenderer>().material = fadeMaterial;
        fadeScreen.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        fadeScreen.transform.LookAt(Camera.main.transform);
        fadeScreen.transform.localScale = new Vector3(10f, 10f, 0.01f);

        // Make the fade screen disappear 
        float timer = 0f;
        while (timer < fadeDuration)
        {
            fadeMaterial.color = new Color(0, 0, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Load new scene
        SceneManager.LoadScene(sceneName);

        // Destroy the fade screen
        Destroy(fadeScreen);
    }
}