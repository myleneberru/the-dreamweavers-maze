using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public float rangeIncreaseRate = 1f; // Rate at which the range increases per second
    public float intensityIncreaseRate = 0.1f; // Rate at which the intensity increases per second
    public float targetRange = 100f; // Target range for the point light
    public float targetIntensity = 4f; // Target intensity for the point light
    private bool triggered = false;
    private Light pointLight;


    private void Start()
    {
        pointLight = GetComponent<Light>();
    }
    void Update()
    {
        if (triggered)
            IncreaseLight();
    }

    void IncreaseLight()
    {
        // Gradually increase the range
        if (pointLight.range < targetRange)
        {
            float newRange = Mathf.Min(pointLight.range + (rangeIncreaseRate * Time.deltaTime), targetRange);
            pointLight.range = newRange;
        }

        // Gradually increase the intensity
        if (pointLight.intensity < targetIntensity)
        {
            float newIntensity = Mathf.Min(pointLight.intensity + (intensityIncreaseRate * Time.deltaTime), targetIntensity);
            pointLight.intensity = newIntensity;
        }
    }

    public void SetTriggered()
    {
        triggered = true;
    }
}
