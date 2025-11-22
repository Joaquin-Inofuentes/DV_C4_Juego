using UnityEngine;

public class MA_P2_ST_ArmaMelee : MA_P2_ST_WeaponBase
{
    protected override void Awake()
    {
        base.Awake();
        BalasActuales = 1; // simula siempre cargado
    }

    public override void Disparar()
    {
        Debug.Log("MELEE golpe! Daño: " + Daño);
    }

    public override void Recargar()
    {
        BalasActuales = 1; // siempre vuelve a 1
    }
}
