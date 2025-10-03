using UnityEngine;

public class C_ProjectilePool : MonoBehaviour
{
    public static C_ProjectilePool Instance;

    public F_Factory Factory;
    public M_Pool Model = new M_Pool();

    void OnEnable() => Instance = this;

    // Pide proyectil con modelo personalizado
    public GameObject RequestProjectile(M_Projectile modelData, Vector3 spawnPosition)
    {
        GameObject proj;

        if (Model.HasAvailable())
        {
            proj = Model.GetInactiveAt(0);
            Model.RemoveInactiveAt(0);
        }
        else
        {
            proj = Factory.CreateProjectile(spawnPosition);
            Debug.Log("[Pool] Nuevo proyectil creado por Factory");
        }

        var controller = proj.GetComponent<C_Projectile>();
        controller.Init(modelData);

        proj.transform.position = spawnPosition;
        proj.transform.SetParent(transform);
        proj.SetActive(true);

        Model.AddActive(proj);

        return proj;
    }

    // Pide proyectil con valores por defecto
    public GameObject RequestProjectileDefault(Vector3 spawnPosition)
    {
        M_Projectile defaultModel = new M_Projectile();
        return RequestProjectile(defaultModel, spawnPosition);
    }

    // Devuelve proyectil al pool
    public void ReturnProjectile(GameObject proj)
    {
        proj.GetComponent<C_Projectile>().ResetState();
        proj.SetActive(false);

        Model.RemoveActive(proj);
        Model.AddInactive(proj);
    }

    // Métodos de consulta
    public bool HasAvailable() => Model.HasAvailable();
    public int AvailableCount() => Model.AvailableCount();
}
