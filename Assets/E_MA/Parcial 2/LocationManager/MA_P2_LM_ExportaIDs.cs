using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class LenguageDate
{
    public string NameOfLenguage;
    public List<string> Values = new List<string>();
}

public class MA_P2_LM_ExportaIDs : MonoBehaviour
{
    [Button(nameof(SyncDataTranslate))]
    [Button(nameof(Reseteng))]
    [Button(nameof(Translate), true)]
    public string Idioma = "";
    public string RutaLocalDestino = "";

    public MA_P2_LM_GetTexts mA_P2_Location_GetTexts;

    public Dictionary<string, Dictionary<string, string>> nestedDictionaryRamData =
        new Dictionary<string, Dictionary<string, string>>();

    public void SyncDataTranslate()
    {
        Debug.Log("Se usara este CSV para traducciones : " + RutaLocalDestino);
        if(RutaLocalDestino == "")
        {
            Debug.LogError("[SincronizarData] Fallo error 404. Debe asignar una ruta");
            return;
        }
        if(mA_P2_Location_GetTexts == null)
        {
            Debug.LogError("Falta asociar el componente de gettext");
            return;
        }
        mA_P2_Location_GetTexts.BuscarTextos();
        RefreshOfTheRAM();
        MA_P2_LM_CsvSync.SincDictionary(ref nestedDictionaryRamData, RutaLocalDestino, true);
        RefreshViewerInspector(nestedDictionaryRamData);
        Translate(Idioma);
    }

    public void RefreshViewerInspector(
    Dictionary<string, Dictionary<string, string>> nestedDictionaryRamData)
    {
        // --------------------------------------------------------------------
        // OBJETIVO:
        // - Generar lista "ids" con los nombres REALES (keys internas)
        // - Cada idioma tendrá SOLO los valores reales
        // - Nada de concatenar columnas
        // --------------------------------------------------------------------

        ids.Clear();
        Lenguage.Clear();

        if (nestedDictionaryRamData == null || nestedDictionaryRamData.Count == 0)
            return;

        // ------------------------------------------------------------
        // 1) juntar todos los ids de todos los idiomas sin duplicados
        // ------------------------------------------------------------
        List<string> listaDeIds = new List<string>();

        foreach (var idioma in nestedDictionaryRamData)
        {
            foreach (var par in idioma.Value)
            {
                string id = par.Key;

                if (!string.IsNullOrEmpty(id) && !listaDeIds.Contains(id))
                    listaDeIds.Add(id);
            }
        }

        // ordenar opcional (para consistencia visual)
        listaDeIds.Sort();

        // guardar en visor
        foreach (string id in listaDeIds)
            ids.Add(id);

        // ------------------------------------------------------------
        // 2) construir las columnas (cada idioma)
        // ------------------------------------------------------------
        foreach (var idiomaEntry in nestedDictionaryRamData)
        {
            string nombreIdioma = idiomaEntry.Key;
            Dictionary<string, string> filas = idiomaEntry.Value;

            LenguageDate idiomaData = new LenguageDate();
            idiomaData.NameOfLenguage = nombreIdioma;

            // por cada id ya ordenado cargar su traducción
            foreach (string id in listaDeIds)
            {
                string valor = "";

                if (filas.TryGetValue(id, out string valEncontrado))
                    valor = valEncontrado;

                idiomaData.Values.Add(valor);
            }

            Lenguage.Add(idiomaData);
        }
    }





