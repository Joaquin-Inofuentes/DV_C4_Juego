using UnityEngine;

public abstract class MA_P2_ST_C_WeaponBase : MonoBehaviour
{
    /*
    [Header("Identidad")]
    [SerializeField] private string nombre;
    [SerializeField] public string tipoMunicion;

    [Header("Stats")]
    [SerializeField] public int daño = 10;
    [SerializeField] private float rateDisparo = 0.2f;
    [SerializeField] private float rateRecarga = 1.5f;
    [SerializeField] public float SpeedShot = 1.5f;


    [Header("Munición")]
    [SerializeField] private int capacidadCargador = 10;
    [SerializeField] private int municionTotal = 30;

    [Header("Origen del disparo")]
    [SerializeField] public Transform origen;

    [SerializeField] public GameObject EfectoVisual;

    public string Nombre => nombre;
    public float Daño => daño;
    public float RateDisparo => rateDisparo;
    public float RateRecarga => rateRecarga;
    public string TipoMunicion => tipoMunicion;

    public int CapacidadCargador => capacidadCargador;
    
    [SerializeField] private int _balasActuales;
    public int MunicionTotal => municionTotal;

    public Transform Origen => origen;

    protected float ultimoDisparo;

    public int BalasActuales
    {
        get => _balasActuales;       
        set => _balasActuales = value; 
    }

    protected virtual void Awake()
    {
        BalasActuales = capacidadCargador;
    }

    public abstract void Disparar();

    public virtual void Recargar()
    {
        if (BalasActuales == CapacidadCargador) return;
        if (municionTotal <= 0) return;

        int falta = CapacidadCargador - BalasActuales;
        int aCargar = Mathf.Min(falta, municionTotal);

        BalasActuales += aCargar;
        municionTotal -= aCargar;
    }
    */
}
