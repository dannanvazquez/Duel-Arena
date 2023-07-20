using System;

[Serializable]
public class ClientData {
    public ulong clientId;
    public int characterId = -1;
    public int teamId = -1;

    public ClientData(ulong clientId, int teamId) {
        this.clientId = clientId;
        this.teamId = teamId;
    }
}
