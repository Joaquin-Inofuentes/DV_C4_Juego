using UnityEngine;

public class MA_P2_ST_TorretaFija : MA_P2_ST_WeaponBase
{
    public override void Disparar()
    {
        if (Time.time - ultimoDisparo < RateDisparo) return;
        if (BalasActuales <= 0) { Recargar(); return; }

        ultimoDisparo = Time.time;
        BalasActuales--;

        Debug.Log("TORRETA dispara automáticamente");

        Debug.DrawRay(Origen.position, Origen.forward * 20f, Color.green, 0.2f);
    }
}
