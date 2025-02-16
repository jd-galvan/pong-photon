using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float speed = 5f; // Velocidad de movimiento
    private Rigidbody2D rb;
    private Transform paddleTransform; // Referencia a la paleta

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
          //Debug.Log("Ingreso a este metodo");
          // Movimiento en el eje Y (Solo hacia arriba y abajo)
          Vector2 movement = new Vector2(0, inputData.moveDirection.y * speed);
          rb.velocity = movement;
        }
    }
}
