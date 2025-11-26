using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AM_P2_Tienda_Monedero : MonoBehaviour
{

    public TextMeshProUGUI CantidadDeCurrency;
    private const int START_CURRENCY = 1000;
    private static AM_P2_Tienda_Monedero _instance;
    public static AM_P2_Tienda_Monedero Instance => _instance;


    public int CurrencyPublic = 0;
    private int currency;

    private void Awake()
    {
        _instance = this;
        currency = PlayerPrefs.GetInt("Currency", START_CURRENCY);
    }
    private void OnEnable()
    {
        _instance = this;
        currency = PlayerPrefs.GetInt("Currency", START_CURRENCY);
    }

    public void Update()
    {
        CurrencyPublic = currency;
        if (CantidadDeCurrency != null)
        {
            CantidadDeCurrency.text = "$" + currency;
        }
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        SaveCurrency();
    }

    public void SubtractCurrency(int amount)
    {
        currency -= amount;
        SaveCurrency();
    }

    public void ResetCurrency()
    {
        currency = START_CURRENCY;
        SaveCurrency();
    }

    public int GetCurrency() => currency;

    private void SaveCurrency()
    {
        PlayerPrefs.SetInt("Currency", currency);
        PlayerPrefs.Save();
    }
}