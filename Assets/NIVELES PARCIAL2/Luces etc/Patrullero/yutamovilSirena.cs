using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yutamovilSirena : MonoBehaviour
{
    [Header("Objetos de luces")]
    public GameObject luzRoja;
    public GameObject luzAzul;

    [Header("Configuración")]
    public float intervalo = 0.2f;

    private float timer;

    private void Start()
    {
        // Estado inicial
        luzRoja.SetActive(true);
        luzAzul.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= intervalo)
        {
            Alternar();
            timer = 0f;
        }
    }

    void Alternar()
    {
        bool rojaActiva = luzRoja.activeSelf;

        luzRoja.SetActive(!rojaActiva);
        luzAzul.SetActive(rojaActiva);
    }
}
