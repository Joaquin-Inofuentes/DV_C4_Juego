using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class V_Projectile : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log($"[V_Projectile] Proyectil activado en {transform.position}");
    }

    public void ShowCollision(GameObject other)
    {
        Debug.Log($"[V_Projectile] Colisión con {other.name}");
    }
}
