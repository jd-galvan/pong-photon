// using UnityEngine;
// using UnityEngine.UI;

// [DefaultExecutionOrder(-1)]
// public class GameManager : MonoBehaviour
// {
//     // [SerializeField] private Ball ball;
//     [SerializeField] private Paddle playerPaddle;
//     [SerializeField] private Paddle computerPaddle;
//     [SerializeField] private Text playerScoreText;
//     [SerializeField] private Text computerScoreText;

//     private int playerScore;
//     private int computerScore;

//     private void Start()
//     {
//         NewGame();
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.R)) {
//             NewGame();
//         }
//     }

//     public void NewGame()
//     {
//         SetPlayerScore(0);
//         SetComputerScore(0);
//         NewRound();
//     }

//     public void NewRound()
//     {
//         playerPaddle.ResetPosition();
//         computerPaddle.ResetPosition();
//         ball.ResetPosition();

//         CancelInvoke();
//         Invoke(nameof(StartRound), 1f);
//     }

//     private void StartRound()
//     {
//         ball.AddStartingForce();
//     }

//     public void OnPlayerScored()
//     {
//         SetPlayerScore(playerScore + 1);
//         NewRound();
//     }

//     public void OnComputerScored()
//     {
//         SetComputerScore(computerScore + 1);
//         NewRound();
//     }

//     private void SetPlayerScore(int score)
//     {
//         playerScore = score;
//         playerScoreText.text = score.ToString();
//     }

//     private void SetComputerScore(int score)
//     {
//         computerScore = score;
//         computerScoreText.text = score.ToString();
//     }

// }
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private BallController ball;
    [SerializeField] private Text player1ScoreText;
    [SerializeField] private Text player2ScoreText;

    private bool jugadoresListos = false;
    private bool gameStarted = false;

    // Variables de puntaje sincronizadas en red
    [Networked] private int player1Score { get; set; }
    [Networked] private int player2Score { get; set; }

    private void Update()
    {
        if (Runner == null) return;

        if (Runner.IsServer && !jugadoresListos)
        {
            if (Runner.SessionInfo.PlayerCount == 2)  // ActivePlayers.Count() no existe en Fusion 2
            {
                jugadoresListos = true;
                NewGame();
            }
        }

        if (gameStarted && Runner.IsServer && Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    public override void Render()
    {
        UpdateScoreUI(); // Asegura que la UI se actualiza en todos los clientes
    }

    public void NewGame()
    {
        Debug.Log("¡El juego ha comenzado!");
        gameStarted = true;

        if (Runner.IsServer)
        {
            player1Score = 0;
            player2Score = 0;
        }

        NewRound();
    }

    public void NewRound()
    {
        CancelInvoke();
        StartRound();
    }

    private void StartRound()
    {
        ball.RestartPosition();
    }

    public void OnPlayer1Scored()
    {
        if (Runner.IsServer)
        {
            player1Score++;
        }
        NewRound();
    }

    public void OnPlayer2Scored()
    {
        if (Runner.IsServer)
        {
            player2Score++;
        }
        NewRound();
    }

    private void UpdateScoreUI()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = player1Score.ToString();

        if (player2ScoreText != null)
            player2ScoreText.text = player2Score.ToString();
    }
}




