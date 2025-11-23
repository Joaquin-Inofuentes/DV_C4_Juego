using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MA_P2_ST_ItemManager : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>(); // tus 5 objetos
    public MA_P2_ST_PlayerArmas playerArmas; // referencia al script del jugador
    public C_InputManager inputManager;

    private void OnEnable()
    {
        items = new List<GameObject>();
        inputManager.OnCambiarArma += SetActiveItem;
        foreach (Transform child in transform)
        {
            items.Add(child.gameObject);
        }
    }


    public void SetActiveItem(int index)
    {
        Debug.Log("Se recibio este indice de cambio de arma: " + index);


        // Revisa si es un pedido de siguiente o anterior
        if(index > 100)
        {
            // Siguiente arma
            // Obtiene el arma actual activa
            int currentIndex = items.FindIndex(item => item.activeSelf);
            // Obtiene la siguiente de la lista
            int nextIndex = (currentIndex + 1) % items.Count;

            index = nextIndex;
        }
        else if(index < -100)
        {
            // Arma anterior
            // Obtiene el arma actual activa
            int currentIndex = items.FindIndex(item => item.activeSelf);
            // Obtiene la siguiente de la lista
            int previousIndex = (currentIndex - 1 + items.Count) % items.Count;
            index = previousIndex;
        }


        // Asigna ese indice como activo
        playerArmas.armaActualComponente = null;
        // Desactivar todos
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetActive(false);
        }

        // Si index es válido → activar solo ese
        if (index >= 0 && index < items.Count)
        {
            items[index].SetActive(true);
            if (items[index].GetComponent<MA_P2_ST_C_IWeapon>() != null && items[index].activeSelf)
            {
                playerArmas.armaActualComponente = 
                    items[index].GetComponent<MA_P2_ST_C_IWeapon>() as MonoBehaviour;
                playerArmas.CambiarArma(
                    items[index].GetComponent<MA_P2_ST_C_IWeapon>());
            }
        }
        Debug.Log("Se cambio a el arma con indice: " + index + " | " + items[index].name, items[index]);
    }

    public void ClearAll()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetActive(false);
        }
        playerArmas.armaActualComponente = null;
    }
}
