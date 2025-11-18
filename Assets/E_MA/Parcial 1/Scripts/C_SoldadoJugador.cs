using UnityEngine;

public class C_SoldadoJugador : C_SoldadoTransform,I_ReceivesDamage
{
    [Header("Jugador")]
    [SerializeField] private Transform camara;
    [SerializeField] private C_InputManager Manager;

    public int Vida = 100;
    private Vector2 moveInput;
    private Vector2 panInput;

    private void OnEnable()
    {
        if (Manager != null)
        {
            Manager.OnMoveInput -= SetMove;
            Manager.OnMoveInput += SetMove;

            Manager.OnPanInput -= SetPan;
            Manager.OnPanInput += SetPan;
        }
    }

    private void OnDisable()
    {
        if (Manager != null)
        {
            Manager.OnMoveInput -= SetMove;
            Manager.OnPanInput -= SetPan;
        }
    }

    private void SetMove(Vector2 input) => moveInput = input;
    private void SetPan(Vector2 input) => panInput = input;

    protected override Vector2 GetMoveInput()
    {
        if (camara == null) return moveInput;

        // Movimiento relativo a la cámara
        Vector3 forward = camara.forward; forward.y = 0;
        Vector3 right = camara.right; right.y = 0;

        Vector3 dir = (forward * moveInput.y + right * moveInput.x);
        return new Vector2(dir.x, dir.z);
    }

    protected override Vector2 GetPanInput() => panInput;

    // Rotación sin Rigidbody
    protected override void Rotar(Vector2 input)
    {
        if (input != Vector2.zero)
        {
            // rotación horizontal del personaje
            float yaw = input.x * sensibilidadRotacion * Time.deltaTime;
            transform.rotation *= Quaternion.Euler(0f, yaw, 0f);

            // rotación vertical de la cámara
            if (camara != null)
            {
                pitch -= input.y * sensibilidadRotacion * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, maxPitchAbajo, maxPitchArriba);
                camara.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }
    }

    // Movimiento sin Rigidbody
    protected override void Mover(Vector2 input)
    {
        Vector3 delta = new Vector3(input.x, 0, input.y) * velocidad * Time.deltaTime;
        transform.position += delta;
    }

    public void ReceiveDamage(int damage)
    {
        if(Vida <= 0)
        {
            Vida = 0;
            Debug.Log(
                "El jugador ha muerto."
                , gameObject);
            // Aquí puedes agregar lógica adicional para manejar la muerte del jugador
            return;
        }
        Vida -= damage;
        Debug.Log(
            "Se daño al jugador: " 
            + damage 
            + ". Vida restante: " 
            + Vida, 
            gameObject);
    }
}
