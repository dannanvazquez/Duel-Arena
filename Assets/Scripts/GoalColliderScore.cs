using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GoalColliderScore : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private int teamId = -1;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (other.CompareTag("Ball")) {
            other.GetComponent<NetworkObject>().Despawn();
            Destroy(other.gameObject);

            gameManager.ScoreGoal(teamId);
            ScoreAudioClientRpc();
        }
    }

    [ClientRpc]
    private void ScoreAudioClientRpc() {
        audioSource.Play();
    }
}
