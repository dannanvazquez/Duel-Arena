using Unity.Netcode;
using UnityEngine;

public class GoalColliderScore : NetworkBehaviour {
    [SerializeField] private GameManager gameManager;

    [SerializeField] private int teamId = -1;

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (other.CompareTag("Ball")) {
            other.GetComponent<NetworkObject>().Despawn();
            Destroy(other.gameObject);

            gameManager.ScoreGoal(teamId);
        }
    }
}
