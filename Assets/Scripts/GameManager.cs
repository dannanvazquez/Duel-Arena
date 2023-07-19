using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    [SerializeField] private GameObject ballPrefab;

    private void Start() {
        if (!IsServer) return;

        GameObject ball = Instantiate(ballPrefab, new Vector3(0f, 0.3125f, 0f), Quaternion.identity);
        ball.GetComponent<NetworkObject>().Spawn();
    }
}
