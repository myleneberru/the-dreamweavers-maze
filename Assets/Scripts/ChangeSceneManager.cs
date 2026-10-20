using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    public string SceneName;  // The name of the scene to load
    public float fadeDuration = 1.0f;  // Duration of the fade effect in seconds
    public Material fadeMaterial;  // Material used for the fade effect

    public static ChangeSceneManager singleton;  // Singleton instance of the ChangeSceneManager

    void Start()
    {
        // Ensure there is only one instance of the ChangeSceneManager
        if (singleton && singleton != this)
            Destroy(this);  // Destroy this instance if another exists
        else
            singleton = this;  // Assign this as the singleton instance
    }

    public void ChangeScene()
    {
        // Start the coroutine to fade out and load the new scene
        StartCoroutine(FadeOutAndLoad(SceneName));
    }

    // Coroutine for the fade-out effect and scene loading
    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        // Create a fade screen
        GameObject fadeScreen = new GameObject("FadeScreen");
        fadeScreen.AddComponent<MeshRenderer>().material = fadeMaterial;  // Add a MeshRenderer and set its material to fadeMaterial
        fadeScreen.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;  // Position it in front of the camera
        fadeScreen.transform.LookAt(Camera.main.transform);  // Make the fade screen face the camera
        fadeScreen.transform.localScale = new Vector3(10f, 10f, 0.01f);  // Set the scale of the fade screen

        // Gradually increase the alpha of the fade material to create a fade-out effect
        float timer = 0f;
        while (timer < fadeDuration)
        {
            fadeMaterial.color = new Color(0, 0, 0, timer / fadeDuration);  // Adjust the alpha value
            timer += Time.deltaTime;  // Increment the timer by the time elapsed since the last frame
            yield return null;  // Wait for the next frame
        }

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Destroy the fade screen after the new scene is loaded
        Destroy(fadeScreen);
    }
}

