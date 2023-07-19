using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour {
    [SerializeField] private CharacterDatabase characterDatabase;

    public override void OnNetworkSpawn() {
        if (!IsServer) return;

        foreach (var client in ServerManager.Instance.ClientData) {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null) {
                var spawnPos = new Vector3(0f, 0.5625f, 0f);
                var characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}
