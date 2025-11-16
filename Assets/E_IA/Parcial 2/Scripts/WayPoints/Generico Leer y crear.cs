using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using CustomInspector; // Se mantiene, aunque el motor de sintaxis no lo requiere aquí

public class GenericoLeerycrear : MonoBehaviour
{
    [Button(nameof(OnEnable))]
    [Tooltip("La ruta completa de la carpeta que contiene los archivos .har")]
    public string rutaDeLaCarpeta = "";

    // El nombre de archivo de salida ahora es solo la extensión deseada
    [Tooltip("Extensión de salida para los archivos generados (ej: .txt)")]
    public string extensionArchivoSalida2 = ".txt";

    void OnEnable()
    {
        if (string.IsNullOrEmpty(rutaDeLaCarpeta) || !Directory.Exists(rutaDeLaCarpeta))
        {
            Debug.LogError($"❌ Error: La ruta '{rutaDeLaCarpeta}' es inválida o no existe.");
            return;
        }

        string[] harFiles = Directory.GetFiles(rutaDeLaCarpeta, "*.har");

        if (harFiles.Length == 0)
        {
            Debug.Log("ℹ️ No se encontraron archivos .har. Proceso finalizado.");
            return;
        }

        Debug.Log($"Iniciando procesamiento de {harFiles.Length} archivos HAR...");

        int totalUrlsVolcadas = 0;

        // 1. Procesar cada archivo HAR de forma individual
        foreach (string filePath in harFiles)
        {
            // --- CONFIGURACIÓN POR ARCHIVO ---
            string baseName = Path.GetFileNameWithoutExtension(filePath);
            string outputFilePath = Path.Combine(rutaDeLaCarpeta, baseName + extensionArchivoSalida2);

            // La lista de URLs se reinicia por cada archivo
            List<string> jdownloaderLines = new List<string>();
            HashSet<string> uniqueUrlsInFile = new HashSet<string>();

            Debug.Log($"\n-> Procesando: {baseName}");

            try
            {
                string content = File.ReadAllText(filePath);
                string[] lines = content.Split('\n');

                foreach (string line in lines)
                {
                    if (line.Contains("\"url\":") && line.Contains("https://"))
                    {
                        string[] parts = line.Split('"');

                        if (parts.Length > 3 && parts[3].StartsWith("https://"))
                        {
                            string url = parts[3];

                            // Limpieza final
                            if (url.EndsWith(","))
                                url = url.Substring(0, url.Length - 1);

                            if (uniqueUrlsInFile.Add(url)) // Solo añade si es una URL única en este archivo
                            {
                                string downloadName = $"{baseName}.dwg";

                                // Añadir el par: Nombre Deseado (Paquete/Archivo) y URL.
                                jdownloaderLines.Add(downloadName);
                                jdownloaderLines.Add(url);
                                totalUrlsVolcadas++;
                            }
                        }
                    }
                }

                // 2. Crear el archivo de salida individual (.urls.txt)
                if (jdownloaderLines.Count > 0)
                {
                    File.WriteAllLines(outputFilePath, jdownloaderLines);
                    Debug.Log($"✅ Éxito: {jdownloaderLines.Count / 2} URLs volcadas a {Path.GetFileName(outputFilePath)}");
                }
                else
                {
                    Debug.Log($"ℹ️ No se encontraron URLs válidas en {baseName}.");
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"❌ Fallo al leer o escribir el archivo {Path.GetFileName(filePath)}: {e.Message}");
            }
        }

        Debug.Log($"\n===============================================");
        Debug.Log($"PROCESO TERMINADO. Total de URLs extraídas: {totalUrlsVolcadas}");
        Debug.Log($"===============================================");
    }
}