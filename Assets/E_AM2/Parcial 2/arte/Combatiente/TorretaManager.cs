using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorretaManager : MonoBehaviour
{
    [Header("Prefab de la torreta")]
    public GameObject torretaPrefab;

    [Header("Zonas con sus 2 slots cada una")]
    public Transform[] zona1Slots = new Transform[2];
    public Transform[] zona2Slots = new Transform[2];
    public Transform[] zona3Slots = new Transform[2];

    private Transform[][] zonas;

    void Start()
    {
        // Cargar las zonas en un arreglo de zonas
        zonas = new Transform[][]
        {
            zona1Slots,
            zona2Slots,
            zona3Slots
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Comprado(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Comprado(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            Comprado(2);
    }

    /// <summary>
    /// Instancia una torreta en la primera ranura libre de la zona elegida.
    /// </summary>
    public void Comprado(int zonaIndex)
    {
        if (zonaIndex < 0 || zonaIndex >= zonas.Length)
            return;

        Transform[] slots = zonas[zonaIndex];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].childCount == 0)
            {
                GameObject nueva = Instantiate(torretaPrefab, slots[i].position, slots[i].rotation);
                nueva.transform.parent = slots[i]; // Se pega al slot
                return;
            }
        }

        Debug.Log("No hay slots vacíos en esta zona.");
    }
}

