using UnityEngine;

public class MA_P2_ST_Escopeta : MA_P2_ST_WeaponBase
{
    public override void Disparar()
    {
        if (Time.time - ultimoDisparo < RateDisparo) return;
        if (BalasActuales <= 0) { Recargar(); return; }

        ultimoDisparo = Time.time;
        BalasActuales--;

        Debug.Log("ESCOPETA BOOM! Daño: " + Daño);

        // Ejemplo raycast
        Debug.DrawRay(Origen.position, Origen.forward * 10f, Color.red, 0.2f);
    }
}
