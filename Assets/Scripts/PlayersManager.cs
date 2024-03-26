using Ata.Singleton;
using Unity.Netcode;

public class PlayersManager : Singleton<PlayersManager>
{
    private NetworkVariable<int> _playersInGame = new NetworkVariable<int>();

    public int PlayersInGame
    {
        get
        {
            return _playersInGame.Value;
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer) {
                _playersInGame.Value++;
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer) {
                _playersInGame.Value--;
            }
        };
    }
}
