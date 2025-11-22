public interface MA_P2_ST_IWeapon
{
    string Nombre { get; }
    float Daño { get; }
    float RateDisparo { get; }
    float RateRecarga { get; }
    string TipoMunicion { get; }

    int CapacidadCargador { get; }
    int BalasActuales { get; }
    int MunicionTotal { get; }

    UnityEngine.Transform Origen { get; }

    void Disparar();
    void Recargar();
}
