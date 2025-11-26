using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID
{
    AM2_Principal,
    MainMenu,
    EscenaVictoria,
    EscenaDerrota
}

public class UI_SceneManager : MonoBehaviour
{
    public void LoadSceneByID(SceneID id)
    {
        Debug.Log("Cargando escena: " + id);
        SceneManager.LoadScene(id.ToString());
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}