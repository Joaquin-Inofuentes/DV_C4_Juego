using UnityEngine;

public class C_Enemy : MonoBehaviour, I_ReceivesDamage
{
    public int Health = 100;

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
}
