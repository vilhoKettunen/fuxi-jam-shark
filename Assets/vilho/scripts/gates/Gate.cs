using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    [Tooltip("Delay before the gate is hidden after opening (in seconds). Set 0 for instant hide.")]
    public float disableDelay = 0f;

    [Header("Activation List")]
    [Tooltip("Objects that will be ACTIVATED when the gate opens.")]
    public List<GameObject> activationObjects = new List<GameObject>();

    [Header("Destruction List")]
    [Tooltip("Objects that will be DESTROYED when the gate opens.")]
    public List<GameObject> destructionObjects = new List<GameObject>();

    private bool isOpen = false;

    /// <summary>
    /// Called by GateHandler when all coins are collected
    /// </summary>
    public void OpenGate()
    {
        if (isOpen) return;
        isOpen = true;

        Debug.Log($"🚪 Gate '{gameObject.name}' opened! Triggering effects...");

        // Disable collider immediately (optional)
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Activate all objects in activation list
        ActivateObjects();

        // Destroy all objects in destruction list
        DestroyObjects();

        // Hide this gate
        if (disableDelay > 0f)
            Invoke(nameof(DisableGate), disableDelay);
        else
            DisableGate();
    }

    private void ActivateObjects()
    {
        foreach (var obj in activationObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
                Debug.Log($"🟢 Activated: {obj.name}");
            }
        }
    }

    private void DestroyObjects()
    {
        foreach (var obj in destructionObjects)
        {
            if (obj != null)
            {
                Debug.Log($"💥 Destroyed: {obj.name}");
                Destroy(obj);
            }
        }
    }

    private void DisableGate()
    {
        Debug.Log($"🚪 Gate '{gameObject.name}' is now hidden.");
        gameObject.SetActive(false);
    }
}