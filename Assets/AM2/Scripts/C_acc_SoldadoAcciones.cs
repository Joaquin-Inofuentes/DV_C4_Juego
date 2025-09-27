using UnityEngine;

public class C_acc_SoldadoAcciones : MonoBehaviour // View 
{
    [SerializeField] private C_InputManager input;
    private void OnEnable()
    {
        if (input != null)
        {
            input.Disparar -= Disparar;
            input.Disparar += Disparar;

            input.Recargar -= Recargar;
            input.Recargar += Recargar;

            input.Agacharse -= Agacharse;
            input.Agacharse += Agacharse;

            input.Levantarse -= Levantarse;
            input.Levantarse += Levantarse;

            input.Saltar -= Saltar;
            input.Saltar += Saltar;
        }
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.Disparar -= Disparar;
            input.Recargar -= Recargar;
            input.Agacharse -= Agacharse;
            input.Levantarse -= Levantarse;
            input.Saltar -= Saltar;
        }
    }

    private void Disparar() => Debug.Log("Disparar");
    private void Recargar() => Debug.Log("Recargar");
    private void Agacharse() => Debug.Log("Agacharse");
    private void Levantarse() => Debug.Log("Levantarse");
    private void Saltar() => Debug.Log("Saltar");
}
