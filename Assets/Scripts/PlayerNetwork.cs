using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float speed = 5f; // Velocidad de movimiento
    private Rigidbody2D rb;

    public float upperLimit = 4f; // Límite superior de la cancha
    public float lowerLimit = -4f; // Límite inferior de la cancha

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
    }

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
