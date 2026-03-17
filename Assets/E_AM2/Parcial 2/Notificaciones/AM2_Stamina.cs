using CustomInspector;
using System;
using TMPro;
using UnityEngine;

public class AM2_Stamina : MonoBehaviour
{
    [Button(nameof(AgregarStamina))]
    [Button(nameof(ConsumirStamina), true)]
    public int Consumir = 1;

    // CONFIG (publicos)
    public int maxStamina = 5;
    public int staminaActual = 3;
    public GameObject bloqueador;
    public TextMeshProUGUI textoStamina;

    // TIEMPO (publicos)
    public string LAST_TIME_KEY = "AM2_LastStaminaTime";
    public string STAMINA_KEY = "AM2_StaminaActual";
    public float checkTimer = 0f;
    public float CHECK_INTERVAL = 2f;
    public int SECONDS_PER_STAMINA = 10; // 10s

    public static AM2_Stamina instance;

    public void OnEnable()
    {
        instance = this;
    }

    // -----------------------
    // START / LOAD
    // -----------------------
    public void Start()
    {
        // Cargar stamina guardada
        if (PlayerPrefs.HasKey(STAMINA_KEY))
            staminaActual = PlayerPrefs.GetInt(STAMINA_KEY, staminaActual);

        // Cargar last time o inicializar
        if (!PlayerPrefs.HasKey(LAST_TIME_KEY))
            PlayerPrefs.SetString(LAST_TIME_KEY, DateTime.Now.ToString("O"));
        else
        {
            // validar parse
            string s = PlayerPrefs.GetString(LAST_TIME_KEY);
            DateTime tmp;
            if (!DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out tmp))
            {
                PlayerPrefs.SetString(LAST_TIME_KEY, DateTime.Now.ToString("O"));
            }
        }
    }

    // -----------------------
    // UPDATE
    // -----------------------
    public void Update()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer >= CHECK_INTERVAL)
        {
            checkTimer = 0f;
            ChequearStaminaPorTiempo();
            ActualizarBloqueador();
        }

        ActualizarTexto();
    }

    // -----------------------
    // LÓGICA DE REGEN (ITERATIVA CORRECTA)
    // -----------------------
    public void ChequearStaminaPorTiempo()
    {
        // Leer lastTime seguro
        string saved = PlayerPrefs.GetString(LAST_TIME_KEY, DateTime.Now.ToString("O"));
        DateTime lastTime;
        if (!DateTime.TryParse(saved, null, System.Globalization.DateTimeStyles.RoundtripKind, out lastTime))
            lastTime = DateTime.Now;

        DateTime now = DateTime.Now;
        double totalSeconds = (now - lastTime).TotalSeconds;

        // Si no hay suficiente tiempo acumulado, salir
        if (totalSeconds < SECONDS_PER_STAMINA) return;

        // Iterar por bloques de SECONDS_PER_STAMINA
        int space = maxStamina - staminaActual;
        if (space <= 0)
        {
            // estamos full: reseteamos lastTime a now para no acumular más
            PlayerPrefs.SetString(LAST_TIME_KEY, now.ToString("O"));
            PlayerPrefs.Save();
            return;
        }

        int gained = 0;
        while (totalSeconds >= SECONDS_PER_STAMINA && staminaActual < maxStamina)
        {
            // aplicar una unidad
            staminaActual++;
            gained++;
            totalSeconds -= SECONDS_PER_STAMINA;

            // avanzar lastTime exactamente un bloque
            lastTime = lastTime.AddSeconds(SECONDS_PER_STAMINA);
        }

        // Guardar stamina y lastTime resultante
        PlayerPrefs.SetInt(STAMINA_KEY, staminaActual);

        // Si llegamos al max, ponemos lastTime = now (evita acumular sobrantes)
        if (staminaActual >= maxStamina)
            PlayerPrefs.SetString(LAST_TIME_KEY, now.ToString("O"));
        else
            PlayerPrefs.SetString(LAST_TIME_KEY, lastTime.ToString("O"));

        PlayerPrefs.Save();

        if (gained > 0)
            Debug.Log($"[Stamina] Ganadas: {gained}. Total = {staminaActual}");
    }

    // -----------------------
    // UI
    // -----------------------
    public void ActualizarTexto()
    {
        if (textoStamina == null) return;

        if (staminaActual >= maxStamina)
        {
            textoStamina.text = $"Stamina = {staminaActual}/{maxStamina}\nFULL";
            return;
        }

        string s = PlayerPrefs.GetString(LAST_TIME_KEY, DateTime.Now.ToString("O"));
        DateTime lastTime;
        if (!DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out lastTime))
            lastTime = DateTime.Now;

        DateTime now = DateTime.Now;
        double totalSeconds = (now - lastTime).TotalSeconds;
        int secUsed = (int)totalSeconds;
        int secToNext = SECONDS_PER_STAMINA - (secUsed % SECONDS_PER_STAMINA);

        TimeSpan t = TimeSpan.FromSeconds(secToNext);
        string timer = $"{t.Minutes:D2}:{t.Seconds:D2}";

        textoStamina.text = $"Stamina = {staminaActual}/{maxStamina}\n⏳ {timer}";
    }

    // -----------------------
    // BLOQUEADOR
    // -----------------------
    public void ActualizarBloqueador()
    {
        if (bloqueador != null)
            bloqueador.SetActive(staminaActual <= 0);
    }

    // -----------------------
    // API
    // -----------------------
    public void AgregarStamina()
    {
        if (staminaActual < maxStamina)
        {
            staminaActual = Mathf.Min(maxStamina, staminaActual + 1);
            PlayerPrefs.SetInt(STAMINA_KEY, staminaActual);

            // si after adding estamos en full, resetear lastTime
            if (staminaActual >= maxStamina)
                PlayerPrefs.SetString(LAST_TIME_KEY, DateTime.Now.ToString("O"));

            PlayerPrefs.Save();
            ActualizarTexto();
        }
    }

    public bool ConsumirStamina(int amount = 1)
    {
        if (staminaActual < amount) return false;
        staminaActual -= amount;
        PlayerPrefs.SetInt(STAMINA_KEY, staminaActual);

        // Al consumir, reiniciamos lastTime desde ahora (inicia conteo limpio)
        PlayerPrefs.SetString(LAST_TIME_KEY, DateTime.Now.ToString("O"));
        PlayerPrefs.Save();

        return true;
    }

    public void Consumir1Estamina()
    {
        ConsumirStamina(1);
    }

    // -----------------------
    // NOTIFICACIONES (al salir/pause)
    // -----------------------
    public void OnApplicationPause(bool pause) { if (pause) ProgramarNotificacionesDeStamina(); }
    public void OnApplicationFocus(bool focus) { if (!focus) ProgramarNotificacionesDeStamina(); }
    public void OnApplicationQuit() { ProgramarNotificacionesDeStamina(); }

    public void ProgramarNotificacionesDeStamina()
    {
        int faltante = maxStamina - staminaActual;
        if (faltante <= 0) return;

        // calcular segundos ya acumulados hacia la próxima
        string s = PlayerPrefs.GetString(LAST_TIME_KEY, DateTime.Now.ToString("O"));
        DateTime lastTime;
        if (!DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out lastTime))
            lastTime = DateTime.Now;

        DateTime now = DateTime.Now;
        double secondsSinceLast = (now - lastTime).TotalSeconds;
        int secondsToNext = SECONDS_PER_STAMINA - (int)(secondsSinceLast % SECONDS_PER_STAMINA);
        if (secondsToNext <= 0) secondsToNext = SECONDS_PER_STAMINA;

        for (int i = 0; i < faltante; i++)
        {
            int delay = secondsToNext + i * SECONDS_PER_STAMINA;

            string titulo = "⚡ ¡Tu energía vuelve!";
            string cuerpo = $"🟢 Recuperaste {i + 1}/{faltante} stamina. ¡Listo para entrar!";

            AndroidNotification.SendNotificationInOneMinute(titulo, cuerpo, delay);
            Debug.Log($"[Stamina] Notif programada en {delay}s -> {i + 1}/{faltante}");
        }
    }



    public void PediAumentoDeStamina()
    {
        // Mostrar rewarded y esperar resultado
        AM2_P2_AdsManager.Instance.ShowRewarded(result =>
        {
            if (result == AdsResult.Completed)
            {
                // ✔ Si el player vio el anuncio completo → darle stamina
                AgregarStamina();
                Debug.Log("Stamina +1 por publicidad");
            }
            else
            {
                // ❌ No completó → no dar reward
                Debug.Log("No se completó el anuncio, no se otorga stamina");
            }
        });
    }
}
