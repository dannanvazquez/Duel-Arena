using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostManager : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private int maxConnections = 2;
    [SerializeField] private string characterSelectSceneName = "CharacterSelect";
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public static HostManager Instance { get; private set; }

    private bool gameHasStarted;

    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    public string JoinCode { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void StartServer() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    public async void StartHost() {
        Allocation allocation;

        try {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        } catch(Exception e) {
            Debug.LogError($"Relay create allocation request failed: {e.Message}");
            throw;
        }

        Debug.Log($"Server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"Server: {allocation.AllocationId}");

        try {
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        } catch {
            Debug.LogError($"Relay get join code request failed.");
            throw;
        }

        var relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

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
