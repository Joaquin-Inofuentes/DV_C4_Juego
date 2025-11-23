using System.Collections;
using UnityEngine;

public class MA_P2_ST_C_Metralleta : MonoBehaviour, MA_P2_ST_C_IWeapon
{
    [Header("Identidad")]
    [SerializeField] public string nombre = "Metralleta";
    [SerializeField] public string tipoMunicion = "BalasChicas";

    [Header("Stats")]
    [SerializeField] public int daño = 10;
    [SerializeField] public float rateDisparo = 0.1f;
    [SerializeField] public float rateRecarga = 2f;
    [SerializeField] public float speedShot = 15f;

    [Header("Munición")]
    [SerializeField] public int capacidadCargador = 30;
    [SerializeField] public int balasActuales = 30;
    [SerializeField] public int municionTotal = 150;

    [Header("Origen")]
    [SerializeField] public Transform origen;
    [SerializeField] public GameObject efectoVisual;

    public float ultimoDisparo;


    #region PedidosDeInterface
    // PROPIEDADES (cumplen con la interface)
    public string Nombre => nombre;
    public int Daño => daño;
    public float RateDisparo => rateDisparo;
    public float RateRecarga => rateRecarga;
    public string TipoMunicion => tipoMunicion;
    public float SpeedShot => speedShot;

    public int CapacidadCargador => capacidadCargador;
    public int BalasActuales { get => balasActuales; set => balasActuales = value; }
    public int MunicionTotal => municionTotal;

    public float UltimoDisparo => ultimoDisparo;

    public Transform Origen => origen;
    public GameObject EfectoVisual => efectoVisual;
    #endregion



    protected void Awake()
    {
        BalasActuales = capacidadCargador;
    }


    public void Recargar()
    {
        if (BalasActuales == CapacidadCargador) return;
        if (municionTotal <= 0) return;

        int falta = CapacidadCargador - BalasActuales;
        int aCargar = Mathf.Min(falta, municionTotal);

        BalasActuales += aCargar;
        municionTotal -= aCargar;
    }




    public void Disparar()
    {
        if (Time.time - ultimoDisparo < RateDisparo) return;
        if (BalasActuales <= 0)
        {
            Recargar();
            return;
        }

        ultimoDisparo = Time.time;
        BalasActuales--;

        //Debug de la linea de proyectil
        Debug.DrawRay(Origen.position, Origen.forward * 20f, Color.green, 0.2f);
        Debug.Log("TORRETA dispara automáticamente");

        CreacionDeProyectil();
        IniciarEfectoVisual();
    }


    public void CreacionDeProyectil()
    {
        Vector3 spawnPos = origen.position + origen.forward * 2f;

        // Pedir proyectil
        GameObject proj = C_PoolManager.Instance.Request(tipoMunicion, spawnPos);
        if(proj == null)
        {
            Debug.LogWarning("No hay proyectiles disponibles en el pool para el tipo: " + tipoMunicion);
            return;
        }
        // Opcional: si tenés C_Projectile con Init(model)
        var controller = proj.GetComponent<C_Projectile>();
        if (controller != null)
        {
            M_Projectile modelData = new M_Projectile()
            {
                Damage = daño,
                Speed = SpeedShot,
                Direction = origen.forward,
                Owner = gameObject
            };
            controller.Init(modelData);
        }
    }

    public void IniciarEfectoVisual()
    {
        if (EfectoVisual != null)
        {
            // Activar el efecto visual
            StartCoroutine(ActivarEfectoVisual());
        }
    }

    private IEnumerator ActivarEfectoVisual()
    {
        if (efectoVisual == null)
        {
            Debug.LogError("EfectoVisual es null en " + gameObject.name);
            yield break;
        }
        EfectoVisual.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        EfectoVisual.gameObject.SetActive(false);
    }
}
