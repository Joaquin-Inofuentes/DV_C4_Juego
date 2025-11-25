using UnityEngine;

public class EnemyAnimController : MonoBehaviour
{
    public Animator anim;

    /// <summary>
    /// Controla si el enemigo camina o no.
    /// true = caminar
    /// false = idle/ataque
    /// </summary>
    public void SetAtacando(bool estaCaminando)
    {
        anim.SetBool("walk", !estaCaminando);

        // Opcional: si no camina, puede atacar
        anim.SetBool("Attack", estaCaminando);
    }
}
