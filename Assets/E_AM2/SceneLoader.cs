using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByIndex(int buildIndex)
    {
        Debug.Log("se intento ir a la escena" + buildIndex);
        return;
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Index fuera de rango: {buildIndex}");
            return;
        }
        Debug.Log($"Cargando escena index: {buildIndex}");
        SceneManager.LoadScene(buildIndex);
    }

    // Recarga la escena actual
    public void ReloadScene()
    {
        Debug.Log("Se intento re cargar la escena");
        return;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    // Cerrar aplicación (en editor solo loggea)
    public void QuitGame()
    {
        Debug.Log("Quit Game (si estuviera en build se cerraría)");
        Application.Quit();
    }
}
