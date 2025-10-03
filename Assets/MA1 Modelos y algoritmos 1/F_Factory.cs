using UnityEngine;

public class F_Factory : MonoBehaviour
{
    public GameObject prefab;

    public GameObject Create(Vector3 spawnPosition)
    {
        GameObject obj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        obj.SetActive(false);
        return obj;
    }
}
