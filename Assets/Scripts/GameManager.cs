using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private BallController ball;
    [SerializeField] private Text player1ScoreText;
    [SerializeField] private Text player2ScoreText;
    [SerializeField] private GameObject matchWon;
    [SerializeField] private Text matchWonText;
    [SerializeField] private GameObject Line;
    [SerializeField] private GameObject InitWindow;

    private int scoreToWin = 2; 
    private bool jugadoresListos = false;
    private bool gameOver = false;

    // Variables de puntaje sincronizadas en red
    [Networked] private int player1Score { get; set; }
    [Networked] private int player2Score { get; set; }


    [Networked] private NetworkBool isInitVisible { get; set; } = true; // Empieza visible
    [Networked] private NetworkBool isFieldVisible { get; set; } = false; // Empieza no visible
    [Networked] private NetworkBool isWonVisible { get; set; } = false; // Empieza no visible

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

        // Si es el host y se presiona la tecla R, reinicia el juego
        if (gameOver && Runner.IsServer && Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    public override void Render()
    {
        InitWindow.SetActive(isInitVisible); // Sincroniza la visibilidad en clientes
        Line.SetActive(isFieldVisible);
        UpdateScoreUI(); // Asegura que la UI se actualiza en todos los clientes
    }

    public void NewGame()
    {
        Debug.Log("¡El juego ha comenzado!");
        if (Runner.IsServer)
        {
            
            isInitVisible = false; // El host oculta "init", y se replica en todos los clientes
            isFieldVisible = true;
            //RpcOcultarVentanaInicio();
        }

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

        if (player1Score == 0 && player2Score == 0) {
            matchWonText.text = "";
        }
        if (player1Score == scoreToWin)
        {
            matchWonText.text = "Player 1 won";
            ball.RpcStopBall();
            gameOver = true;
        }
        else if (player2Score == scoreToWin) {
            matchWonText.text = "Player 2 won";
            ball.RpcStopBall();
            gameOver = true;
        }

    }
}




