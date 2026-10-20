using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PortalTuto : MonoBehaviour
{
    public string SceneName;  // Name of the scene to load
    public float fadeDuration = 0.5f;  // Duration of the fade effect
    public Material fadeMaterial;  // Material for the fade effect, ensure to set this in the Unity inspector

    // Triggered when another collider enters this object's collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start the fade-out effect and load the scene
            StartCoroutine(FadeOutAndLoad(SceneName));
        }
    }

    // Coroutine for the fade-out effect and scene loading
    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        // Create a fade screen
        GameObject fadeScreen = new GameObject("FadeScreen");
        fadeScreen.AddComponent<MeshRenderer>().material = fadeMaterial;
        fadeScreen.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        fadeScreen.transform.LookAt(Camera.main.transform);
        fadeScreen.transform.localScale = new Vector3(10f, 10f, 0.01f);

        // Gradually fade out the screen
        float timer = 0f;
        while (timer < fadeDuration)
        {
            fadeMaterial.color = new Color(0, 0, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Destroy the fade screen
        Destroy(fadeScreen);
    }
}

