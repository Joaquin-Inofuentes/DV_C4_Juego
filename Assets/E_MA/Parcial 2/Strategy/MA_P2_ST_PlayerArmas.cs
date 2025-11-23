using UnityEngine;

public class MA_P2_ST_PlayerArmas : MonoBehaviour
{
    [SerializeField] public MonoBehaviour armaActualComponente;
    public MA_P2_ST_C_IWeapon armaActual;

    private void OnEnable()
    {
        armaActual = armaActualComponente as MA_P2_ST_C_IWeapon;

        if (armaActual == null)
            Debug.LogError("El componente asignado NO implementa IWeapon.");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            armaActual?.Disparar();

        if (Input.GetKeyDown(KeyCode.R))
            armaActual?.Recargar();
    }

    public void CambiarArma(MA_P2_ST_C_IWeapon nueva)
    {
        armaActual = nueva;
    }
}
