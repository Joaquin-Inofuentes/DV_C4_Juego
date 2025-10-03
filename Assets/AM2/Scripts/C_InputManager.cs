using UnityEngine;
using System;

public class C_InputManager : MonoBehaviour  // controller
{
    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2> OnPanInput;
    public event Action OnDisparar;
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

    
    public void InvokeDisparar()
    {
        OnDisparar?.Invoke();
    }

    public void RecibirRecargar() => Recargar?.Invoke();
    public void RecibirAgacharse() => Agacharse?.Invoke();
    public void RecibirLevantarse() => Levantarse?.Invoke();
    public void RecibirSaltar() => Saltar?.Invoke();

}
/*
Capa	Qué hace	¿Toca Unity?	Ejemplo
Model	
- Guarda y 
- gestiona los datos y 
- lógica del juego	
❌ No	
Vida, daño, energía, reglas de combate

Controller	
- Escucha inputs/eventos, 
- decide acciones, 
- modifica componentes	
✅ Sí	
Detecta input y aplica fuerza para saltar

View	
- Muestra lo visual, 
- UI, 
- animaciones, 
- efectos	
✅ Sí (solo estético)	
Animación de ataque, sonido de golpe





 */ 