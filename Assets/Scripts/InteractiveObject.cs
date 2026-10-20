using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    // Enum for different types of interactive objects
    public enum ObjectType { Torch, Puzzle, Destructible }
    protected ObjectType myType;  // The type of this interactive object
    private bool isSelected = false;  // Flag to track if the object is selected
    private Renderer objectRenderer;  // Reference to the Renderer component

    // Get the type of this interactive object
    public ObjectType get_my_type() { return myType; }

    // Placeholder method to toggle visibility (implementation not provided)
    public void toggle_visibility() { }

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();  // Get the Renderer component attached to this object
    }

    // Select this object and apply a glow effect
    public void SelectObject()
    {
        isSelected = true;  // Set the selected flag to true
        ApplyGlowEffect();  // Apply the glow effect
    }

    // Deselect this object and remove the glow effect
    public void DeselectObject()
    {
        isSelected = false;  // Set the selected flag to false
        RemoveGlowEffect();  // Remove the glow effect
    }

    // Apply a glow effect to this object
    private void ApplyGlowEffect()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.SetColor("_EmissionColor", Color.white * 0.5f);  // Set the emission color to a dim white
        }
    }

    // Remove the glow effect from this object
    private void RemoveGlowEffect()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.SetColor("_EmissionColor", Color.black);  // Set the emission color to black (no emission)
        }
    }
}
