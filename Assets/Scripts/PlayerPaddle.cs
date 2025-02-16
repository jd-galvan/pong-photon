/// <summary>
/// Controlador de la paleta del jugador.
/// </summary>
using UnityEngine;

public class PlayerPaddle : Paddle
{
    private Vector2 direction;

    /// <summary>
    /// Captura la entrada del jugador y determina la dirección de movimiento.
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            direction = Vector2.down;
        }
        else
        {
            direction = Vector2.zero;
        }
    }

    /// <summary>
    /// Aplica fuerza a la paleta en la dirección especificada.
    /// </summary>
    private void FixedUpdate()
    {
        if (direction.sqrMagnitude != 0)
        {
            rb.AddForce(direction * speed);
        }
    }
}