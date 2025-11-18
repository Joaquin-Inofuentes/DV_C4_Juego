using CustomInspector;
using UnityEngine;

public class C_Enemy : MonoBehaviour, I_ReceivesDamage
{
    [Button(nameof(Atacar), true)]
    public Collider ObjetivoPosible;


    public int Health = 100;
    public GameObject _Weapon;
    public I_Interactuar Weapon;

    public void Atacar(Collider Enemigo) // Llamado desde trigger Unity Event
    {
        if (Enemigo.name != "Player") return;
        if (Weapon == null)
        {
            InicializarVariables();
            if (Weapon == null)
                Debug.Log("No tiene weapon");
            return;
        }
        // Mira hacia el objetivo
        transform.LookAt(Enemigo.transform);
        // Dispara el arma
        Weapon.Interactuar();
    }

    public void OnEnable()
    {
        InicializarVariables();
    }

    public void InicializarVariables()
    {
        if (_Weapon != null)
        {
            Weapon = _Weapon.GetComponent<I_Interactuar>();
            if (Weapon == null)
            {
                Debug.LogError($"[C_Enemy] El objeto {_Weapon.name} no tiene un componente que implemente I_Interactuar.");
            }
        }
        else
        {
            Debug.LogError("[C_Enemy] _Weapon no está asignado.");
        }
    }


    public void ReceiveDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"[C_Enemy] {gameObject.name} recibió {damage} daño. Vida restante: {Health}");

        if (Health <= 0)
        {
            Debug.Log($"[C_Enemy] {gameObject.name} murió.");
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        Debug.Log("Se murio", gameObject);
    }
}
