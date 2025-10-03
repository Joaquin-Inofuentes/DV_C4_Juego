using UnityEngine;

public class F_Factory : MonoBehaviour
{
    public GameObject projectilePrefab;

    public GameObject CreateProjectile(Vector3 spawnPosition)
    {
        GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        proj.SetActive(false);
        return proj;
    }
}
