using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using CustomInspector;

public class FindAllTexts : EditorWindow
{
    [Button(nameof(FindAllTexts))]
    private List<Text> legacyTexts;
    private List<TextMeshProUGUI> tmpGUITexts;
    private List<TextMeshPro> tmp3DTexts;

    [MenuItem("Tools/Find All Texts")]
    public static void ShowWindow()
    {
        GetWindow<FindAllTexts>("Find All Texts");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Buscar todos los textos en la escena"))
        {
            BuscarTextos();
        }

        EditorGUILayout.Space(10);

        if (legacyTexts != null)
        {
            EditorGUILayout.LabelField($"Text Legacy encontrados: {legacyTexts.Count}");
            foreach (var t in legacyTexts)
                EditorGUILayout.ObjectField(t.name, t, typeof(Text), true);
        }

        EditorGUILayout.Space(10);

        if (tmpGUITexts != null)
        {
            EditorGUILayout.LabelField($"TextMeshProUGUI encontrados: {tmpGUITexts.Count}");
            foreach (var t in tmpGUITexts)
                EditorGUILayout.ObjectField(t.name, t, typeof(TextMeshProUGUI), true);
        }

        EditorGUILayout.Space(10);

        if (tmp3DTexts != null)
        {
            EditorGUILayout.LabelField($"TextMeshPro (3D) encontrados: {tmp3DTexts.Count}");
            foreach (var t in tmp3DTexts)
                EditorGUILayout.ObjectField(t.name, t, typeof(TextMeshPro), true);
        }
    }

    private void BuscarTextos()
    {
        // Encuentra todos los objetos activos e inactivos en la escena
        legacyTexts = new List<Text>(Resources.FindObjectsOfTypeAll<Text>());
        tmpGUITexts = new List<TextMeshProUGUI>(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>());
        tmp3DTexts = new List<TextMeshPro>(Resources.FindObjectsOfTypeAll<TextMeshPro>());

        Debug.Log($"Text Legacy: {legacyTexts.Count}, TMP UGUI: {tmpGUITexts.Count}, TMP 3D: {tmp3DTexts.Count}");
    }
}
