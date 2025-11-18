using CustomInspector;
using UnityEngine;

public class C_Shooter : MonoBehaviour, I_Interactuar
{
    [Button(nameof(Interactuar))]
    public Transform ShootPoint;
    public int DamageProyectil = 25;
    public float SpeedProyectil = 30f;
    public string Proyectil = "proyectil";
    public void Interactuar()
    {
        Debug.Log("[C_Shooter] Disparar!", gameObject);
        Vector3 spawnPos = transform.position + transform.forward * 2f;

        // Pedir proyectil
        GameObject proj = C_PoolManager.Instance.Request(Proyectil, spawnPos);

        // Opcional: si tenés C_Projectile con Init(model)
        var controller = proj.GetComponent<C_Projectile>();
        if (controller != null)
        {
            M_Projectile modelData = new M_Projectile()
            {
                Damage = DamageProyectil,
                Speed = SpeedProyectil,
                Direction = transform.forward,
                Owner = gameObject
            };
            controller.Init(modelData);
        }
    }

    public void Recargar()
    {
        Debug.Log("[C_Shooter] Recargar!", gameObject);
    }
}
