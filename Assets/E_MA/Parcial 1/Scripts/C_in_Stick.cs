using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class C_in_Stick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform fondo;
    [SerializeField] private RectTransform handle;

    [Header("Salida")]
    [SerializeField] private C_InputManager Input; // referencia al manager
    public event Action<Vector2> OnStickFloat;

    private Vector2 inputVector;
    private bool presionado;

    void Awake()
    {
        if (!fondo || !handle)
            Debug.LogError("C_in_Stick: Falta asignar fondo/handle en el inspector.", this);

        // Recomendación fuerte: pivots centrados para que todo sea coherente
        // (igual el cálculo ya contempla el rect, pero esto evita sorpresas).
        // fondo.pivot = new Vector2(0.5f, 0.5f);
        // handle.pivot = new Vector2(0.5f, 0.5f);
    }

    void Update()
    {
        if (!presionado) return;

        OnStickFloat?.Invoke(inputVector);
        Input?.RecibirMove(inputVector);
    }

    private void UpdateStickFromScreen(Vector2 screenPos, Camera eventCam)
    {
        if (!fondo || !handle) return;

        // 1) Punto local dentro del rect (origen = pivot del fondo)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            fondo, screenPos, eventCam, out Vector2 localPosPivot
        );

        // 2) Convertir a "desde el centro real" del rect
        // localPosPivot está respecto al pivot. Si pivot no es 0.5,0.5, esto lo corrige.
        Rect r = fondo.rect;
        Vector2 centroDesdePivot = new Vector2(
            Mathf.Lerp(r.xMin, r.xMax, 0.5f),
            Mathf.Lerp(r.yMin, r.yMax, 0.5f)
        );

        Vector2 localFromCenter = localPosPivot - centroDesdePivot;

        // 3) Radios reales desde el centro (semi-ancho y semi-alto)
        float radiusX = r.width * 0.5f;
        float radiusY = r.height * 0.5f;

        // 4) Normalizar [-1..1] por eje
        Vector2 normalized = new Vector2(
            (radiusX <= 0.0001f) ? 0f : localFromCenter.x / radiusX,
            (radiusY <= 0.0001f) ? 0f : localFromCenter.y / radiusY
        );

        // 5) Clamp circular
        if (normalized.magnitude > 1f) normalized = normalized.normalized;

        inputVector = normalized;

        // 6) Mover handle desde el centro
        handle.anchoredPosition = new Vector2(inputVector.x * radiusX, inputVector.y * radiusY);
    }

    public void OnDrag(PointerEventData e)
    {
        UpdateStickFromScreen(e.position, e.pressEventCamera);
    }

    public void OnPointerDown(PointerEventData e)
    {
        presionado = true;
        UpdateStickFromScreen(e.position, e.pressEventCamera);
    }

    public void OnPointerUp(PointerEventData e)
    {
        presionado = false;

        // Reset visual + data
        inputVector = Vector2.zero;
        if (handle) handle.anchoredPosition = Vector2.zero;

        // IMPORTANTE: avisar STOP (sino queda “pegado”)
        OnStickFloat?.Invoke(Vector2.zero);
        Input?.RecibirMove(Vector2.zero);
    }
}