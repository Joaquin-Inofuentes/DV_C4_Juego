using CustomInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_BusEvent_Manager : MonoBehaviour
{
    [Button(nameof(BuscarAgentesEnEscena))]
    public string botonBuscarAgentes;

    public static List<IA_P2_FSM> agentes = new List<IA_P2_FSM>();

    public static Action<GameObject> OnEnemyFound;

    public static Vector3 ultimaPosicionVisto;
    public static GameObject ultimoEnemigoVisto;

    private void OnEnable()
    {
        BuscarAgentesEnEscena();
    }

    public static void BuscarAgentesEnEscena()
    {
        agentes.Clear();
        IA_P2_FSM[] encontrados = FindObjectsOfType<IA_P2_FSM>();

        for (int i = 0; i < encontrados.Length; i++)
            agentes.Add(encontrados[i]);
    }

    public static void NotificarEncontrado(GameObject enemigo,string NombreDelOrigen)
    {
        ultimoEnemigoVisto = enemigo;
        ultimaPosicionVisto = enemigo.transform.position;
        OnEnemyFound?.Invoke(enemigo);
        Debug.Log("Se notifico q se encontro al enemigo");
    }


    void Update()
    {

    }
}
