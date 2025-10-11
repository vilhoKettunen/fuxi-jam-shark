using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CoinProxy : MonoBehaviour
{
    private GateHandler handler;
    private bool collected = false;

    public void Initialize(GateHandler gateHandler)
    {
        handler = gateHandler;
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.CompareTag("Player"))
        {
            collected = true;
            gameObject.SetActive(false);
            if (handler != null) handler.OnCoinCollected(gameObject);
        }
    }
}
