using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class IdiomaData
{
    public string nombreIdioma;
    public List<string> valores = new List<string>();
}

public class MA_P2_LocalizationManager_ExportaIDs : MonoBehaviour
{
    [Button(nameof(ExportarIDs))]
    [Button(nameof(ImportarCSV))]
    [Button(nameof(Translate),true)]
    public string Idioma = "";
    public string RutaLocalDestino = "";

    MA_P2_Location_GetTexts mA_P2_Location_GetTexts;



    public void ExportarIDs()
    {
        if (File.Exists(RutaLocalDestino))
        {
            // Aseguramos referencia
            if (mA_P2_Location_GetTexts == null)
                mA_P2_Location_GetTexts = FindObjectOfType<MA_P2_Location_GetTexts>();

            if (mA_P2_Location_GetTexts == null)
            {
                Debug.LogError("No se encontró MA_P2_Location_GetTexts en la escena.");
                return;
            }

            // Construimos el CSV
            System.Text.StringBuilder csv = new System.Text.StringBuilder();

            // Encabezado
            csv.AppendLine("ID;TextoOriginal");

            // --- Legacy Text ---
            foreach (var t in mA_P2_Location_GetTexts.legacyTexts)
            {
                if (t != null)
                {
                    string id = t.gameObject.name;
                    string texto = t.text.Replace(";", ","); // evita romper CSV

                    csv.AppendLine($"{id};{texto}");
                }
            }

            // --- TMP UGUI ---
            foreach (var t in mA_P2_Location_GetTexts.tmpGUITexts)
            {
                if (t != null)
                {
                    string id = t.gameObject.name;
                    string texto = t.text.Replace(";", ",");

                    csv.AppendLine($"{id};{texto}");
                }
            }

            // --- TMP 3D ---
            foreach (var t in mA_P2_Location_GetTexts.tmp3DTexts)
            {
                if (t != null)
                {
                    string id = t.gameObject.name;
                    string texto = t.text.Replace(";", ",");

                    csv.AppendLine($"{id};{texto}");
                }
            }

            // Guarda el archivo
            File.WriteAllText(RutaLocalDestino, csv.ToString(), System.Text.Encoding.UTF8);

            Debug.Log($"🟢 Exportación completada. Guardado en: {RutaLocalDestino}");
        }
        else
        {
            Debug.LogError("Indique una ruta destino en inspector");
        }
    }













    /*
    // Exporta data a CSV
    public void ExportarIDs()
    {
        if (File.Exists(RutaLocalDestino))
        {
            // Obtiene los TMPPro     public List<TextMeshProUGUI> tmpGUITexts = new List<TextMeshProUGUI>();

            // Obtiene los ID de esos elementos q son los nombres de esos gameobjects

            // Obtiene los valores actuales Originales

            // Crea un string de formato CSV ; y \n

            // Guarda el string en la ruta de la variable
        }
        else
        {
            Debug.LogError("Indique una ruta destino en inspector");
        }
    }
    */

















    [Header("IDs (columna 1)")]
    public List<string> ids = new List<string>();

    [Header("Idiomas → Listas alineadas")]
    public List<IdiomaData> idiomas = new List<IdiomaData>();

    // =========================================================
    //   IMPORTAR CSV
    // =========================================================

    [ContextMenu("📥 Importar CSV")]
    public void ImportarCSV()
    {
        if (!File.Exists(RutaLocalDestino))
        {
            Debug.LogError("No existe archivo CSV en: " + RutaLocalDestino);
            return;
        }

        string[] lineas = File.ReadAllLines(RutaLocalDestino);

        if (lineas.Length < 2)
        {
            Debug.LogError("CSV vacío o mal formateado");
            return;
        }

        ids.Clear();
        idiomas.Clear();

        // ---- 1. Leer encabezado ----
        string[] columnas = lineas[0].Split(';');

        // columnas[0] = ID
        // columnas[1..N] = idiomas

        for (int i = 1; i < columnas.Length; i++)
        {
            IdiomaData lang = new IdiomaData();
            lang.nombreIdioma = columnas[i];
            idiomas.Add(lang);
        }

        // ---- 2. Leer datos ----
        for (int i = 1; i < lineas.Length; i++)
        {
            string[] cols = lineas[i].Split(';');

            if (cols.Length != columnas.Length)
                continue;

            // Guardar ID
            ids.Add(cols[0]);

            // Agregar valores a cada idioma
            for (int c = 1; c < columnas.Length; c++)
            {
                idiomas[c - 1].valores.Add(cols[c]);
            }
        }

        Debug.Log("📥 CSV importado. Filas: " + ids.Count + "   Idiomas: " + idiomas.Count);
    }














    public void Translate(string idioma)
    {
        // 1. Buscar idioma
        IdiomaData lang = idiomas.Find(x => x.nombreIdioma == idioma);

        // 2. Obtener textos en escena
        MA_P2_Location_GetTexts textos = FindObjectOfType<MA_P2_Location_GetTexts>();

        if (textos == null)
        {
            Debug.LogError("⚠ No se encontró MA_P2_Location_GetTexts en la escena.");
            return;
        }

        bool idiomaEncontrado = lang != null;

        if (!idiomaEncontrado)
        {
            Debug.LogWarning("⚠ Idioma no encontrado: " + idioma + " → Se dejarán todos los textos vacíos.");
        }

        // ---------------------------------------------
        // CAMBIAR TEXTOS
        // ---------------------------------------------

        // --- Legacy TEXT ---
        foreach (var t in textos.legacyTexts)
        {
            if (t != null)
            {
                string id = t.gameObject.name;

                if (idiomaEncontrado)
                {
                    int index = ids.IndexOf(id);
                    t.text = (index >= 0 && index < lang.valores.Count)
                        ? lang.valores[index]
                        : "NOREF";
                }
                else
                {
                    t.text = "NOREF"; // o null
                }
            }
        }

        // --- TMP UGUI ---
        foreach (var t in textos.tmpGUITexts)
        {
            if (t != null)
            {
                string id = t.gameObject.name;

                if (idiomaEncontrado)
                {
                    int index = ids.IndexOf(id);
                    t.text = (index >= 0 && index < lang.valores.Count)
                        ? lang.valores[index]
                        : "NOREF";
                }
                else
                {
                    t.text = "NOREF";
                }
            }
        }

        // --- TMP 3D ---
        foreach (var t in textos.tmp3DTexts)
        {
            if (t != null)
            {
                string id = t.gameObject.name;

                if (idiomaEncontrado)
                {
                    int index = ids.IndexOf(id);
                    t.text = (index >= 0 && index < lang.valores.Count)
                        ? lang.valores[index]
                        : "NOREF";
                }
                else
                {
                    t.text = "NOREF";
                }
            }
        }

        Debug.Log("🌍 Idioma aplicado (o limpiado): " + idioma);
    }

}
