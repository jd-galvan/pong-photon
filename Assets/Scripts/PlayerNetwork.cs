/// <summary>
/// Controlador de red para el jugador en un juego multijugador utilizando Fusion.
/// </summary>
using Fusion;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    /// <summary>
    /// Velocidad de movimiento del jugador.
    /// </summary>
    public float speed = 5f;

    private Rigidbody2D rb;

    /// <summary>
    /// Límite superior de movimiento en la cancha.
    /// </summary>
    public float upperLimit = 4f;

    /// <summary>
    /// Límite inferior de movimiento en la cancha.
    /// </summary>
    public float lowerLimit = -4f;

    /// <summary>
    /// Método llamado cuando el objeto es generado en la red.
    /// </summary>
    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Método que maneja la lógica de movimiento del jugador en cada actualización de la red.
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (rb == null)
        {
            return;
        }

        if (GetInput(out NetworkInputData inputData))
        {
            // Movimiento en el eje Y
            Vector2 movement = new Vector2(0, inputData.moveDirection.y * speed);
            rb.velocity = movement;

            // Obtener la nueva posición del Rigidbody
            Vector2 newPosition = rb.position;

            // Limitar la posición dentro del rango permitido
            newPosition.y = Mathf.Clamp(newPosition.y, lowerLimit, upperLimit);

            // Aplicar la posición corregida
            rb.position = newPosition;
        }
    }
}