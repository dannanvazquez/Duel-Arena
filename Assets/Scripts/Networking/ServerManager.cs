using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private string characterSelectSceneName = "CharacterSelect";
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public static ServerManager Instance { get; private set; }

    private bool gameHasStarted;

    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void StartServer() {
        try {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += OnNetworkReady;
        } catch {
            Debug.Log("Callbacks already exist when starting server.");
        }

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    public void StartHost() {
        try {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += OnNetworkReady;
        } catch {
            Debug.Log("Callbacks already exist when starting server.");
        }

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartHost();
    }

    public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (ClientData.Count >= 2 || gameHasStarted) {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId, ClientData.Count % 2);

        Debug.Log($"Added client {request.ClientNetworkId} to team {ClientData[request.ClientNetworkId].teamId}");
    }

    public void OnNetworkReady() {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectSceneName, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId) {
        if (ClientData.ContainsKey(clientId) && ClientData.Remove(clientId)) {
            Debug.Log($"Removed client {clientId}");
            //SceneManager.LoadScene(mainMenuSceneName);
            //Destroy(gameObject);
        }
    }

    public void SetCharacter(ulong clientId, int characterId) {
        if (ClientData.TryGetValue(clientId, out ClientData data)) {
            data.characterId = characterId;
        }
    }

    public void StartGame() {
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}
