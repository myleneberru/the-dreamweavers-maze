using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LightsOff : MonoBehaviour
{
    void Start()
    {
        // Find all directional lights in the scene
        Light[] directionalLights = FindObjectsOfType<Light>();

        // Iterate through each light
        foreach (Light light in directionalLights)
        {
            // Check if the light is a directional light
            if (light.type == LightType.Directional)
            {
                // Disable the directional light
                light.enabled = false;
            }
        }
    }
}