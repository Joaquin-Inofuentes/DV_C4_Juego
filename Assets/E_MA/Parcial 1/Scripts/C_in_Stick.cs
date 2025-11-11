using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class C_in_Stick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform fondo, handle;


    private Vector2 inputVector;
    private bool presionado;

    public event Action<Vector2> OnStickFloat;
    [SerializeField] private C_InputManager Input; // referencia al manager

    private void Update()
    {
        if (presionado)
        {
            // envía constantemente el vector actual
            OnStickFloat?.Invoke(inputVector);
            Input?.RecibirMove(inputVector);
        }
    }

    private void UpdateStick(Vector2 pos)
    {
        float radiusX = fondo.sizeDelta.x * 0.5f;
        float radiusY = fondo.sizeDelta.y * 0.5f;

        Vector2 normalized = new Vector2(pos.x / radiusX, pos.y / radiusY);

        if (normalized.magnitude > 1f) normalized = normalized.normalized;

        inputVector = normalized;

        handle.anchoredPosition = new Vector2(inputVector.x * radiusX, inputVector.y * radiusY);
    }

    public void OnDrag(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(fondo, e.position, e.pressEventCamera, out var localPos);
        UpdateStick(localPos);
    }

    public void OnPointerDown(PointerEventData e)
    {
        presionado = true;
        OnDrag(e);
    }

    public void OnPointerUp(PointerEventData e)
    {
        presionado = false;
        UpdateStick(Vector2.zero);
    }
}
