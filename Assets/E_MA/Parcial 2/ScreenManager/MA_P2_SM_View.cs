using UnityEngine;

/// <summary>
/// Vista (V - MVC)
/// Solo activa/desactiva GameObjects y oculta todo.
/// No contiene lógica de flujo.
/// </summary>
public class MA_P2_SM_View : MonoBehaviour
{
    public void ShowOverlay(MA_P2_SM_Model target)
    {
        if (target?.ScreenObject == null) return;
        target.ScreenObject.SetActive(true);
    }

    public void Hide(MA_P2_SM_Model target)
    {
        if (target?.ScreenObject == null) return;
        target.ScreenObject.SetActive(false);
    }

    public void HideAll(MA_P2_SM_Model[] allScreens)
    {
        if (allScreens == null) return;
        foreach (var s in allScreens)
            if (s?.ScreenObject != null)
                s.ScreenObject.SetActive(false);
    }
}
