using UnityEngine;
using UnityEngine.EventSystems;

public class M_Pan_Tactil : MonoBehaviour
{
    [SerializeField] private V_InputManager manager;
    [SerializeField] private float sensibilidad = 0.2f;

    [Header("Config")]
    [SerializeField] private bool usarMouse = true; // ✅ lo elegís en el inspector

    private Vector2 inicioTouch;
    private bool tocando = false;

    void Update()
    {
        if (usarMouse)
            ProcesarMouse();
        else
            ProcesarTouch();
    }

    // ------------------- TOUCH -------------------
    private void ProcesarTouch()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        switch (t.phase)
        {
            case TouchPhase.Began:
                if (!EventSystem.current.IsPointerOverGameObject(t.fingerId))
                    IniciarTouch(t.position);
                break;

            case TouchPhase.Moved:
                if (tocando) MoverTouch(t.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (tocando) FinalizarTouch();
                break;
        }
    }

    // ------------------- MOUSE -------------------
    private void ProcesarMouse()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            IniciarTouch(Input.mousePosition);

        if (Input.GetMouseButton(0) && tocando)
            MoverTouch(Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && tocando)
            FinalizarTouch();
    }

    // ------------------- MÉTODOS DE RESPONSABILIDAD -------------------
    private void IniciarTouch(Vector2 pos)
    {
        tocando = true;
        inicioTouch = pos;
    }

    private void MoverTouch(Vector2 pos)
    {
        Vector2 delta = (pos - inicioTouch) * sensibilidad;
        manager?.RecibirPan(delta);
        inicioTouch = pos;
        // Debug.Log($"Pan Delta: {delta}");
    }

    private void FinalizarTouch()
    {
        tocando = false;
        inicioTouch = Vector2.zero;
        manager?.RecibirPan(Vector2.zero);
    }
}
