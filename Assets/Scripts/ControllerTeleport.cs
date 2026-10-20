using UnityEngine;

public class ControllerTeleport : MonoBehaviour
{
    [Header("Controller Anchors")]
    public Transform leftHandAnchor;  // Reference to the left hand controller anchor
    public Transform rightHandAnchor; // Reference to the right hand controller anchor

    [Header("Maximum Distance")]
    [Range(2f, 30f)]
    public float maximumTeleportationDistance = 15f;  // Max distance for teleportation 

    [Header("Marker")]
    public GameObject markerPrefab;  // Prefab used for marking the teleportation target
    private GameObject markerPrefabInstance;  // Instance of the marker prefab

    private CharacterController characterController;  // Character controller component for player 
    private LineRenderer lineRenderer;  // Component for rendering curved teleportation lines
    public int curveSegments = 10;  // Number of segments for the curve, more segments = smoother curve

    private bool aiming = false;  // Flag to check if the player is aiming

    [Header("Teleportation Sound")]
    public AudioClip teleportSound;  // Audio clip for teleportation sound
    private AudioSource audioSource;  // Audio source component for playing the sound

    void Start()
    {
        // Get the character controller component
        characterController = GetComponent<CharacterController>();

        // Initialize the line renderer for teleportation ray
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = curveSegments + 1;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Shader for color
        lineRenderer.startColor = Color.blue;  // Set the start color to blue
        lineRenderer.endColor = Color.blue;    // Set the end color to blue
        lineRenderer.enabled = false;  // Initially disable the line renderer

        // Initialize the audio source for teleportation sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = teleportSound;
    }

    void Update()
    {
        // Check if controller triggers are pressed
        bool leftTriggerPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch);
        bool rightTriggerPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);

        // Right hand aiming process
        if (rightTriggerPressed)
        {
            Vector3 targetPoint;

            // Aim with the right hand, show marker and draw arc if aiming is successful
            if (AimWith(rightHandAnchor, out targetPoint))
            {
                ShowMarkerAt(targetPoint);  // Show the teleportation marker at the target point
                DrawArc(rightHandAnchor.position, targetPoint);  // Draw a curved line from hand to target
                aiming = true;
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;  // Disable line renderer if not aiming successfully
                RemoveMarker();  // Cleanup marker if not aiming successfully
                aiming = false;  // Reset aiming state
            }
        }
        else if (aiming)
        {
            lineRenderer.enabled = false;  // Disable line renderer when not aiming
            RemoveMarker();  // Cleanup marker if right trigger is released
            aiming = false;  // Reset aiming state
        }

        // Execute teleportation if both triggers are pressed and player is aiming and the target point is not on a non-teleportable layer
        if (leftTriggerPressed && rightTriggerPressed && aiming && markerPrefabInstance != null && !IsOnNotTeleportableLayer(markerPrefabInstance))
        {
            Vector3 teleportPosition = markerPrefabInstance.transform.position;
            TeleportTo(teleportPosition);  // Teleport player
            aiming = false;  // Reset aiming state
            lineRenderer.enabled = false;  // Disable line renderer post teleportation
            RemoveMarker();  // Cleanup marker from the scene
        }
    }

    private void DrawArc(Vector3 start, Vector3 end)
    {
        // Determine the control point for the arc, slightly elevated
        Vector3 controlPoint = (start + end) * 0.5f + Vector3.up * 1;

        //draw the arc
        for (int i = 0; i <= curveSegments; i++)
        {
            float t = i / (float)curveSegments;
            Vector3 position = QuadraticBezierCurve(start, controlPoint, end, t);
            lineRenderer.SetPosition(i, position);
        }
    }

    private Vector3 QuadraticBezierCurve(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        // Calculate position on the quadratic Bezier curve 
        return Mathf.Pow(1 - t, 2) * start + 2 * t * (1 - t) * control + Mathf.Pow(t, 2) * end;
    }

    protected bool AimWith(Transform handTransform, out Vector3 targetPoint)
    {
        targetPoint = new Vector3();
        RaycastHit hit;
        // Perform a raycast from the hand position forward up to the max distance
        if (!Physics.Raycast(handTransform.position, handTransform.forward, out hit, maximumTeleportationDistance)) return false;
        if (hit.distance > maximumTeleportationDistance) return false;  // Check if hit is within max distance

        targetPoint = hit.point;  // Set target point to hit location

        // Check if the hit object is on the layer notteleportable
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("notteleportable"))
        {
            lineRenderer.startColor = Color.red;  // Change line color to red
            lineRenderer.endColor = Color.red;    // Change line color to red
        }
        else
        {
            lineRenderer.startColor = Color.blue;  // Change line color to blue
            lineRenderer.endColor = Color.blue;    // Change line color to blue
        }

        return true;
    }

    // Check if the object is on the layer notteleportable
    private bool IsOnNotTeleportableLayer(GameObject obj)
    {
        if (obj == null) return false;
        return obj.CompareTag("notteleportable");
    }

    // Show marker at position of aim 
    private void ShowMarkerAt(Vector3 position)
    {
        if (markerPrefabInstance == null)
        {
            markerPrefabInstance = Instantiate(markerPrefab, position, Quaternion.identity);
        }
        else
        {
            // Adjust the marker position to be exactly on the ground (terrain or plane)
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 5, Vector3.down, out hit, 10))
            {
                position = hit.point;
            }
            markerPrefabInstance.transform.position = position;
        }
    }

    // Remove marker 
    private void RemoveMarker()
    {
        if (markerPrefabInstance != null)
        {
            Destroy(markerPrefabInstance);
            markerPrefabInstance = null;
        }
    }

    private void TeleportTo(Vector3 position)
    {
        // Ensure the position is valid by checking for collisions before moving
        if (!Physics.CheckCapsule(position, position + Vector3.up * characterController.height, characterController.radius, LayerMask.GetMask("notteleportable")))
        {
            characterController.enabled = false;  // Disable the character controller to avoid conflicts
            this.transform.position = position + Vector3.up * (characterController.height / 2);   // Directly set the position slightly above the ground
            characterController.enabled = true;   // Re-enable the character controller
            audioSource.Play();  // Play the teleportation sound
        }
    }
}