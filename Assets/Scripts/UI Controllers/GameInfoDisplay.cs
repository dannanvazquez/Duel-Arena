using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameInfoDisplay : NetworkBehaviour {
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text scoreLeftText;
    [SerializeField] private TMP_Text scoreRightText;

    [ClientRpc]
    public void UpdateCountdownDisplayClientRpc(int currentTime) {
        if (currentTime == 0) {
            countdownText.text = "GAME!";
        } else {
            countdownText.text = currentTime.ToString();
        }
    }

    [ClientRpc]
    public void UpdateScoreClientRpc(int teamId, int score) {
        if (teamId == 0) {
            scoreLeftText.text = score.ToString();
        } else if (teamId == 1) {
            scoreRightText.text = score.ToString();
        } else {
            Debug.LogError("UpdateScoreClientRpc was provided an invalid teamId.", transform);
        }
    }
}
