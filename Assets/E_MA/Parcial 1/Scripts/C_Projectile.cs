using UnityEngine;

[RequireComponent(typeof(V_Projectile))]
public class C_Projectile : MonoBehaviour
{
    public M_Projectile Model { get; private set; }
    public V_Projectile view;
    public GameObject Owner;

    public float lifeTimer = 0f;
    public float lifeTime = 3f; // 3 segundos
    public string NombreDelProyectil = "proyectil";

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
        Owner = model.Owner;
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
            C_PoolManager.Instance.Return(gameObject, NombreDelProyectil);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Colisiono con " + collision.gameObject.name + " | " + Model.Owner.layer.ToString() + "==" + collision.gameObject.layer.ToString(), collision.gameObject);
        if (Model.Owner == null) return;
        if (collision == null) return;
        if (Model.Owner.layer.ToString() == collision.gameObject.layer.ToString()) return;
        //Debug.Log(collision.gameObject.name);
        view.ShowCollision(collision.gameObject);

        var damageable = collision.gameObject.GetComponent<I_ReceivesDamage>();
        if (damageable != null)
        {
            //Debug.Log($"[C_Projectile] Hizo {Model.Damage} de daño a {collision.gameObject.name}");
            damageable.ReceiveDamage(Model.Damage); // Daño de proyectil
        }
        else
        {
            Debug.Log($"[C_Projectile] {collision.gameObject.name} no puede recibir daño.");
        }

        C_PoolManager.Instance.Return(gameObject, "proyectil");
    }

    void OnCollisionStay(Collision collision)
    {
        //Debug.Log("Colisiono con " + collision.gameObject.name, collision.gameObject);
    }
    public void ResetState()
    {
        Model = null;
        lifeTimer = 0f;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}
