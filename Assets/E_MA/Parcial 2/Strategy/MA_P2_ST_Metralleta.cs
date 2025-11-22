using UnityEngine;

public class MA_P2_ST_Metralleta : MA_P2_ST_WeaponBase
{
    public override void Disparar()
    {
        if (Time.time - ultimoDisparo < RateDisparo) return;
        if (BalasActuales <= 0) { Recargar(); return; }

        ultimoDisparo = Time.time;
        BalasActuales--;

        Debug.Log("METRALLETA RATATATA daño: " + Daño);

        Debug.DrawRay(Origen.position, Origen.forward * 15f, Color.yellow, 0.1f);
    }
}
