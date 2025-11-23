using UnityEngine;
using Unity.Notifications.Android;
using System;

public class AndroidNotification : MonoBehaviour
{
    public string TituloDeNotificacion = "Regrese, tenemos un buen paquete para usted aqui";
    public string TextoAMostrar = "La energia ya se recargo para otro round !";
    public int SegundosDeEsperaDeEspera = 50;
    public static string CHANNEL_ID = "Notif_Channel_1";
    public bool IniciarNotificacionesAlComenzar = false;

    void OnEnable()
    {
        RequestPermission();
        SetupChannel();
        if(IniciarNotificacionesAlComenzar)
        {
            Poner1NotificacionTest();
        }
    }

    public void Poner1NotificacionTest() // Llamado desde boton UI
    {
        SendNotificationInOneMinute(TituloDeNotificacion, TextoAMostrar, SegundosDeEsperaDeEspera);
    }
    private void RequestPermission()
    {
        var status = AndroidNotificationCenter.UserPermissionToPost;

        if (status != PermissionStatus.Allowed)
        {
            Debug.Log("El permiso de notificaciones NO está concedido. Debes pedirlo manualmente en la configuración del sistema.");
        }
    }


    private void SetupChannel()
    {
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
    }

    public static void SendNotificationInOneMinute(string title, string text, int SegundosDeEspera)
    {
        try
        {
            var notification = new Unity.Notifications.Android.AndroidNotification()
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
    }
}
