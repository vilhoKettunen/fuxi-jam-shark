using System;
using System.Collections.Generic;
using UnityEngine;

public class GateHandler : MonoBehaviour
{
    [Header("Drag & drop GameObjects here")]
    [Tooltip("Coin GameObjects (prefab instances). If they have a Coin component it will be used; otherwise a CoinProxy will be added automatically.")]
    public List<GameObject> coins = new List<GameObject>();

    [Tooltip("Gate GameObjects. If they have a Gate component it will be used; otherwise the GameObject will be deactivated when opened.")]
    public List<GameObject> gates = new List<GameObject>();

    // runtime state
    private HashSet<GameObject> collected = new HashSet<GameObject>();
    private bool opened = false;

    // keep subscriptions so we can unsubscribe cleanly
    private Dictionary<Coin, Action> coinSubscriptions = new Dictionary<Coin, Action>();

    private void Start()
    {
        collected.Clear();
        opened = false;
        coinSubscriptions.Clear();

        foreach (var coinGO in coins)
        {
            if (coinGO == null) continue;

            var coinComp = coinGO.GetComponent<Coin>();
            if (coinComp != null)
            {
                // create dedicated callback so we can unsubscribe later
                GameObject localGO = coinGO;
                Action callback = () => OnCoinCollected(localGO);
                coinComp.OnCollected += callback;
                coinSubscriptions.Add(coinComp, callback);
            }
            else
            {
                // ensure collider exists and is trigger
                var col = coinGO.GetComponent<Collider>();
                if (col == null)
                {
                    Debug.LogWarning($"GateHandler: coin '{coinGO.name}' has no Collider. Adding SphereCollider (isTrigger=true).", coinGO);
                    var sc = coinGO.AddComponent<SphereCollider>();
                    sc.isTrigger = true;
                }
                else if (!col.isTrigger)
                {
                    Debug.LogWarning($"GateHandler: coin '{coinGO.name}' Collider is not a trigger — setting isTrigger = true.", coinGO);
                    col.isTrigger = true;
                }

                // add or reuse CoinProxy which will call back to this handler
                var proxy = coinGO.GetComponent<CoinProxy>();
                if (proxy == null)
                    proxy = coinGO.AddComponent<CoinProxy>();
                proxy.Initialize(this);
            }
        }
    }

    // Called by Coin component (via event) or by CoinProxy
    public void OnCoinCollected(GameObject coinGO)
    {
        if (opened) return;
        if (coinGO == null) return;
        if (collected.Contains(coinGO)) return;

        collected.Add(coinGO);
        Debug.Log($"GateHandler: coin collected ({collected.Count}/{coins.Count})");

        if (collected.Count >= coins.Count)
        {
            OpenAllGates();
        }
    }

    private void OpenAllGates()
    {
        opened = true;
        Debug.Log("GateHandler: All coins collected! Opening gates...");

        foreach (var gateGO in gates)
        {
            if (gateGO == null) continue;

            var gateComp = gateGO.GetComponent<Gate>();
            if (gateComp != null)
            {
                gateComp.OpenGate();
            }
            else
            {
                // Fallback: disable collider and deactivate object
                var col = gateGO.GetComponent<Collider>();
                if (col != null) col.enabled = false;
                gateGO.SetActive(false);
                Debug.Log($"GateHandler: gate '{gateGO.name}' had no Gate component — deactivated as fallback.");
            }
        }

        OnGatesOpened();
    }

    // Activation hook — currently just logs; change this to call other systems
    private void OnGatesOpened()
    {
        Debug.Log("GateHandler: OnGatesOpened() called — activation hook.");
        // Put your custom activation logic here or override this class in derived class
    }

    private void OnDestroy()
    {
        // Unsubscribe all coin event handlers we registered
        foreach (var kv in coinSubscriptions)
        {
            if (kv.Key != null)
                kv.Key.OnCollected -= kv.Value;
        }
        coinSubscriptions.Clear();
    }

    // helper for quick reset if you want to reuse the handler at runtime
    public void ResetHandler()
    {
        collected.Clear();
        opened = false;
        // optionally re-enable coins/gates etc.
    }
}