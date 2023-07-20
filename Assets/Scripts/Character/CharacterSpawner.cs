using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class CharacterSpawner : NetworkBehaviour {
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Transform[] spawnTransforms = null;

    public override void OnNetworkSpawn() {
        if (!IsServer) return;

        foreach (var client in ServerManager.Instance.ClientData) {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null) {
                var spawnTransform = spawnTransforms[client.Value.teamId];
                var characterInstance = Instantiate(character.GameplayPrefab, spawnTransform.position, spawnTransform.rotation);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }

    public void RespawnPlayers() {
        foreach (var client in ServerManager.Instance.ClientData) {
            var currentPlayerObject = NetworkManager.Singleton.ConnectedClients[client.Value.clientId].PlayerObject;
            currentPlayerObject.Despawn(true);

            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null) {
                var spawnTransform = spawnTransforms[client.Value.teamId];
                var characterInstance = Instantiate(character.GameplayPrefab, spawnTransform.position, spawnTransform.rotation);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}
