using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class C_SoldadoJugador : C_SoldadoTransform, I_ReceivesDamage
{
    [Header("Jugador")]
    [SerializeField] public Transform camara;
    [SerializeField] public C_InputManager Manager;

    public int Vida = 100;
    public Vector2 moveInput;
    public Vector2 panInput;
    public GameObject Geometria;


    public TextMeshProUGUI TextoDeSoldados;

    public void OnEnable()
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


    public void Update()
    {
        if (TextoDeSoldados != null)
        {
            TextoDeSoldados.text = AliadosActivos.Count + "/" + CantidadMaximaDeAliados.ToString();
        }
    }

    private void SetMove(Vector2 input)
    {
        //Debug.Log("se recibio movimiento" + input);
        moveInput = input;
        Mover(input);

    }
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
    public AM2_P2_Aliado IndicadorDeDisparando;
    protected override void Mover(Vector2 input)
    {
        //Debug.Log("se movio hacia " + input);
        Vector3 delta = new Vector3(input.x, 0, input.y) * velocidad * Time.deltaTime;
        transform.position += delta;
        if (IndicadorDeDisparando != null
            && IndicadorDeDisparando.enemigoActual == null)
        {
            Vector3 DireccionDeMira = delta + transform.position;
            Debug.DrawLine(DireccionDeMira, transform.position, Color.red);
            //Debug.Log("Se esta miradno al objetivo");
            Geometria.transform.LookAt(DireccionDeMira);
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (Vida <= 0)
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



    public Transform destinoDelArma; // Arrastra aquí el objeto "Mano" o "WeaponHolder"
    public float duracion = 1.0f;    // Tiempo que tarda en llegar

    // Llama a esta función cuando hagas click o presiones E
    public void Recojer(Transform arma)
    {
        GameObject armaGO = arma.gameObject;
        // Si tiene rigbody. Lo desactivamos para evitar problemas
        Rigidbody rb = armaGO.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Si tiene collider lo desactivamos para evitar problemas
        Collider col = armaGO.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        StartCoroutine(AnimacionRecojer(arma));
    }

    IEnumerator AnimacionRecojer(Transform arma)
    {
        // 1. Hacemos que el arma sea hija del destino INMEDIATAMENTE
        // El segundo parámetro 'true' mantiene su posición mundial por un instante
        arma.SetParent(destinoDelArma, true);

        // 2. Guardamos la posición y rotación iniciales (locales respecto a la mano)
        Vector3 posInicial = arma.localPosition;
        Quaternion rotInicial = arma.localRotation;

        // 3. Definimos el destino (0,0,0 local y rotación 0)
        Vector3 posFinal = Vector3.zero;
        Quaternion rotFinal = Quaternion.identity;

        float tiempoPasado = 0;

        while (tiempoPasado < duracion)
        {
            tiempoPasado += Time.deltaTime;

            // Calculamos el porcentaje completado (0 a 1)
            float t = tiempoPasado / duracion;

            // OPCIONAL: "SmoothStep" hace que empiece lento y termine lento (más natural)
            t = t * t * (3f - 2f * t);

            // Movemos y rotamos suavemente
            arma.localPosition = Vector3.Lerp(posInicial, posFinal, t);
            arma.localRotation = Quaternion.Lerp(rotInicial, rotFinal, t);

            // Esperamos al siguiente frame
            yield return null;
        }

        // 4. Nos aseguramos de que quede EXACTO al final (por si hubo decimales)
        arma.localPosition = posFinal;
        arma.localRotation = rotFinal;
    }


    public int CantidadMaximaDeAliados;
    public List<GameObject> AliadosActivos = new List<GameObject>();
    public GameObject PrefabDeAliado;

    public void ColocarAliado() // Llamado via boton
    {
        // 1) Validaciones básicas
        if (PrefabDeAliado == null)
        {
            Debug.LogWarning("PrefabDeAliado es null. Asignalo en el inspector.");
            return;
        }
        LimpiarAliadosNulos();

        if (AliadosActivos == null)
            AliadosActivos = new List<GameObject>();

        // 2) Límite máximo
        if (AliadosActivos.Count >= CantidadMaximaDeAliados)
        {
            Debug.Log("No se puede colocar: límite de aliados alcanzado.");
            return;
        }

        // 3) Instanciar y configurar
        Vector3 spawnPos = transform.position; // cambia esto si querés otra posición
        GameObject nuevo = Instantiate(PrefabDeAliado, spawnPos, Quaternion.identity);
        nuevo.name = PrefabDeAliado.name + "_" + AliadosActivos.Count;

        // 4) Añadir a la lista
        AliadosActivos.Add(nuevo);
    } 


    // 🧹 Limpia todos los elementos null/missing
    private void LimpiarAliadosNulos()
    {
        for (int i = AliadosActivos.Count - 1; i >= 0; i--)
        {
            if (AliadosActivos[i] == null)
                AliadosActivos.RemoveAt(i);
        }
    }


    public void OnDestroy()
    {
        GameManager.Instance.CambiarDeEscena("EscenaDerrota");
    }
}
