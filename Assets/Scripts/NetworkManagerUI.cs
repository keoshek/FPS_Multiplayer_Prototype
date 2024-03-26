using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    private RelayManager _relayManager;

    [SerializeField] private Button _serverButton;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;

    [SerializeField] private TMP_InputField _joinCodeInput;

    [SerializeField] private TextMeshProUGUI _codeHost;
    [SerializeField] private TextMeshProUGUI _playersInGameText;

    private void Awake()
    {
        _relayManager = NetworkManager.Singleton.gameObject.GetComponent<RelayManager>();

        _serverButton.onClick.AddListener(() =>
        {
            HandleServerClick();
        });
        _hostButton.onClick.AddListener(() =>
        {
            HandleHostClick();
        });
        _clientButton.onClick.AddListener(() =>
        {
            HandleClientClick();
        });
    }

    private void Update()
    {
        _playersInGameText.text = "Players in game: " + PlayersManager.Instance.PlayersInGame.ToString();
    }

    private void HandleServerClick()
    {
        NetworkManager.Singleton.StartServer();
        HandleGameStart();
    }

    private void HandleHostClick()
    {
        //NetworkManager.Singleton.StartHost();
        _relayManager.StartHostWithRelay();
        HandleGameStart();
    }

    private void HandleClientClick()
    {
        if (_joinCodeInput.text.Length == 0) return;

        //NetworkManager.Singleton.StartClient();
        _relayManager.StartClientWithRelay(_joinCodeInput.text);
        HandleGameStart();
    }

    private void HandleGameStart()
    {
        _serverButton.gameObject.SetActive(false);
        _clientButton.gameObject.SetActive(false);
        _hostButton.gameObject.SetActive(false);
        _joinCodeInput.gameObject.SetActive(false);
        _playersInGameText.gameObject.SetActive(true);
    }

    public void ShowEntryCode(string _code)
    {
        _codeHost.gameObject.SetActive(true);
        _codeHost.text = "Entry code: " + _code;
    }

    
}
