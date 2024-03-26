using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    private NetworkVariable<NetworkString> _playerName = new NetworkVariable<NetworkString>();

    private bool _overlayIsSet = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            _playerName.Value = "Player " + OwnerClientId;
        }
    }

    private void SetOverlay()
    {
        var _localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _localPlayerOverlay.text = _playerName.Value;
    }

    private void Update()
    {
        if (!_overlayIsSet && !string.IsNullOrEmpty(_playerName.Value)) {
            SetOverlay();
            _overlayIsSet = true;
        }
    }
}
