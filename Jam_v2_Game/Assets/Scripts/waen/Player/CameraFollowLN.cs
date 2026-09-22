using UnityEngine;

/// <summary>
/// Cámara semi-fija estilo Little Nightmares:
/// - Se queda quieta mientras el jugador se mueve dentro de una "zona
///   muerta" (deadzone) cómoda del encuadre.
/// - Solo se desplaza cuando el jugador se acerca al borde de esa zona,
///   y lo hace con seguimiento suave (no 1 a 1).
/// - La rotación se mantiene prácticamente fija (nada de look-at dinámico).
/// - Tiene un ligero balanceo idle (sway) para que no se sienta estática.
/// </summary>
public class CameraFollowLN : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Offset por defecto (relativo al ancla)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);

    [Header("Deadzone (zona donde el jugador se mueve libre)")]
    [Tooltip("Ancho total de la zona muerta en el eje horizontal de cámara.")]
    [SerializeField] private float horizontalDeadzone = 3f;
    [Tooltip("Alto total de la zona muerta en el eje vertical de cámara.")]
    [SerializeField] private float verticalDeadzone = 2f;
    [Tooltip("Profundidad total de la zona muerta (eje hacia/desde cámara).")]
    [SerializeField] private float depthDeadzone = 3f;

    [Header("Encuadre")]
    [Tooltip("Si está activo, la cámara SIEMPRE mira al target. Para Little Nightmares déjalo desactivado.")]
    [SerializeField] private bool lookAtTarget = false;

    public enum FixedRotationMode
    {
        UseInitialSceneRotation,
        UseEulerValuesBelow
    }

    [SerializeField] private FixedRotationMode rotationMode = FixedRotationMode.UseInitialSceneRotation;
    [SerializeField] private Vector3 fixedRotationEuler;

    [Header("Suavizado")]
    [Tooltip("Qué tan suave se mueve el ancla de cámara al salir de la deadzone. Más alto = más lento/cinematográfico.")]
    [SerializeField] private float anchorSmoothTime = 0.6f;
    [SerializeField] private float rotationSmoothSpeed = 4f;

    [Header("Balanceo idle (sway)")]
    [SerializeField] private bool enableIdleSway = true;
    [SerializeField] private float swayAmplitude = 0.03f;
    [SerializeField] private float swaySpeed = 0.4f;

    private Vector3 anchorPosition;
    private Vector3 anchorVelocity;
    private Quaternion initialRotation;
    private float swaySeed;

    // Si una CameraZone quiere forzar otro punto/ángulo de cámara
    private Transform overrideAnchor;

    private void Awake()
    {
        initialRotation = transform.rotation;
        swaySeed = Random.Range(0f, 100f);

        if (target != null)
        {
            anchorPosition = target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Quaternion baseRotation = GetBaseRotation();

        if (overrideAnchor == null)
        {
            UpdateAnchorWithDeadzone(baseRotation);
        }
        else
        {
            anchorPosition = overrideAnchor.position - offset;
        }

        Vector3 desiredPosition = anchorPosition + offset;

        if (enableIdleSway)
        {
            desiredPosition += GetIdleSway(baseRotation);
        }

        // Posición: seguimiento directo (el suavizado real ya ocurre
        // al mover el ancla, no aquí, para evitar doble "lag").
        transform.position = desiredPosition;

        Quaternion targetRotation = lookAtTarget
            ? Quaternion.LookRotation((target.position + Vector3.up * 1.5f) - transform.position)
            : (overrideAnchor != null ? overrideAnchor.rotation : baseRotation);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }

    private Quaternion GetBaseRotation()
    {
        return rotationMode == FixedRotationMode.UseEulerValuesBelow
            ? Quaternion.Euler(fixedRotationEuler)
            : initialRotation;
    }

    private void UpdateAnchorWithDeadzone(Quaternion baseRotation)
    {
        Vector3 right = baseRotation * Vector3.right;
        Vector3 up = baseRotation * Vector3.up;
        Vector3 fwd = baseRotation * Vector3.forward;

        Vector3 delta = target.position - anchorPosition;

        float h = Vector3.Dot(delta, right);
        float v = Vector3.Dot(delta, up);
        float d = Vector3.Dot(delta, fwd);

        float excessH = Mathf.Max(0f, Mathf.Abs(h) - horizontalDeadzone * 0.5f) * Mathf.Sign(h);
        float excessV = Mathf.Max(0f, Mathf.Abs(v) - verticalDeadzone * 0.5f) * Mathf.Sign(v);
        float excessD = Mathf.Max(0f, Mathf.Abs(d) - depthDeadzone * 0.5f) * Mathf.Sign(d);

        Vector3 desiredAnchor = anchorPosition
            + right * excessH
            + up * excessV
            + fwd * excessD;

        anchorPosition = Vector3.SmoothDamp(
            anchorPosition,
            desiredAnchor,
            ref anchorVelocity,
            anchorSmoothTime
        );
    }

    private Vector3 GetIdleSway(Quaternion baseRotation)
    {
        float t = Time.time * swaySpeed + swaySeed;

        float swayX = (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f;
        float swayY = (Mathf.PerlinNoise(0f, t) - 0.5f) * 2f;

        Vector3 right = baseRotation * Vector3.right;
        Vector3 up = baseRotation * Vector3.up;

        return (right * swayX + up * swayY) * swayAmplitude;
    }

    /// <summary>Llamado por una CameraZone al entrar el jugador.</summary>
    public void SetOverrideAnchor(Transform anchor)
    {
        overrideAnchor = anchor;
    }

    /// <summary>Llamado por una CameraZone al salir el jugador.</summary>
    public void ClearOverrideAnchor()
    {
        overrideAnchor = null;
    }
}