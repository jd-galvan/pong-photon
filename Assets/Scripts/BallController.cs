using Fusion;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    [SerializeField] private float initialSpeed = 5f;
    public float currentSpeed { get; set; }

    private Rigidbody2D rb;


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcRestartBall()
    {
        gameObject.SetActive(true);
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        LaunchBall();
    }

    public override void Spawned()
    {
        // Llamado cuando el objeto nace en la red
        rb = GetComponent<Rigidbody2D>();
    }


    // Función para reiniciar la posición de la pelota
    public void RestartPosition()
    {
        
        if (Runner.IsServer) // Solo el servidor debe cambiar la posición
        {
            //gameObject.SetActive(true);
            //rb.velocity = Vector2.zero;
            //transform.position = Vector2.zero;
            //LaunchBall();
            RpcRestartBall();
        }
    
    }

    public void LaunchBall()
    {
        // Velocidad inicial random: a la izquierda o a la derecha
        float xDirection = Random.value < 0.5f ? -1 : 1;
        float yDirection = Random.Range(-0.5f, 0.5f);

        Vector2 initialVelocity = new Vector2(xDirection, yDirection).normalized * initialSpeed;
        rb.velocity = initialVelocity;
    }

    // RPC para detener la pelota en todos los clientes
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStopBall()
    {
        rb.velocity = Vector2.zero;
    }
}
