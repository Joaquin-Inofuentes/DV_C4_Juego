using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class M_Pool
{
    [SerializeField] private List<GameObject> activeObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> inactiveObjects = new List<GameObject>();

    public void AddActive(GameObject obj) => activeObjects.Add(obj);
    public void RemoveActive(GameObject obj) => activeObjects.Remove(obj);
    public void AddInactive(GameObject obj) => inactiveObjects.Add(obj);
    public void RemoveInactive(GameObject obj) => inactiveObjects.Remove(obj);

    public bool HasAvailable() => inactiveObjects.Count > 0;
    public int AvailableCount() => inactiveObjects.Count;

    public GameObject GetInactiveAt(int index) => inactiveObjects[index];
    public void RemoveInactiveAt(int index) => inactiveObjects.RemoveAt(index);
}
