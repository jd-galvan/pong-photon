/// <summary>
/// Administrador del juego para gestionar la lógica principal del juego de pelota.
/// </summary>
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    /// <summary>
    /// Referencia al controlador de la pelota.
    /// </summary>
    [SerializeField] private BallController ball;

    /// <summary>
    /// Referencias a los textos de puntuación de los jugadores.
    /// </summary>
    [SerializeField] private Text player1ScoreText;
    [SerializeField] private Text player2ScoreText;

    /// <summary>
    /// Referencia a la pantalla de victoria y su texto.
    /// </summary>
    [SerializeField] private GameObject matchWon;
    [SerializeField] private Text matchWonText;

    /// <summary>
    /// Referencias a elementos de la interfaz.
    /// </summary>
    [SerializeField] private GameObject Line;
    [SerializeField] private GameObject InitWindow;

    private int scoreToWin = 5;
    private bool jugadoresListos = false;
    private bool gameOver = false;

    /// <summary>
    /// Puntuaciones de los jugadores sincronizadas en la red.
    /// </summary>
    [Networked] private int player1Score { get; set; }
    [Networked] private int player2Score { get; set; }

    [Networked] private NetworkBool isInitVisible { get; set; } = true; // Comienza visible
    [Networked] private NetworkBool isFieldVisible { get; set; } = false; // Comienza no visible
    [Networked] private NetworkBool isWonVisible { get; set; } = false; // Comienza no visible

    /// <summary>
    /// Verifica el estado del juego y reinicia si es necesario.
    /// </summary>
    private void Update()
    {
        if (Runner == null) return;

        if (Runner.IsServer && !jugadoresListos)
        {
            if (Runner.SessionInfo.PlayerCount == 2)
            {
                jugadoresListos = true;
                NewGame();
            }
        }

        // Reiniciar el juego si el host presiona R
        if (gameOver && Runner.IsServer && Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    /// <summary>
    /// Renderiza la interfaz de usuario y sincroniza la visibilidad de los elementos.
    /// </summary>
    public override void Render()
    {
        InitWindow.SetActive(isInitVisible);
        Line.SetActive(isFieldVisible);
        UpdateScoreUI();
    }

    /// <summary>
    /// Inicia un nuevo juego reseteando puntuaciones y la interfaz.
    /// </summary>
    public void NewGame()
    {
        Debug.Log("¡El juego ha comenzado!");
        if (Runner.IsServer)
        {
            isInitVisible = false;
            isFieldVisible = true;
            player1Score = 0;
            player2Score = 0;
        }
        NewRound();
    }

    /// <summary>
    /// Inicia una nueva ronda reseteando la posición de la pelota.
    /// </summary>
    public void NewRound()
    {
        CancelInvoke();
        StartRound();
    }

    /// <summary>
    /// Reinicia la posición de la pelota.
    /// </summary>
    private void StartRound()
    {
        ball.RestartPosition();
    }

    /// <summary>
    /// Se ejecuta cuando el jugador 1 anota un punto.
    /// </summary>
    public void OnPlayer1Scored()
    {
        if (Runner.IsServer)
        {
            player1Score++;
        }
        NewRound();
    }

    /// <summary>
    /// Se ejecuta cuando el jugador 2 anota un punto.
    /// </summary>
    public void OnPlayer2Scored()
    {
        if (Runner.IsServer)
        {
            player2Score++;
        }
        NewRound();
    }

    /// <summary>
    /// Actualiza la interfaz con las puntuaciones actuales.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = player1Score.ToString();

        if (player2ScoreText != null)
            player2ScoreText.text = player2Score.ToString();

        if (player1Score == 0 && player2Score == 0)
        {
            matchWonText.text = "";
        }

        if (player1Score == scoreToWin)
        {
            matchWonText.text = "Player 1 won\nPress R to restart (Host)";
            ball.RpcStopBall();
            gameOver = true;
        }
        else if (player2Score == scoreToWin)
        {
            matchWonText.text = "Player 2 won\nPress R to restart (Host)";
            ball.RpcStopBall();
            gameOver = true;
        }
    }
}