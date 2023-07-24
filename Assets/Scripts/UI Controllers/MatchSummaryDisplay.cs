using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class MatchSummaryDisplay : NetworkBehaviour {
    private Canvas canvas;

    private void Awake() {
        canvas = GetComponent<Canvas>();
    }

    [ClientRpc]
    public void ToggleCanvasClientRpc(bool isToggled) {
        canvas.enabled = isToggled;
    }

    public void Disconnect() {
        BackToLobbyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BackToLobbyServerRpc() {
        NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
    }
}
