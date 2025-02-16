/// <summary>
/// Superficie que aplica un efecto de rebote a la pelota al colisionar.
/// </summary>
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BouncySurface : MonoBehaviour
{
    /// <summary>
    /// Tipos de fuerza aplicables al rebote.
    /// </summary>
    public enum ForceType
    {
        Additive,
        Multiplicative,
    }

    /// <summary>
    /// Tipo de fuerza utilizada para modificar la velocidad de la pelota.
    /// </summary>
    public ForceType forceType = ForceType.Additive;

    /// <summary>
    /// Intensidad del rebote aplicada a la pelota.
    /// </summary>
    public float bounceStrength = 0f;

    /// <summary>
    /// Detecta la colisión con la pelota y ajusta su velocidad en función del tipo de fuerza seleccionado.
    /// </summary>
    /// <param name="collision">Información sobre la colisión.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BallController ball))
        {
            switch (forceType)
            {
                case ForceType.Additive:
                    ball.currentSpeed += bounceStrength;
                    return;
                case ForceType.Multiplicative:
                    ball.currentSpeed *= bounceStrength;
                    return;
            }
        }
    }
}