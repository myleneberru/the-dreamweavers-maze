using UnityEngine;

public class WandObject : GrabbableObject
{
    private GameObject hitObject = null;  // The object being pointed at
    private GameObject previousHitObject = null;  // The previously pointed at object
    private InteractiveObject interactiveObject = null;  // The object being interacted with
    private float max_distance_to_target = 10;  // Maximum distance to detect targets
    private float distance_to_target = 100;  // Distance to the current target
    private bool visibleRay = false;  // Whether the ray is visible or not
    private LineRenderer lineRenderer;  // The ray used to point at objects
    public Material blueRay;  // Material for the default ray
    public Material redRay;  // Material for the ray when pointing at a torch
    public Material greenRay;  // Material for the ray when pointing at a puzzle

    public float get_distance_to_target() { return distance_to_target; }
    public GameObject get_hitObject() { return hitObject; }
    public InteractiveObject get_interactiveObject() { return interactiveObject; }

    private ParticleSystem ps;  // Particle system for visual effects
    private Light light;  // Light for visual effects

    public void activate_particles() { ps.Play(); }
    public void deactivate_particles() { ps.Stop(); }
    public void activate_light() { light.enabled = true; }
    public void deactivate_light() { light.enabled = false; }

    private int charges = 3;  // Number of charges the wand has
    public bool is_charged() { return charges > 0; }

    // Recharge the wand
    public void charge()
    {
        charges = 3;
        activate_particles();
        activate_light();
    }

    // Use a charge from the wand
    public void uncharge()
    {
        charges -= 1;
        if (charges < 1)
        {
            deactivate_particles();
            deactivate_light();
        }
    }

    public void set_visibleRay(bool visible_ray) { visibleRay = visible_ray; }

    void Start()
    {
        ps = GetComponent<ParticleSystem>();  // Get the ParticleSystem component
        light = GetComponent<Light>();  // Get the Light component
        lineRenderer = gameObject.GetComponent<LineRenderer>();  // Get the LineRenderer component
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (is_charged() && wandController != null)  // Render the ray and update hitObject if the wand is charged
        {
            DisplayRay();
            lineRenderer.enabled = true;
        }
        else
        {
            hitObject = null;  // No hit object
            lineRenderer.enabled = false;  // No line
        }

        if (hitObject != previousHitObject)  // If hitObject changed, deselect current interactiveObject and select new one
        {
            interactiveObject = hitObject?.GetComponent<InteractiveObject>();  // Update interactive object

            // Update the ray material based on the type of interactive object
            if (interactiveObject != null)
            {
                if (interactiveObject.get_my_type() == InteractiveObject.ObjectType.Torch) lineRenderer.material = redRay;
                if (interactiveObject.get_my_type() == InteractiveObject.ObjectType.Puzzle) lineRenderer.material = greenRay;
            }
            else
            {
                lineRenderer.material = blueRay;
            }
        }
        previousHitObject = hitObject;  // Update previousHitObject
    }

    // Display the ray
    public void DisplayRay()
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = wandController.transform.forward;

        RaycastHit hit;

        // Determine hitObject if any
        if (Physics.Raycast(rayOrigin, rayDirection, out hit))
        {
            distance_to_target = Vector3.Distance(hit.point, rayOrigin);
            hitObject = distance_to_target < max_distance_to_target ? hit.collider.gameObject : null;
        }
        else
        {
            hitObject = null;
        }

        // Render the line
        Vector3[] positions = { rayOrigin, rayOrigin + rayDirection.normalized * distance_to_target };
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(positions);
    }
}
