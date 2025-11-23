using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MA_P2_ST_C_ArmaMelee : MA_P2_ST_C_WeaponBase, MA_P2_ST_C_IWeapon
{
    [Header("Identidad")]
    [SerializeField] public string nombre = "Melee";
    [SerializeField] public string tipoMunicion = "Melee";

    [Header("Stats")]
    [SerializeField] public int daño = 10;
    [SerializeField] public float rateDisparo = 1f;
    [SerializeField] public float rateRecarga = 2f;
    [SerializeField] public float speedShot = 0f;

    [Header("Munición")]
    [SerializeField] public int capacidadCargador = 1;
    [SerializeField] public int balasActuales = 1;
    [SerializeField] public int municionTotal = 1;

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
            BalasActuales = 1;
            // Al ser melee no se recarga
            //return;
        }

        ultimoDisparo = Time.time;
        BalasActuales--;

        //Debug de la linea de proyectil
        Debug.DrawRay(Origen.position, Origen.forward * 20f, Color.green, 0.2f);
        Debug.Log("hacha dispara");

        CreacionDeProyectil();
        IniciarEfectoVisual();
    }


    public void CreacionDeProyectil()
    {
        Vector3 spawnPos = origen.position + origen.forward * 2f;

        // Pedir proyectil
        GameObject proj = C_PoolManager.Instance.Request(tipoMunicion, spawnPos);
        if (proj == null)
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
            DispararGolpe();
        
    }

    private IEnumerator ActivarEfectoVisual()
    {
        EfectoVisual.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        EfectoVisual.gameObject.SetActive(false);
    }



    public Transform objetoGolpe;   // tu GameObject hijo (el que se va a mover)
    public float distancia = 0.5f;
    

    Vector3 posOriginal;
    Coroutine golpeActual;

    public void DispararGolpe()
    {
        if (golpeActual != null)
            StopCoroutine(golpeActual);

        golpeActual = StartCoroutine(Golpe());
    }

    IEnumerator Golpe()
    {
        float tiempoIda = rateDisparo * 0.3f;  // rápido
        float tiempoVuelta = rateDisparo * 0.7f; // lento

        Vector3 posFinal = posOriginal + objetoGolpe.forward * distancia;

        Debug.Log("⚡ INICIO DEL GOLPE");

        // IDA (rápido)
        float t = 0;
        while (t < tiempoIda)
        {
            t += Time.deltaTime;
            float lerp = t / tiempoIda;
            objetoGolpe.localPosition = Vector3.Lerp(posOriginal, posFinal, lerp);
            yield return null;
        }

        Debug.Log("⏫ POSICIÓN MÁXIMA ALCANZADA");

        // VUELTA (lento)
        t = 0;
        while (t < tiempoVuelta)
        {
            t += Time.deltaTime;
            float lerp = t / tiempoVuelta;
            objetoGolpe.localPosition = Vector3.Lerp(posFinal, posOriginal, lerp);
            yield return null;
        }

        Debug.Log("🔙 GOLPE FINALIZADO (VOLVIÓ)");
        objetoGolpe.localPosition = posOriginal;
    }

    public void Restaurar()
    {
        objetoGolpe.localPosition = posOriginal;
    }



    void OnEnable()
    {
        posOriginal = objetoGolpe.localPosition;
    }



}
