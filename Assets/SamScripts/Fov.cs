using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class fovi : MonoBehaviour
{
    [Header("Camera Effects")]
    public float fovZoomAmount = -15f;
    public float fovTransitionTime = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            // Add a temporary helper component that adjusts FOV each frame
            var helper = mainCam.gameObject.AddComponent<CameraFOVPermanent>();
            helper.StartZoom(fovZoomAmount, fovTransitionTime);
        }

        Destroy(gameObject);
    }
}

public class CameraFOVPermanent : MonoBehaviour
{
    private Camera cam;
    private float targetFOV;
    private float duration;
    private float startFOV;
    private float elapsed = 0f;
    private bool active = false;

    public void StartZoom(float fovChange, float zoomDuration)
    {
        cam = GetComponent<Camera>();
        if (cam == null) return;

        startFOV = cam.fieldOfView;
        targetFOV = startFOV + fovChange;
        duration = zoomDuration;
        elapsed = 0f;
        active = true;
    }

    private void Update()
    {
        if (!active) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

        if (t >= 1f)
        {
            active = false;
            Destroy(this); // Remove helper once done
        }
    }
}
