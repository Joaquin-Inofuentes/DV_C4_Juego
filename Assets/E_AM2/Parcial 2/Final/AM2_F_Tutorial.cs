using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AM2_F_Tutorial : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI TextoGuia;

    [Header("Tiempos")]
    public float DelayNeutroInicial = 2f;
    public float DelayEntreMensajes = 4f; // SIEMPRE 4s

    // --- Estado ---
    private List<string> _mensajes = new List<string>();
    private int _idx = 0;
    private bool _running = false;

    // Gates (acciones del jugador)
    private bool _movedStickDerecho = false;
    private bool _pressedBotonIzquierdo = false;

    void Awake()
    {
        CargarMensajes();
        if (TextoGuia != null) TextoGuia.text = "";
    }

    void Start()
    {
        IniciarTutorial();
    }

    void CargarMensajes()
    {
        _mensajes.Clear();

        _mensajes.Add("Bienvenido.");
        _mensajes.Add("Bien!! El juego se basa en reclutar y aguantar.");
        _mensajes.Add("Si destruyen el centro azul (Nexo), fallaste. Debes protegerlo.");
        _mensajes.Add("Vamos a movernos...");
        _mensajes.Add("Muévase con el stick derecho.");
        _mensajes.Add("Bien, te mueves rápido. Ahora a reclutar.");
        _mensajes.Add("Apriete el botón izquierdo para crear un aliado.");
        _mensajes.Add("Crear un aliado cuesta plata. Si no tenés, no sale 😅");
        _mensajes.Add("Mata los que se te piden según el contador y ganarás. ¡Éxitos!");
    }

    public void IniciarTutorial()
    {
        if (_running) return;
        _running = true;

        _idx = 0;
        _movedStickDerecho = false;
        _pressedBotonIzquierdo = false;

        StopAllCoroutines();
        StartCoroutine(RunTutorial());
    }

    IEnumerator RunTutorial()
    {
        // 1) Bienvenido (2s neutro)
        MostrarMensajeActual();
        yield return new WaitForSeconds(DelayNeutroInicial);
        Avanzar();

        // 2) Mensajes “solo texto” con pausas
        yield return MostrarConPausa(); // reclutar y aguantar
        yield return MostrarConPausa(); // Nexo
        yield return MostrarConPausa(); // Vamos a movernos...

        // 3) Moverse con stick derecho (gate)
        MostrarMensajeActual(); // "Muévase con el stick derecho."
        yield return EsperarHasta(() => _movedStickDerecho);
        Avanzar();
        yield return new WaitForSeconds(DelayEntreMensajes);

        // 4) “Bien te mueves…”
        yield return MostrarConPausa();

        // 5) Botón izquierdo (gate)
        MostrarMensajeActual(); // "Apriete el botón izquierdo..."
        yield return EsperarHasta(() => _pressedBotonIzquierdo);
        Avanzar();
        yield return new WaitForSeconds(DelayEntreMensajes);

        // 6) Mensajes finales con pausas
        yield return MostrarConPausa(); // cuesta plata
        yield return MostrarConPausa(); // objetivo

        Debug.Log("<color=yellow>✅ Tutorial terminado. Se cargará el menú inicial...</color>");
        _running = false;
        GameManager.Instance.CambiarDeEscena("MainMenu");
    }

    IEnumerator MostrarConPausa()
    {
        MostrarMensajeActual();
        yield return new WaitForSeconds(DelayEntreMensajes);
        Avanzar();
    }

    IEnumerator EsperarHasta(System.Func<bool> condicion)
    {
        while (!condicion())
            yield return null;
    }

    void MostrarMensajeActual()
    {
        if (TextoGuia == null) return;

        int total = _mensajes.Count;
        int numeroHumano = Mathf.Clamp(_idx + 1, 1, total);

        string msg = (_idx >= 0 && _idx < total) ? _mensajes[_idx] : "";
        TextoGuia.text = $"{numeroHumano}/{total}  {msg}";
    }

    void Avanzar()
    {
        _idx++;
        if (_idx >= _mensajes.Count)
            _idx = _mensajes.Count - 1;
    }

    // =========================
    // HOOKS (LLAMADOS EXTERNOS)
    // =========================

    // Llamalo cuando detectes movimiento del stick derecho (tu input)
    public void Notify_StickDerechoMovido()
    {
        _movedStickDerecho = true;
    }

    // Llamalo desde tu botón izquierdo (que crea aliado) o input real
    public void Notify_BotonIzquierdoApretado()
    {
        _pressedBotonIzquierdo = true;
    }

    // Opcional: por si querés “forzar” avanzar debug
    public void Debug_SaltarGateActual()
    {
        _movedStickDerecho = true;
        _pressedBotonIzquierdo = true;
    }
}