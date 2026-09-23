using UnityEngine;

public class ControladorPersonaje2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 8f;
    private float movimientoHorizontal;

    [Header("Salto")]
    public float fuerzaSalto = 12f;
    public Transform detectorSuelo;
    public float distanciaRaycast = 0.2f;
    public LayerMask capaSuelo;

    private Rigidbody2D rb;
    private bool tocandoSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Obtener entrada de movimiento (Flechas / AD / Joystick)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        // Detectar si está tocando el suelo usando un Raycast hacia abajo
        tocandoSuelo = Physics2D.Raycast(detectorSuelo.position, Vector2.down, distanciaRaycast, capaSuelo);

        // Dibujar el Raycast en la ventana de Escena para poder calibrarlo visualmente
        Debug.DrawRay(detectorSuelo.position, Vector2.down * distanciaRaycast, tocandoSuelo ? Color.green : Color.red);

        // Saltar si se presiona la barra espaciadora y está en el suelo
        if (Input.GetButtonDown("Jump") && tocandoSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
    }

    void FixedUpdate()
    {
        // Aplicar el movimiento horizontal en el FixedUpdate (mejor para físicas)
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidad, rb.linearVelocity.y);
    }
}