    // Asegúrate de tener 'using TMPro;' arriba si usas TextMeshPro
    public void RefreshOfTheRAM()
    {
        // =================================================================
        // SECCIÓN 1: PREPARACIÓN Y VERIFICACIÓN (Paso 1 de 3)
        // =================================================================

        const string CLAVE_COLUMNA_ID = "ID";
        const string CLAVE_COLUMNA_TEXTO = "TextoOriginal";

        // 1. Verificar la referencia al script de textos
        if (mA_P2_Location_GetTexts == null)
            mA_P2_Location_GetTexts = FindObjectOfType<MA_P2_LM_GetTexts>();

        if (mA_P2_Location_GetTexts == null)
        {
            Debug.LogError("No se encontró el administrador de textos 'MA_P2_Location_GetTexts'.");
            return;
        }

        // 2. Limpiar e inicializar la estructura en RAM
        nestedDictionaryRamData.Clear();

        // Inicializamos la columna para los IDs y la columna para el Texto Original.
        nestedDictionaryRamData[CLAVE_COLUMNA_ID] = new Dictionary<string, string>();
        nestedDictionaryRamData[CLAVE_COLUMNA_TEXTO] = new Dictionary<string, string>();

        // -----------------------------------------------------------------

        // =================================================================
        // SECCIÓN 2: ESCANEO DE ESCENA Y CARGA DE RAM (Paso 2 de 3)
        // =================================================================

        string idObjeto;
        string textoComponente;
        string textoLimpio;

        // ---------------------------------------------
        // 2.1 ESCANEAR TEXTOS LEGACY (UnityEngine.UI.Text)
        // ---------------------------------------------
        foreach (UnityEngine.UI.Text t in mA_P2_Location_GetTexts.legacyTexts)
        {
            if (t == null) continue;

            idObjeto = t.gameObject.name;
            textoComponente = t.text;
            textoLimpio = textoComponente.Replace(";", ",").Trim();

            // Almacenamos el ID y el TextoOriginal en RAM
            nestedDictionaryRamData[CLAVE_COLUMNA_ID][idObjeto] = idObjeto;
            nestedDictionaryRamData[CLAVE_COLUMNA_TEXTO][idObjeto] = textoLimpio;
        }

        // ---------------------------------------------
        // 2.2 ESCANEAR TEXTOS TMP UGUI (TMPro.TextMeshProUGUI)
        // ---------------------------------------------
        foreach (TMPro.TextMeshProUGUI t in mA_P2_Location_GetTexts.tmpGUITexts)
        {
            if (t == null) continue;

            idObjeto = t.gameObject.name;
            textoComponente = t.text;
            textoLimpio = textoComponente.Replace(";", ",").Trim();

            // Almacenamos el ID y el TextoOriginal en RAM
            nestedDictionaryRamData[CLAVE_COLUMNA_ID][idObjeto] = idObjeto;
            nestedDictionaryRamData[CLAVE_COLUMNA_TEXTO][idObjeto] = textoLimpio;
        }

        // ---------------------------------------------
        // 2.3 ESCANEAR TEXTOS TMP 3D (TMPro.TextMeshPro)
        // ---------------------------------------------
        foreach (TMPro.TextMeshPro t in mA_P2_Location_GetTexts.tmp3DTexts)
        {
            if (t == null) continue;

            idObjeto = t.gameObject.name;
            textoComponente = t.text;
            textoLimpio = textoComponente.Replace(";", ",").Trim();

            // Almacenamos el ID y el TextoOriginal en RAM
            nestedDictionaryRamData[CLAVE_COLUMNA_ID][idObjeto] = idObjeto;
            nestedDictionaryRamData[CLAVE_COLUMNA_TEXTO][idObjeto] = textoLimpio;
        }

        // -----------------------------------------------------------------

        // =================================================================
        // SECCIÓN 3: ACTUALIZAR EL VISOR (Paso 3 de 3)
        // =================================================================

        // Reflejamos los datos recién cargados de la RAM en las listas visibles del Inspector.
        RefreshViewerInspector(nestedDictionaryRamData);

        //Debug.Log($"🟢 Escaneo de escena y carga a RAM completado. {ids.Count} IDs encontrados y cargados al visor.");
    }




    public void Reseteng()
    {
        ids.Clear();
        Lenguage.Clear();
        nestedDictionaryRamData.Clear();
    }





    [Header("IDs (columna 1)")]
    public List<string> ids = new List<string>();

    [Header("Idiomas → Listas alineadas")]
    public List<LenguageDate> Lenguage = new List<LenguageDate>();



    public void Translate(string idioma)
    {
        if (!nestedDictionaryRamData.ContainsKey(idioma))
        {
            Debug.LogError($"No existe el idioma {idioma}. Reviselo");
            return;
        }
        // 1. Buscar idioma
        LenguageDate lang = Lenguage.Find(x => x.NameOfLenguage == idioma);

        // 2. Obtener textos en escena
        MA_P2_LM_GetTexts textos = FindObjectOfType<MA_P2_LM_GetTexts>();

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
                    t.text = (index >= 0 && index < lang.Values.Count)
                        ? lang.Values[index]
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
                    t.text = (index >= 0 && index < lang.Values.Count)
                        ? lang.Values[index]
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
                    t.text = (index >= 0 && index < lang.Values.Count)
                        ? lang.Values[index]
                        : "NOREF";
                }
                else
                {
                    t.text = "NOREF";
                }
            }
        }
        Idioma = idioma;
        Debug.Log("🌍 Set Lenguage : " + idioma);
    }

}
