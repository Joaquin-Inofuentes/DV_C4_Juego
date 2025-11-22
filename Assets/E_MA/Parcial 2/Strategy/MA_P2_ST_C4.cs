using UnityEngine;

public class MA_P2_ST_C4 : MA_P2_ST_WeaponBase
{
    protected override void Awake()
    {
        base.Awake();
        BalasActuales = 1; // un explosivo
    }

    public override void Disparar()
    {
        if (BalasActuales <= 0) return;

        BalasActuales = 0;
        Debug.Log("C4 EXPLOTA!! Daño: " + Daño);

        // Ejemplo: esfera de explosión
        Debug.DrawRay(Origen.position, Vector3.up * 2f, Color.magenta, 1f);
    }

    public override void Recargar()
    {
        // C4 no se recarga
    }
}
