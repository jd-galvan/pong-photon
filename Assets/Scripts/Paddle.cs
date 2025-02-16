/// <summary>
/// Clase base abstracta para las paletas en el juego.
/// Proporciona funcionalidades básicas como movimiento y rebote dinámico de la pelota.
/// </summary>
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Paddle : MonoBehaviour
{
    /// <summary>
    /// Referencia al componente Rigidbody2D de la paleta.
    /// </summary>
    protected Rigidbody2D rb;

    /// <summary>
    /// Velocidad de movimiento de la paleta.
    /// </summary>
    public float speed = 8f;

    /// <summary>
    /// Determina si se debe utilizar un rebote dinámico basado en la posición de impacto.
    /// </summary>
    [Tooltip("Changes how the ball bounces off the paddle depending on where it hits the paddle. The further from the center of the paddle, the steeper the bounce angle.")]
    public bool useDynamicBounce = false;

    /// <summary>
    /// Inicializa la referencia al Rigidbody2D.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Reinicia la posición de la paleta al centro en el eje Y y detiene su movimiento.
    /// </summary>
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        rb.position = new Vector2(rb.position.x, 0f);
    }

    /// <summary>
    /// Maneja la colisión con la pelota y ajusta su dirección si el rebote dinámico está activado.
    /// </summary>
    /// <param name="collision">Datos de la colisión.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (useDynamicBounce && collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ball = collision.rigidbody;
            Collider2D paddle = collision.otherCollider;

            // Obtener información sobre la colisión
            Vector2 ballDirection = ball.velocity.normalized;
            Vector2 contactDistance = ball.transform.position - paddle.bounds.center;
            Vector2 surfaceNormal = collision.GetContact(0).normal;
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, surfaceNormal);

            // Rotar la dirección de la pelota en función de la distancia de contacto
            float maxBounceAngle = 75f;
            float bounceAngle = contactDistance.y / paddle.bounds.size.y * maxBounceAngle;
            ballDirection = Quaternion.AngleAxis(bounceAngle, rotationAxis) * ballDirection;

            // Aplicar la nueva dirección a la pelota
            ball.velocity = ballDirection * ball.velocity.magnitude;
        }
    }
}