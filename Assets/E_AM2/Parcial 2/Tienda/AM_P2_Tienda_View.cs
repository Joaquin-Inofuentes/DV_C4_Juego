using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AM_P2_Tienda_View : MonoBehaviour
{
    [Button(nameof(ActualizarInterfaz))]
    public TextMeshProUGUI CurrencyActualText;

    public AM_P2_Tienda_Items Items;

    public TextMeshProUGUI DañoPorProyectil;
    public TextMeshProUGUI VelocidadDeRecargaText;
    public TextMeshProUGUI SoldadosMaximosText;

    public void ActualizarInterfaz()
    {
        if (
            DañoPorProyectil != null
            &&
            VelocidadDeRecargaText != null
            &&
            VelocidadDeRecargaText != null
            &&
            CurrencyActualText != null
            )
        {
            DañoPorProyectil.text = Items.DañoProyectil.ToString();
            VelocidadDeRecargaText.text = Items.VelocidadDeRecarga.ToString();
            SoldadosMaximosText.text = Items.SoldadosMaximos.ToString();
            if (AM_P2_Tienda_Monedero.Instance == null)
            {
                Invoke(nameof(ActualizarInterfaz), 0.2f);
                return;
            }
            CurrencyActualText.text = "$" + AM_P2_Tienda_Monedero.Instance.GetCurrency().ToString();
        }
    }
}
