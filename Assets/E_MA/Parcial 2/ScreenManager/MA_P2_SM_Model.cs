using UnityEngine;

/// <summary>
/// Modelo (M - MVC)
/// Representa una pantalla (única). Identificador = ScreenObject.name.
/// Puede configurarse si bloquea input o no desde el inspector.
/// </summary>
[System.Serializable]
public class MA_P2_SM_Model
{
    // Asignar el GameObject UI en el inspector (hijo del Canvas)
    public GameObject ScreenObject;

    // Si true, esta pantalla bloquea input global cuando está en top
    public bool blocksInput = false;

    // Conveniencia
    public bool IsActive => ScreenObject != null && ScreenObject.activeSelf;
    public string ScreenName => ScreenObject != null ? ScreenObject.name : string.Empty;
}
