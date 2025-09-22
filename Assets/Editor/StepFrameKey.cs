using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class StepFrameKey
{
    static StepFrameKey()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (!EditorApplication.isPlaying || !EditorApplication.isPaused)
            return;

        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backslash) // "|" = Shift + \
        {
            EditorApplication.Step();
            e.Use();
        }
    }
}
