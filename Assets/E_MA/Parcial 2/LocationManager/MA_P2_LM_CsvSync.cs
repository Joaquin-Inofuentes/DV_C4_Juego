using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MA_P2_LM_CsvSync
{
    public static void SincDictionary(
        ref Dictionary<string, Dictionary<string, string>> nestedDictionaryRamData,
        string absoluteCsvFilePath,
        bool csvHasPriorityOverRam = true)
    {

        if (absoluteCsvFilePath == "")
        {
            Debug.LogError("Usted. Debe asignar una ruta destino");
            return;
        }
        // =====================================================================================
        // SECTOR 1: VERIFICAR SI EL CSV EXISTE
        // =====================================================================================
        bool fileExists = File.Exists(absoluteCsvFilePath);

        if (!fileExists)
        {
            // Si el archivo no existe simplemente salvamos la RAM como CSV inicial
            SaveDictionaryAsCsv(absoluteCsvFilePath, nestedDictionaryRamData);
            return;
        }

        // =====================================================================================
        // SECTOR 2: LEER TODO EL CSV LÍNEA POR LÍNEA
        // =====================================================================================
        string[] allCsvLines = File.ReadAllLines(absoluteCsvFilePath);

        if (allCsvLines.Length == 0)
        {
            // CSV vacío → guardar RAM directamente
            SaveDictionaryAsCsv(absoluteCsvFilePath, nestedDictionaryRamData);
            return;
        }

        // =====================================================================================
        // SECTOR 3: LEER HEADERS DE LA PRIMERA LÍNEA
        // =====================================================================================
        string headerLine = allCsvLines[0];
        string[] csvColumnNames = headerLine.Split(';');

        // Diccionario que representa la tabla cargada desde el CSV
        Dictionary<string, Dictionary<string, string>> nestedDictionaryCsvData =
            new Dictionary<string, Dictionary<string, string>>();

        // Crear estructura vacía para cada columna del CSV
        for (int columnIndex = 0; columnIndex < csvColumnNames.Length; columnIndex++)
        {
            string columnName = csvColumnNames[columnIndex];

            if (!nestedDictionaryCsvData.ContainsKey(columnName))
            {
                nestedDictionaryCsvData[columnName] = new Dictionary<string, string>();
            }
        }

        // =====================================================================================
        // SECTOR 4: CARGAR FILAS DEL CSV EN LA ESTRUCTURA nestedDictionaryCsvData
        // =====================================================================================
        for (int lineIndex = 1; lineIndex < allCsvLines.Length; lineIndex++)
        {
            string currentLine = allCsvLines[lineIndex];

            if (string.IsNullOrWhiteSpace(currentLine))
                continue;

            string[] cellValues = currentLine.Split(';');

            string currentRowId = cellValues[0];

            for (int colIndex = 0; colIndex < csvColumnNames.Length; colIndex++)
            {
                string columnName = csvColumnNames[colIndex];

                string cellValue = "";
                if (colIndex < cellValues.Length)
                    cellValue = cellValues[colIndex];

                // Insertamos siempre el valor
                nestedDictionaryCsvData[columnName][currentRowId] = cellValue;
            }
        }

        // =====================================================================================
        // SECTOR 5: CONSTRUIR LA LISTA DE TODOS LOS HEADERS (COLUMNAS)
        // =====================================================================================
        List<string> listOfAllColumnNames = new List<string>();

        // Primero columnas del CSV
        foreach (var columnFromCsv in nestedDictionaryCsvData.Keys)
        {
            if (!listOfAllColumnNames.Contains(columnFromCsv))
                listOfAllColumnNames.Add(columnFromCsv);
        }

        // Luego columnas de RAM
        foreach (var columnFromRam in nestedDictionaryRamData.Keys)
        {
            if (!listOfAllColumnNames.Contains(columnFromRam))
                listOfAllColumnNames.Add(columnFromRam);
        }

        // =====================================================================================
        // SECTOR 6: CONSTRUIR LA LISTA DE TODOS LOS ROW IDs (FILAS)
        // =====================================================================================
        List<string> listOfAllRowIds = new List<string>();

        // IDs del CSV
        foreach (var csvColumnPair in nestedDictionaryCsvData)
        {
            foreach (var rowId in csvColumnPair.Value.Keys)
            {
                if (!listOfAllRowIds.Contains(rowId))
                    listOfAllRowIds.Add(rowId);
            }
        }

        // IDs de RAM
        foreach (var ramColumnPair in nestedDictionaryRamData)
        {
            foreach (var rowId in ramColumnPair.Value.Keys)
            {
                if (!listOfAllRowIds.Contains(rowId))
                    listOfAllRowIds.Add(rowId);
            }
        }

        // =====================================================================================
        // SECTOR 7: NORMALIZAR LA ESTRUCTURA EN AMBOS DICCIONARIOS
        // =====================================================================================
        foreach (string columnName in listOfAllColumnNames)
        {
            // Asegurar columna en RAM
            if (!nestedDictionaryRamData.ContainsKey(columnName))
                nestedDictionaryRamData[columnName] = new Dictionary<string, string>();

            // Asegurar columna en CSV
            if (!nestedDictionaryCsvData.ContainsKey(columnName))
                nestedDictionaryCsvData[columnName] = new Dictionary<string, string>();

            // Asegurar todas las filas
            foreach (string rowId in listOfAllRowIds)
            {
                if (!nestedDictionaryRamData[columnName].ContainsKey(rowId))
                    nestedDictionaryRamData[columnName][rowId] = "";

                if (!nestedDictionaryCsvData[columnName].ContainsKey(rowId))
                    nestedDictionaryCsvData[columnName][rowId] = "";
            }
        }

        // =====================================================================================
        // SECTOR 8: SINCRONIZACIÓN SEGÚN PRIORIDAD
        // =====================================================================================
        foreach (string columnName in listOfAllColumnNames)
        {
            foreach (string rowId in listOfAllRowIds)
            {
                string ramValue = nestedDictionaryRamData[columnName][rowId];
                string csvValue = nestedDictionaryCsvData[columnName][rowId];

                // Obtenemos el valor final que debería prevalecer
                string finalValue = "";

                if (csvHasPriorityOverRam)
                {
                    // CSV domina: Si el CSV tiene valor, úsalo. Si está vacío, usa el de RAM (relleno).
                    finalValue = !string.IsNullOrWhiteSpace(csvValue) ? csvValue : ramValue;
                }
                else
                {
                    // RAM domina: Si la RAM tiene valor, úsalo. Si está vacío, usa el de CSV (relleno).
                    finalValue = !string.IsNullOrWhiteSpace(ramValue) ? ramValue : csvValue;
                }

                // Aplicamos el valor final a ambos diccionarios para asegurar que RAM (que se guarda)
                // y CSV temporal (para consistencia) estén sincronizados.
                nestedDictionaryRamData[columnName][rowId] = finalValue;
                nestedDictionaryCsvData[columnName][rowId] = finalValue;
            }
        }
    
    

        // =====================================================================================
        // SECTOR 9: GUARDAR CSV FINAL UNIFICADO
        // =====================================================================================
        SaveDictionaryAsCsv(absoluteCsvFilePath, nestedDictionaryRamData);
    }

    // =========================================================================================
    // MÉTODO AUXILIAR SUPER LEGIBLE PARA GUARDAR EL CSV
    // =========================================================================================
    private static void SaveDictionaryAsCsv(
        string path,
        Dictionary<string, Dictionary<string, string>> nestedDictionary)
    {
        List<string> listOfColumnNames = new List<string>();
        foreach (var column in nestedDictionary.Keys)
            listOfColumnNames.Add(column);

        List<string> listOfRowIds = new List<string>();
        foreach (var column in nestedDictionary)
        {
            foreach (var id in column.Value.Keys)
            {
                if (!listOfRowIds.Contains(id))
                    listOfRowIds.Add(id);
            }
        }

        List<string> lines = new List<string>();

        // Header
        lines.Add(string.Join(";", listOfColumnNames));


        // Filas
        foreach (string id in listOfRowIds)
        {
            List<string> row = new List<string>();

            // No agregues el ID aquí.
            // Simplemente itera sobre todas las columnas (incluida la columna ID)
            foreach (var columnName in listOfColumnNames)
            {
                string value = "";
                // El TryGetValue de la columna 'ID' para la fila 'id' te dará el propio 'id'
                nestedDictionary[columnName].TryGetValue(id, out value);
                row.Add(value ?? "");
            }

            lines.Add(string.Join(";", row));
        }
        // ===================================

        File.WriteAllLines(path, lines);
    }
}
