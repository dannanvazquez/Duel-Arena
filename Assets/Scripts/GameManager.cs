using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    [SerializeField] private CharacterSpawner characterSpawner;
    [SerializeField] private GameInfoDisplay gameInfoDisplay;
    [SerializeField] private MatchSummaryDisplay matchSummaryDisplay;

    [SerializeField] private GameObject ballPrefab;
    private GameObject ball;

    [HideInInspector] public int teamLeft = 0;
    [HideInInspector] public int teamRight = 0;

    [Header("Settings")]
    [SerializeField] private int countdown = 60;

    private void Start() {
        if (!IsServer) return;

        SpawnBall();
        StartCoroutine(GameCountdown(countdown));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            EndGameServerRpc();
        }
    }

    public void ScoreGoal(int teamId) {
        if (teamId == 0) {
            teamLeft++;
            gameInfoDisplay.UpdateScoreClientRpc(teamId, teamLeft);
        } else if (teamId == 1) {
            teamRight++;
            gameInfoDisplay.UpdateScoreClientRpc(teamId, teamRight);
        } else {
            Debug.LogError("ScoreGoal was provided an invalid teamId.", transform);
        }

        if (characterSpawner != null) {
            characterSpawner.RespawnPlayers();
        }
        SpawnBall();
    }

    private void SpawnBall() {
        ball = Instantiate(ballPrefab, new Vector3(0f, 0.3125f, 0f), Quaternion.identity);
        ball.GetComponent<NetworkObject>().Spawn();
    }

    private IEnumerator GameCountdown(int time) {
        while (time > 0) {
            gameInfoDisplay.UpdateCountdownDisplayClientRpc(time);
            yield return new WaitForSeconds(1f);
            time--;
        }

        EndGame();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndGameServerRpc() {
        if (ball != null) {
            EndGame();
        }
    }

    private void EndGame() {
        gameInfoDisplay.UpdateCountdownDisplayClientRpc(0);
        ball.GetComponent<NetworkObject>().Despawn(true);
        matchSummaryDisplay.ToggleCanvasClientRpc(true);

        foreach (var client in HostManager.Instance.ClientData) {
            var currentPlayerObject = NetworkManager.Singleton.ConnectedClients[client.Value.clientId].PlayerObject;
            currentPlayerObject.Despawn(true);
        }
    }
}
