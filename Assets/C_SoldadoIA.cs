using UnityEngine;

public class C_SoldadoIA : C_SoldadoMoverse
{
    [Header("IA")]
    public Transform target;
    public float radioLlegada = 2f;
    public float desaceleracion = 20f; // distancia donde empieza a frenar
    public float anticipacion = 1f;
    public float suavizadoRotacion = 2f;
    [Range(0f, 50f)] public float anguloCurvatura = 20f;
    [Range(0f, 5f)] public float longitudCurva = 1f;

    [Header("Altura fija")]
    public float altura = 0f;

    [Header("Debug")]
    public bool mostrarDebug = true;

    public string estado = "Sin objetivo";

    private Vector3 ultimaPosicionTarget;

    protected override Vector2 GetMoveInput()
    {
        if (target == null)
        {
            estado = "Sin objetivo";
            return Vector2.zero;
        }

        // ------------------------------
        // 1️⃣ Predicción del target
        // ------------------------------
        Vector3 velocidadTarget = (target.position - ultimaPosicionTarget) / Time.deltaTime;
        ultimaPosicionTarget = target.position;
        Vector3 prediccion = target.position + velocidadTarget * anticipacion;
        prediccion.y = altura;

        Vector3 dir = prediccion - transform.position;
        float distancia = dir.magnitude;

        // ------------------------------
        // 2️⃣ Estado según distancia
        // ------------------------------
        if (distancia < radioLlegada) estado = "Llegado";
        else if (distancia < desaceleracion) estado = "Objetivo cerca";
        else estado = "Objetivo lejos";

        // ------------------------------
        // 3️⃣ Arrive con lerp de velocidad
        // ------------------------------
        float factorVel;
        if (distancia < radioLlegada)
        {
            factorVel = 0f;
        }
        else
        {
            // velocidad máxima al principio y reduce progresivamente al acercarse
            float t = Mathf.Clamp01((distancia - radioLlegada) / (desaceleracion - radioLlegada));
            factorVel = Mathf.Lerp(0.1f, 1f, t); // comienza lento, aumenta y luego frena
        }

        // ------------------------------
        // 4️⃣ Curvatura aleatoria
        // ------------------------------
        Vector3 direccionDeseada = dir.normalized;
        if (anguloCurvatura > 0f)
        {
            float perlin = Mathf.PerlinNoise(Time.time, 0) - 0.5f;
            float desviacion = perlin * anguloCurvatura * longitudCurva;
            Quaternion curva = Quaternion.AngleAxis(desviacion, Vector3.up);
            direccionDeseada = curva * direccionDeseada;
        }

        // ------------------------------
        // 5️⃣ Rotación suave hacia target
        // ------------------------------
        Quaternion rotFinal = Quaternion.LookRotation(dir.normalized);
        transform.forward = Vector3.Slerp(transform.forward, rotFinal * Vector3.forward, suavizadoRotacion * Time.deltaTime);

        // ------------------------------
        // 6️⃣ Movimiento
        // ------------------------------
        transform.position += transform.forward * velocidad * factorVel * Time.deltaTime;

        // mantener altura
        Vector3 pos = transform.position;
        pos.y = altura;
        transform.position = pos;

        // ------------------------------
        // 7️⃣ Debug line
        // ------------------------------
        if (mostrarDebug)
        {
            Color colorLinea = Color.green; // lejos
            if (estado == "Objetivo cerca") colorLinea = Color.yellow;
            else if (estado == "Llegado") colorLinea = Color.red;

            Debug.DrawLine(transform.position, transform.position + transform.forward * 2f, colorLinea);
            Debug.DrawLine(transform.position, prediccion, Color.cyan);
        }

        return new Vector2(transform.forward.x * factorVel, transform.forward.z * factorVel);
    }

    protected override Vector2 GetPanInput() => Vector2.zero;

    private void Update()
    {
        Vector2 move = GetMoveInput();
        Vector2 pan = GetPanInput();

        Mover(move);
        Rotar(pan);
    }
}
