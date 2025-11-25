using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM2_P2_Enemigo : MonoBehaviour
{
    [Header("Detección")]
    public LayerMask layersEnemigos;
    public Transform puntoQueMira;

    [Header("Movimiento")]
    public Transform targetDestino;
    public float velocidadMovimiento = 3f;
    public float distanciaAtaque = 4f;

    [Header("ATAQUE FINAL")]
    public float distanciaFinalParaAtacar = 3f;  // <<< NUEVO

    [Header("Arma")]
    public C_Shooter shooter;
    public float tiempoEntreDisparos = 0.2f;

    [Header("Estados públicos")]
    public bool tengoEnemigo = false;
    public bool perdiEnemigo = true;
    public bool ignorarEnemigos = false;   // <<< NUEVO

    private List<Transform> enemigosDetectados = new();
    private Transform enemigoActual;
    private float timerDisparo;

    void Update()
    {
        VerificarDistanciaFinal();
        ActualizarObjetivo();
        MirarAlObjetivo();
        AccionSegunEstado();
        DibujarLineasDebug();
        if(targetDestino == null && ObjetivoFinal.Objetivo != null)
        {
            targetDestino = ObjetivoFinal.Objetivo.transform;
        }
    }

    // =====================================================
    // SI ESTÁ CERCA DEL DESTINO FINAL → IGNORA TODO
    // =====================================================
    void VerificarDistanciaFinal()
    {
        if (targetDestino == null) return;

        float dist = Vector3.Distance(transform.parent.position, targetDestino.position);

        ignorarEnemigos = dist <= distanciaFinalParaAtacar;
    }

    // =====================================================
    // DETECCIÓN POR TRIGGER Y LAYER
    // =====================================================
    void OnTriggerStay(Collider other)
    {
        if (ignorarEnemigos) return; // <<< ignora detecciones

        if (EstaEnLayersEnemigos(other.gameObject.layer))
        {
            if (!enemigosDetectados.Contains(other.transform))
                enemigosDetectados.Add(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (ignorarEnemigos) return;

        if (EstaEnLayersEnemigos(other.gameObject.layer))
        {
            enemigosDetectados.Remove(other.transform);
        }
    }

    bool EstaEnLayersEnemigos(int layer)
    {
        return (layersEnemigos.value & (1 << layer)) != 0;
    }

    // =====================================================
    // SELECCIÓN DEL MÁS CERCANO (solo si NO ignora)
    // =====================================================
    void ActualizarObjetivo()
    {
        if (ignorarEnemigos)
        {
            enemigoActual = null;
            tengoEnemigo = false;
            perdiEnemigo = true;
            return;
        }

        enemigoActual = null;

        if (enemigosDetectados.Count == 0)
        {
            tengoEnemigo = false;
            perdiEnemigo = true;
            return;
        }

        float menorDist = float.MaxValue;

        foreach (var e in enemigosDetectados)
        {
            if (e == null) continue;

            float d = Vector3.Distance(transform.position, e.position);
            if (d < menorDist)
            {
                menorDist = d;
                enemigoActual = e;
            }
        }

        tengoEnemigo = enemigoActual != null;
        perdiEnemigo = enemigoActual == null;
    }

    // =====================================================
    // LOOK AT
    // =====================================================
    void MirarAlObjetivo()
    {
        if (puntoQueMira == null) return;

        if (ignorarEnemigos && targetDestino)
        {
            puntoQueMira.LookAt(targetDestino.position);
            return;
        }

        if (enemigoActual == null) return;
        puntoQueMira.LookAt(enemigoActual.position);
    }

    // =====================================================
    // LÓGICA PRINCIPAL
    // =====================================================
    void AccionSegunEstado()
    {
        if (ignorarEnemigos)
        {
            DetenerMovimiento();
            IntentarDisparar();
            return;
        }

        if (enemigoActual != null)
        {
            float dist = Vector3.Distance(transform.parent.position, enemigoActual.position);

            if (dist > distanciaAtaque)
            {
                MoverHaciaEnemigo();
            }
            else
            {
                DetenerMovimiento();
                IntentarDisparar();
            }
        }
        else
        {
            MoverHaciaDestino();
        }
    }

    // =====================================================
    // DISPARO
    // =====================================================
    void IntentarDisparar()
    {
        timerDisparo += Time.deltaTime;

        if (timerDisparo >= tiempoEntreDisparos)
        {
            timerDisparo = 0f;
            shooter.Interactuar();
        }
    }

    // =====================================================
    // MOVIMIENTO HACIA DESTINO
    // =====================================================
    void MoverHaciaDestino()
    {
        if (targetDestino == null) return;

        Vector3 dir = targetDestino.position - transform.parent.position;
        dir.y = 0f;
        dir = dir.normalized;

        transform.parent.position += dir * velocidadMovimiento * Time.deltaTime;

        if (dir != Vector3.zero)
            puntoQueMira.rotation = Quaternion.LookRotation(dir);
    }

    void DetenerMovimiento() { }

    void MoverHaciaEnemigo()
    {
        if (enemigoActual == null) return;

        Vector3 dir = enemigoActual.position - transform.parent.position;
        dir.y = 0f;
        dir = dir.normalized;

        transform.parent.position += dir * velocidadMovimiento * Time.deltaTime;

        if (dir != Vector3.zero)
            puntoQueMira.rotation = Quaternion.LookRotation(dir);
    }

    // =====================================================
    // DEBUG LINE
    // =====================================================
    void DibujarLineasDebug()
    {
        if (ignorarEnemigos) return;

        foreach (var e in enemigosDetectados)
        {
            if (e == null) continue;
            Debug.DrawLine(transform.position, e.position, Color.red);
        }
    }
}
