using UnityEngine;
using System;

public class C_InputManager : MonoBehaviour  // controller
{
    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2> OnPanInput;
    public event Action<int> OnCambiarArma;
    public event Action OnDisparar;
    public event Action Recargar;
    public event Action Agacharse;
    public event Action Levantarse;
    public event Action Saltar;

    [SerializeField] private bool usarTactil = true; // decide cuál input usar

    public static C_InputManager Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
    }
    public void OnEnable()
    {
        Instance = this;
    }

    public void RecibirMove(Vector2 input)
    {
        OnMoveInput?.Invoke(input);
    }

    public void RecibirPan(Vector2 input)
    {
        OnPanInput?.Invoke(input);
    }

    
    public void InvokeDisparar()
    {
        OnDisparar?.Invoke();
    }

    public void RecibirRecargar() => Recargar?.Invoke();
    public void RecibirAgacharse() => Agacharse?.Invoke();
    public void RecibirLevantarse() => Levantarse?.Invoke();
    public void RecibirSaltar() => Saltar?.Invoke();

    public void RecibirCambioDeArma(int Value) => OnCambiarArma?.Invoke(Value);

}