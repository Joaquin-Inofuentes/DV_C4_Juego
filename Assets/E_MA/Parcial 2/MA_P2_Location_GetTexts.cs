using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CustomInspector;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class FindTextsInScene : MonoBehaviour
{
    [Button(nameof(BuscarTextos))]
    public string Algo1;

    [Header("Text Legacy (UI antiguo)")]
    public List<Text> legacyTexts = new List<Text>();

    [Header("TextMeshPro UGUI (UI Canvas)")]
    public List<TextMeshProUGUI> tmpGUITexts = new List<TextMeshProUGUI>();

    [Header("TextMeshPro (3D Mesh)")]
    public List<TextMeshPro> tmp3DTexts = new List<TextMeshPro>();

#if UNITY_EDITOR
    [ContextMenu("Buscar todos los textos")]
    public void BuscarTextos()
    {
        legacyTexts = new List<Text>(Resources.FindObjectsOfTypeAll<Text>());
        tmpGUITexts = new List<TextMeshProUGUI>(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>());
        tmp3DTexts = new List<TextMeshPro>(Resources.FindObjectsOfTypeAll<TextMeshPro>());

        Debug.Log($"🟢 Encontrados: Legacy = {legacyTexts.Count}, TMP UGUI = {tmpGUITexts.Count}, TMP 3D = {tmp3DTexts.Count}");
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(FindTextsInScene))]
public class FindTextsInSceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FindTextsInScene script = (FindTextsInScene)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("🔍 Buscar todos los textos en la escena"))
        {
            script.BuscarTextos();
        }
    }
}
#endif
