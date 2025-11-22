using System.Collections.Generic;
using UnityEngine;

// 1. Enum para definir acciones
public enum ScreenAction
{
    Push,
    Pop,
    HideAll
}

// 2. Clase serializable para el Inspector
[System.Serializable]
public class ScreenCase
{
    public string nombre;       // Nombre de la pantalla (Solo necesario para Push)
    public ScreenAction accion; // Acción a realizar
    public KeyCode tecla;       // Tecla asignada
}

public class MA_P2_SM_TestingUnit : MonoBehaviour
{
    [Header("Configura tus atajos aquí")]
    // 3. Inicializamos la lista con la CONFIGURACIÓN POR DEFECTO
    public List<ScreenCase> configuraciones = new List<ScreenCase>()
    {
        new ScreenCase { nombre = "MainMenu",      accion = ScreenAction.Push,    tecla = KeyCode.Alpha1 },
        new ScreenCase { nombre = "Pause",         accion = ScreenAction.Push,    tecla = KeyCode.Alpha2 },
        new ScreenCase { nombre = "ConfirmDialog", accion = ScreenAction.Push,    tecla = KeyCode.Alpha3 },
        new ScreenCase { nombre = "",              accion = ScreenAction.Pop,     tecla = KeyCode.Backspace },
        new ScreenCase { nombre = "",              accion = ScreenAction.HideAll, tecla = KeyCode.H }
    };

    private void OnEnable()
    {
        MA_P2_SM_Controller.OnScreenChanged += HandleScreenChanged;
        MA_P2_SM_Controller.ConfirmResponseEvent += HandleConfirmResponse;
    }

    private void OnDisable()
    {
        MA_P2_SM_Controller.OnScreenChanged -= HandleScreenChanged;
        MA_P2_SM_Controller.ConfirmResponseEvent -= HandleConfirmResponse;
    }

    // Monitor general de cambios
    private void HandleScreenChanged(string screenName, string action)
    {
        Debug.Log($"[Listener] {screenName} -> {action} | InputBlocked: {MA_P2_SM_Controller.Instance.InputBlocked}");
    }

    // Respuesta específica para Dialogs (Log simple)
    private void HandleConfirmResponse(string screenName, int result)
    {
        if (screenName.Contains("Dialog"))
        {
            string respuesta = result == 1 ? "SÍ" : "NO";
            Debug.Log($"📝 LOG SIMPLE DIALOG: '{screenName}' respondió {respuesta} ({result})");
        }
    }

    private void Update()
    {
        // Recorremos la lista de configuraciones
        foreach (var caso in configuraciones)
        {
            if (Input.GetKeyDown(caso.tecla))
            {
                EjecutarCaso(caso);
            }
        }
    }

    private void EjecutarCaso(ScreenCase caso)
    {
        if (!MA_P2_SM_Controller.Instance) return;
        switch (caso.accion)
        {
            case ScreenAction.Push:
                MA_P2_SM_Controller.Instance.Push(caso.nombre);
                break;

            case ScreenAction.Pop:
                MA_P2_SM_Controller.Instance.Pop();
                break;

            case ScreenAction.HideAll:
                MA_P2_SM_Controller.Instance.HideAll();
                break;
        }
    }
}