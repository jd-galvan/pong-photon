using Fusion;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    [SerializeField] private float initialSpeed = 5f;

    private Rigidbody2D rb;

    public override void Spawned()
    {
        // Llamado cuando el objeto nace en la red
        rb = GetComponent<Rigidbody2D>();

        // Cuando la pelota aparece, inicia el movimiento
        LaunchBall();
    }

    private void LaunchBall()
    {
        // Velocidad inicial random: a la izquierda o a la derecha
        float xDirection = Random.value < 0.5f ? -1 : 1;
        float yDirection = Random.Range(-0.5f, 0.5f);

        Vector2 initialVelocity = new Vector2(xDirection, yDirection).normalized * initialSpeed;
        rb.velocity = initialVelocity;
    }

    public override void FixedUpdateNetwork()
    {
        // Aquí podrías incluir ajustes de velocidad de la pelota
        // o algún tipo de lógica de rebote extra si quisieras
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ejemplo: si choca con paredes o paletas, puedes cambiar la dirección
        // Fusion sincroniza la física, así que rebotará normalmente si tienes un material físico.
    }
}
