using UnityEngine;
using System;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class AndroidNotification : MonoBehaviour
{
    public string TituloDeNotificacion = "Regrese, tenemos un buen paquete para usted aqui";
    public string TextoAMostrar = "La energia ya se recargo para otro round !";
    public int SegundosDeEsperaDeEspera = 50;
    public static string CHANNEL_ID = "Notif_Channel_1";
    public bool IniciarNotificacionesAlComenzar = false;

    void OnEnable()
    {
#if UNITY_ANDROID
        RequestPermission();
        SetupChannel();

        if (IniciarNotificacionesAlComenzar)
        {
            Poner1NotificacionTest();
        }
#endif
    }

    public void Poner1NotificacionTest() // Llamado desde boton UI
    {
#if UNITY_ANDROID
        SendNotificationInOneMinute(TituloDeNotificacion, TextoAMostrar, SegundosDeEsperaDeEspera);
#endif
    }

    private void RequestPermission()
    {
#if UNITY_ANDROID
        var status = AndroidNotificationCenter.UserPermissionToPost;

        if (status != PermissionStatus.Allowed)
        {
            Debug.Log("El permiso de notificaciones NO está concedido.");
        }
#endif
    }

    private void SetupChannel()
    {
#if UNITY_ANDROID
        try
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = CHANNEL_ID,
                Name = "Default Notifications",
                Importance = Importance.High,
                Description = "General notifications"
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            Debug.Log("Canal creado OK");
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR al crear canal: {e.Message}");
        }
#endif
    }

    public static void SendNotificationInOneMinute(string title, string text, int SegundosDeEspera)
    {
#if UNITY_ANDROID
        try
        {
            var notification = new AndroidNotification()
            {
                Title = title,
                Text = text,
                FireTime = DateTime.Now.AddSeconds(SegundosDeEspera)
            };

            int id = AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);

            Debug.Log($"Notificación programada. ID: {id}, Hora: {notification.FireTime}");
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR al enviar notificación: {e.Message}");
        }
#else
        Debug.Log("Notificaciones solo funcionan en Android.");
#endif
    }
}
