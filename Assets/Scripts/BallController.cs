/// <summary>
/// Controlador de la pelota en el juego de red utilizando Fusion.
/// </summary>
using Fusion;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    /// <summary>
    /// Velocidad inicial de la pelota.
    /// </summary>
    [SerializeField] private float initialSpeed = 5f;

    /// <summary>
    /// Velocidad actual de la pelota.
    /// </summary>
    public float currentSpeed { get; set; }

    private Rigidbody2D rb;

    /// <summary>
    /// Reinicia la posición de la pelota en todos los clientes.
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcRestartBall()
    {
        gameObject.SetActive(true);
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        LaunchBall();
    }

    /// <summary>
    /// Método llamado cuando el objeto es generado en la red.
    /// </summary>
    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Reinicia la posición de la pelota en el servidor.
    /// </summary>
    public void RestartPosition()
    {
        if (Runner.IsServer) // Solo el servidor debe cambiar la posición
        {
            RpcRestartBall();
        }
    }

    /// <summary>
    /// Lanza la pelota en una dirección aleatoria.
    /// </summary>
    public void LaunchBall()
    {
        float xDirection = Random.value < 0.5f ? -1 : 1;
        float yDirection = Random.Range(-0.5f, 0.5f);

        Vector2 initialVelocity = new Vector2(xDirection, yDirection).normalized * initialSpeed;
        rb.velocity = initialVelocity;
    }

    /// <summary>
    /// Detiene la pelota en todos los clientes.
    /// </summary>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStopBall()
    {
        rb.velocity = Vector2.zero;
    }
}