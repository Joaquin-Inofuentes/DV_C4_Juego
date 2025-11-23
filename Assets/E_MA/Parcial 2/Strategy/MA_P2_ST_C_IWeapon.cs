using UnityEngine;

public interface MA_P2_ST_C_IWeapon
{
    string Nombre { get; }
    int Daño { get; }
    float RateDisparo { get; }
    float RateRecarga { get; }
    string TipoMunicion { get; }
    float SpeedShot { get; }

    int CapacidadCargador { get; }
    int BalasActuales { get; set; }
    int MunicionTotal { get; }
    float UltimoDisparo { get; }

    GameObject EfectoVisual { get; }
    Transform Origen { get; }

    void Disparar();
    void Recargar();
    void CreacionDeProyectil();
    void IniciarEfectoVisual();
}
