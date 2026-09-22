using UnityEngine;

/// <summary>
/// Cámara semi-fija estilo Little Nightmares: sigue al jugador con un
/// offset y suavizado, y opcionalmente puede "saltar" a un ancla de
/// cámara distinta (ver CameraZone.cs) para cambiar el ángulo en
/// ciertas zonas del nivel.
/// </summary>
public class CameraFollowLN : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Offset por defecto")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);

    [Header("Encuadre")]
    [Tooltip("Si está activo, la cámara SIEMPRE mira al target (puede verse rara cuando el jugador se acerca). Desactívalo para un encuadre fijo tipo Little Nightmares.")]
    [SerializeField] private bool lookAtTarget = false;

    public enum FixedRotationMode
    {
        UseInitialSceneRotation,
        UseEulerValuesBelow
    }

    [Tooltip("UseInitialSceneRotation: usa la rotación que la cámara tenga en la escena al arrancar (la que acomodas a mano). UseEulerValuesBelow: usa los valores de 'Fixed Rotation Euler', editables en vivo.")]
    [SerializeField] private FixedRotationMode rotationMode = FixedRotationMode.UseInitialSceneRotation;

    [Tooltip("Solo se usa si Rotation Mode = UseEulerValuesBelow. Editable en vivo, incluso en Play mode.")]
    [SerializeField] private Vector3 fixedRotationEuler;

    [Header("Suavizado")]
    [SerializeField] private float positionSmoothTime = 0.25f;
    [SerializeField] private float rotationSmoothSpeed = 4f;

    private Vector3 currentVelocity;
    private Quaternion initialRotation;

    // Si una CameraZone quiere forzar otro punto/ángulo de cámara
    private Transform overrideAnchor;

    private void Awake()
    {
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition;

        if (overrideAnchor != null)
        {
            desiredPosition = overrideAnchor.position;
        }
        else
        {
            desiredPosition = target.position + offset;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            positionSmoothTime
        );

        if (lookAtTarget)
        {
            Vector3 lookPoint = target.position + Vector3.up * 1.5f;
            Quaternion targetRotation = Quaternion.LookRotation(lookPoint - transform.position);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothSpeed * Time.deltaTime
            );
        }
        else
        {
            Quaternion targetRotation;

            if (overrideAnchor != null)
            {
                targetRotation = overrideAnchor.rotation;
            }
            else if (rotationMode == FixedRotationMode.UseEulerValuesBelow)
            {
                // Se lee en vivo cada frame: puedes ajustarlo en el inspector
                // en Play Mode y verás el cambio de inmediato.
                targetRotation = Quaternion.Euler(fixedRotationEuler);
            }
            else
            {
                targetRotation = initialRotation;
            }

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothSpeed * Time.deltaTime
            );
        }
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