using Fusion;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ScoringZone : NetworkBehaviour
{
    public UnityEvent scoreTrigger;
    [SerializeField] private BallController ball;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BallController _)) {
            scoreTrigger.Invoke();
            ball.RestartPosition();
        }
    }
}
