using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AM2_P2_Aliado : MonoBehaviour
{
    [Header("Detección")]
    public LayerMask layerEnemigos;
    public Transform puntoQueMira;

    [Header("Disparo")]
    public C_Shooter shooter;
    public float tiempoEntreDisparos = 0.2f;

    [Header("Balas y recarga")]
    public int balasMaximas = 10;
    public float tiempoRecarga = 3f;
    public int balasActuales;
    public bool isReloading;

    public readonly List<Transform> listaEnemigos = new();
    public Transform enemigoActual;
    public float timerDisparo;

    void Start()
    {
        balasActuales = balasMaximas;
        ActualizarBarraRecarga(1f); // llena
    }

    void Update()
    {
        ActualizarObjetivo();
        MirarAlObjetivo();
        DibujarLineaDebug();
        EjecutarDisparo();
    }

    // =====================================================
    // DETECCIÓN POR LAYER
    // =====================================================
    void OnTriggerStay(Collider other)
    {
        if (EstaEnLayerEnemigo(other.gameObject.layer))
        {
            if (!listaEnemigos.Contains(other.transform))
                listaEnemigos.Add(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (EstaEnLayerEnemigo(other.gameObject.layer))
        {
            listaEnemigos.Remove(other.transform);
        }
    }

    bool EstaEnLayerEnemigo(int layer)
    {
        return (layerEnemigos.value & (1 << layer)) != 0;
    }

    // =====================================================
    // SELECCIÓN DEL OBJETIVO
    // =====================================================
    void ActualizarObjetivo()
    {
        if (listaEnemigos.Count == 0)
        {
            enemigoActual = null;
            return;
        }

        float menorDist = float.MaxValue;
        Transform elegido = null;

        foreach (var e in listaEnemigos)
        {
            if (e == null) continue;

            float d = Vector3.Distance(transform.position, e.position);
            if (d < menorDist)
            {
                menorDist = d;
                elegido = e;
            }
        }

        enemigoActual = elegido;
    }

    // =====================================================
    // LOOK AT
    // =====================================================
    void MirarAlObjetivo()
    {
        if (enemigoActual == null || puntoQueMira == null) return;
        puntoQueMira.LookAt(enemigoActual.position);
    }

    // =====================================================
    // DEBUG
    // =====================================================
    void DibujarLineaDebug()
    {
        if (enemigoActual == null) return;
        Debug.DrawLine(puntoQueMira.position, enemigoActual.position, Color.red);
    }

    public TextMeshProUGUI IndicadorDeBalas;

    // =====================================================
    // DISPARO + RECARGA + BALAS
    // =====================================================
    void EjecutarDisparo()
    {
        if (isReloading)
            IndicadorDeBalas.text = "Load...";
        else
        {
            IndicadorDeBalas.text = balasActuales.ToString() + "/" + balasMaximas;

        }

        barraRecarga.parent.gameObject.SetActive(isReloading);
        if (enemigoActual == null || isReloading) return;

        timerDisparo += Time.deltaTime;

        if (timerDisparo >= tiempoEntreDisparos)
        {
            timerDisparo = 0f;

            if (balasActuales > 0)
            {
                shooter.Interactuar();
                balasActuales--;

                if (balasActuales <= 0)
                    StartCoroutine(Recargar());
            }
        }
    }

    // =====================================================
    // RECARGA
    // =====================================================
    IEnumerator Recargar()
    {
        isReloading = true;
        float t = 0f;

        while (t < tiempoRecarga)
        {
            t += Time.deltaTime;
            float progreso = t / tiempoRecarga;

            // actualizar barra
            ActualizarBarraRecarga(progreso);

            yield return null;
        }

        balasActuales = balasMaximas;
        ActualizarBarraRecarga(1f);

        isReloading = false;
    }

    // =====================================================
    // BARRA DE RECARGA (RectTransform + RawImage)
    // Pivot X = 0
    // =====================================================
    [Header("UI Recarga")]
    public RectTransform barraRecarga;  // tu RawImage

    void ActualizarBarraRecarga(float normalized)
    {
        if (barraRecarga == null) return;

        // Cambia solo el X, mantiene Y
        barraRecarga.localScale = new Vector3(normalized, 1f, 1f);
    }

}
