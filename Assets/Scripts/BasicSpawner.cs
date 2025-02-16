using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner; //Atributo interno privado donde descansa el NetworkRunner
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    async void StartGame(GameMode mode) {
        if (_runner != null) {
            Debug.LogWarning("El juego ya está en ejecución.");
            return;
        }

        // Crear el NetworkRunner y establecer que proveerá input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Obtener la escena actual
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);

        // Iniciar el juego o unirse a una sesión
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log($"Juego iniciado como: {mode}");
    }

    private void OnGUI()
    {
        if (_runner == null)
        {
            float buttonWidth = 200;
            float buttonHeight = 40;
            float spacing = 10; // Espacio entre botones
            float offsetY = 100; // Desplazamiento hacia abajo en píxeles

            // Posición centralizada
            float centerX = (Screen.width - buttonWidth) / 2;
            float centerY = (Screen.height - (buttonHeight * 2 + spacing)) / 2 + offsetY;

            // Botón Host
            if (GUI.Button(new Rect(centerX, centerY, buttonWidth, buttonHeight), "Host"))
            {
                StartGame(GameMode.Host);
            }

            // Botón Join (Debajo del botón Host)
            if (GUI.Button(new Rect(centerX, centerY + buttonHeight + spacing, buttonWidth, buttonHeight), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        if (runner.IsServer) {
            // Obtener el ancho de la pantalla en unidades del mundo
            float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

            // Determinar si el jugador es el primer jugador o el segundo
            bool isHost = _spawnedCharacters.Count == 0;
            float paddleXPosition = isHost ? -8 : 8; // Izquierda para Host, Derecha para Cliente

            // Posición de la paleta
            Vector2 spawnPosition = new Vector2(paddleXPosition, 0);

            // Instanciar la paleta en la posición correcta
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            // Guardar referencia a la paleta
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
      if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) {
        runner.Despawn(networkObject);
        _spawnedCharacters.Remove(player);
      }
    }

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
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
    ArraySegment<byte> data) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float
    progress) {}
}
