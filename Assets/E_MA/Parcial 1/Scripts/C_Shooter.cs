using CustomInspector;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class C_Shooter : MonoBehaviour, I_Interactuar
{
    [Button(nameof(Interactuar))]
    public Transform ShootPoint;
    public int DamageProyectil = 25;
    public float SpeedProyectil = 30f;
    public string Proyectil = "proyectil";
    public GameObject EfectoVisual;
    public void Interactuar()
    {
        //Debug.Log("Reibe orden de ejcutarse", gameObject);
        if (EfectoVisual != null)
        {
            // Activar el efecto visual
            StartCoroutine(ActivarEfectoVisual());
        }

        Vector3 spawnPos = ShootPoint.position + ShootPoint.forward * 2f;

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
                Direction = ShootPoint.forward,
                Owner = gameObject
            };
            controller.Init(modelData);
        }
    }

    private IEnumerator ActivarEfectoVisual()
    {
        EfectoVisual.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        EfectoVisual.gameObject.SetActive(false);
    }

    public void Recargar()
    {
        Debug.Log("[C_Shooter] Recargar!", gameObject);
    }
}
