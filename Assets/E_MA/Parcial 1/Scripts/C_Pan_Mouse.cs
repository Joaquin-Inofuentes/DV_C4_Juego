using UnityEngine;
using CustomInspector;

public class C_Pan_Mouse : MonoBehaviour
{
    [Button(nameof(CambiarBloqueo))]

    [SerializeField] private C_InputManager manager;
    [SerializeField] private float sensibilidad = 0.2f;
    [SerializeField] private bool bloquearCursor = true;



    void Update()
    {

        float dx = Input.GetAxisRaw("Mouse X") * sensibilidad;
        float dy = Input.GetAxisRaw("Mouse Y") * sensibilidad;

        Vector2 delta = new Vector2(dx, dy);


        if (delta != Vector2.zero)
        {
            manager?.RecibirPan(delta);
            //Debug.Log($"Pan Mouse Delta: {delta}");
        }
        else
        {
            manager?.RecibirPan(Vector2.zero);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            CambiarBloqueo();
        }
    }
    [ContextMenu("CambiarBloqueo")]
    public void CambiarBloqueo()
    {
        bloquearCursor = !bloquearCursor;
        if (bloquearCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
