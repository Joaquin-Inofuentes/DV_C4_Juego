using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PoolEntry
{
    public string key;            // Identificador único: "Projectile", "VFX", "Enemy"
    public C_Pool pool;           // Referencia al pool
}

public class C_PoolManager : MonoBehaviour
{
    public static C_PoolManager Instance;

    [SerializeField] private List<PoolEntry> pools = new List<PoolEntry>();

    void OnEnable() => Instance = this;

    // Pide un GameObject de cualquier tipo
    public GameObject Request(string key, Vector3 position)
    {
        C_Pool pool = pools.Find(p => p.key == key)?.pool;

        if (pool != null)
        {
            return pool.Request(position);
        }
        else
        {
            Debug.LogWarning($"[PoolManager] No existe pool con key: {key}");
            return null;
        }
    }

    // Devuelve un objeto al pool correspondiente
    public void Return(GameObject obj, string key)
    {
        C_Pool pool = pools.Find(p => p.key == key)?.pool;

        if (pool != null)
            pool.Return(obj);
        else
            Debug.LogWarning($"[PoolManager] No existe pool con key: {key}");
    }
}
