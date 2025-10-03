using UnityEngine;

[RequireComponent(typeof(V_Projectile))]
public class C_Projectile : MonoBehaviour
{
    public M_Projectile Model { get; private set; }
    public V_Projectile view;

    public float lifeTimer = 0f;
    public float lifeTime = 3f; // 3 segundos

    void OnEnable()
    {
        view = GetComponent<V_Projectile>();
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = false;
    }

    public void Init(M_Projectile model)
    {
        Model = model;

        if (Model.Direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(Model.Direction);

        lifeTimer = 0f; // reset del timer
    }

    void Update()
    {
        // Evitar error si Model es null
        if (Model == null) return;

        transform.position += Model.Direction * Model.Speed * Time.deltaTime;

        // Timer para devolver al pool
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            C_ProjectilePool.Instance.ReturnProjectile(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        view.ShowCollision(collision.gameObject);

        var damageable = collision.gameObject.GetComponent<I_ReceivesDamage>();
        if (damageable != null)
        {
            damageable.ReceiveDamage(Model.Damage);
            Debug.Log($"[C_Projectile] Hizo {Model.Damage} de daño a {collision.gameObject.name}");
        }

        C_ProjectilePool.Instance.ReturnProjectile(gameObject);
    }

    public void ResetState()
    {
        Model = null;
        lifeTimer = 0f;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
