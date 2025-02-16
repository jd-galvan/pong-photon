/// <summary>
/// Administrador de generación de jugadores en la red utilizando Fusion.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    /// <summary>
    /// Instancia del NetworkRunner para manejar la conexión de red.
    /// </summary>
    private NetworkRunner _runner;

    /// <summary>
    /// Prefab de red para los jugadores.
    /// </summary>
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    /// <summary>
    /// Diccionario que almacena los personajes generados asociados a cada jugador.
    /// </summary>
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    /// <summary>
    /// Inicia un nuevo juego en el modo seleccionado.
    /// </summary>
    /// <param name="mode">Modo de juego (Host o Cliente).</param>
    async void StartGame(GameMode mode)
    {
        if (_runner != null)
        {
            Debug.LogWarning("El juego ya está en ejecución.");
            return;
        }

        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log($"Juego iniciado como: {mode}");
    }

    /// <summary>
    /// Muestra botones en la interfaz para iniciar el juego como Host o Cliente.
    /// </summary>
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                StartGame(GameMode.Host);
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                StartGame(GameMode.Client);
        }
    }

    /// <summary>
    /// Evento llamado cuando un jugador se une al juego.
    /// </summary>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            float paddleXPosition = _spawnedCharacters.Count == 0 ? -8 : 8;
            Vector2 spawnPosition = new Vector2(paddleXPosition, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    /// <summary>
    /// Evento llamado cuando un jugador abandona la partida.
    /// </summary>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    /// <summary>
    /// Captura la entrada del usuario y la envía a la red.
    /// </summary>
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Vector2 moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
            moveInput.y = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            moveInput.y = -1;

        NetworkInputData inputData = new NetworkInputData();
        inputData.moveDirection = moveInput;

        input.Set(inputData);

        Debug.Log($"[OnInput] Jugador {runner.LocalPlayer} envió movimiento: {inputData.moveDirection}");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}