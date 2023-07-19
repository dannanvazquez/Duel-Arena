using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour {
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Vector3[] spawnPositions = null;

    public override void OnNetworkSpawn() {
        if (!IsServer) return;

        int spawnIndex = 0;
        foreach (var client in ServerManager.Instance.ClientData) {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null) {
                var spawnPos = spawnPositions[spawnIndex];
                var characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }

            spawnIndex++;
            spawnIndex %= spawnPositions.Length;
        }
    }
}
