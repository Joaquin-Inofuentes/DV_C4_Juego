using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public MA_P2_SM_Controller ScreenManager;

    bool pausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }

    public void TogglePause()
    {
        pausado = !pausado;

        if (pausado)
            ActivarPausa();
        else
            DesactivarPausa();
    }

    public void ActivarPausa()
    {
        Time.timeScale = 0f;
        ScreenManager.Push("Pause");
    }

    public void DesactivarPausa()
    {
        Time.timeScale = 1f;
        ScreenManager.HideAll();
    }
}
