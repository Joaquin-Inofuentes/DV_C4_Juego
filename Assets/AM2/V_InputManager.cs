using UnityEngine;
using System;

public class V_InputManager : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2> OnPanInput;
    public event Action Disparar;
    public event Action Recargar;
    public event Action Agacharse;
    public event Action Levantarse;
    public event Action Saltar;

    [SerializeField] private bool usarTactil = true; // decide cuál input usar


    public void RecibirMove(Vector2 input)
    {
        OnMoveInput?.Invoke(input);
    }

    public void RecibirPan(Vector2 input)
    {
        OnPanInput?.Invoke(input);
    }

    public void RecibirDisparar() => Disparar?.Invoke();
    public void RecibirRecargar() => Recargar?.Invoke();
    public void RecibirAgacharse() => Agacharse?.Invoke();
    public void RecibirLevantarse() => Levantarse?.Invoke();
    public void RecibirSaltar() => Saltar?.Invoke();

}
