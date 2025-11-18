using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MA_P2_Strategy_Weapon_Melee : MonoBehaviour, I_Interactuar
{
    [Button(nameof(Interactuar))]
    public int DamageProyectil = 25;
    public string Proyectil = "Melee";

    public void Interactuar()
    {
        Debug.Log("[C_Shooter] Atacar con melee", gameObject);

        // Pedir proyectil
        GameObject proj = C_PoolManager.Instance.Request(Proyectil, transform.position);

        // Opcional: si tenés C_Projectile con Init(model)
        var controller = proj.GetComponent<C_Projectile>();
        if (controller != null)
        {
            M_Projectile modelData = new M_Projectile()
            {
                Damage = DamageProyectil,
                Speed = 0f,
                Direction = Vector3.zero,
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
