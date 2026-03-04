using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AM2_F_Notificaciones : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(InicializarAvisoDeReEntry), 2);
    }

    public void InicializarAvisoDeReEntry()
    {
        bool yaProgramado = PlayerPrefs.GetInt("NotifSabadoConfig", 0) == 1;

        if (!yaProgramado)
        {
            NotificarSabado16();

            PlayerPrefs.SetInt("NotifSabadoConfig", 1);
            PlayerPrefs.Save();
        }
    }


    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            string titulo = "Llamado a la gloria";
            string cuerpo = "El equipo delta te necesita. Venga para destruir esos cascarudotes";
            int delay = 10;

            AndroidNotification.SendNotificationInOneMinute(titulo, cuerpo, delay);
            Debug.Log("App en segundo plano");
        }

    }

    void NotificarSabado16()
    {
        System.DateTime ahora = System.DateTime.Now;

        int diasHastaSabado = ((int)System.DayOfWeek.Saturday - (int)ahora.DayOfWeek + 7) % 7;

        System.DateTime proximoSabado = ahora.Date.AddDays(diasHastaSabado).AddHours(16);

        if (proximoSabado <= ahora)
            proximoSabado = proximoSabado.AddDays(7);

        int segundos = (int)(proximoSabado - ahora).TotalSeconds;

        string titulo = "🔥 Sábado gamer desbloqueado";
        string cuerpo = "Son las 16:00. Es hora de snacks, estrategia y destruir enemigos. El campo de batalla te espera.";

        AndroidNotification.SendNotificationInOneMinute(titulo, cuerpo, segundos);
    }


}
