/// <summary>
/// Zona de puntuación en el juego de red utilizando Fusion.
/// </summary>
using Fusion;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ScoringZone : NetworkBehaviour
{
    /// <summary>
    /// Evento que se activa cuando se detecta una colisión con la pelota.
    /// </summary>
    public UnityEvent scoreTrigger;

    /// <summary>
    /// Referencia al controlador de la pelota.
    /// </summary>
    [SerializeField] private BallController ball;

    /// <summary>
    /// Detecta la colisión con la pelota y activa el evento de puntuación.
    /// </summary>
    /// <param name="collision">Información sobre la colisión.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BallController _))
        {
            scoreTrigger.Invoke();
            ball.RestartPosition();
        }
    }
}