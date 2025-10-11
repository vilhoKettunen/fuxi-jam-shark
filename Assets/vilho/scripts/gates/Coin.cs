using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    public bool IsCollected { get; private set; } = false;
    public event Action OnCollected;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollected) return;
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    // can be called externally too
    public void Collect()
    {
        if (IsCollected) return;
        IsCollected = true;
        gameObject.SetActive(false);
        Debug.Log($"Coin: {gameObject.name} collected.");
        OnCollected?.Invoke();
    }
}