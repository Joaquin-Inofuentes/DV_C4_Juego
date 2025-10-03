using UnityEngine;

public class C_Shooter : MonoBehaviour
{
    public Transform ShootPoint;
    public int DamageProyectil = 25;
    public float SpeedProyectil = 30f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            M_Projectile model = new M_Projectile
            {
                Damage = DamageProyectil,
                Speed = DamageProyectil,
                Direction = transform.forward,
                Owner = gameObject
            };

            C_ProjectilePool.Instance.GetProjectile(model, ShootPoint.position);
        }
    }
}
