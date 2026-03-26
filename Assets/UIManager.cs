using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    [SerializeField]
    private NetworkManagerController _networkController;

    [SerializeField]
    private TextMeshProUGUI _statusText;

    [SerializeField]
    private Button _serverButton;
    [SerializeField]
    private Button _clientButton;
    [SerializeField]
    private Button _hostButton;
    [SerializeField]
    private Button _disconnectButton;

    private void Awake()
    {
        if (!_networkController)
            _networkController = FindFirstObjectByType<NetworkManagerController>();
        _networkController.OnDisplayText += OnDisplayText;

        _serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            DisplayConnectionButtons(false);
        });

        _clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            DisplayConnectionButtons(false);
        });

        _hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            DisplayConnectionButtons(false);
        });

        _disconnectButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            DisplayConnectionButtons(true);
        });

        DisplayConnectionButtons(true);
    }

    private void OnDisable()
    {
        if (_networkController)
            _networkController.OnDisplayText -= OnDisplayText;
    }

    private void OnDisplayText(string text) =>
        _statusText.text = text;

    private void DisplayConnectionButtons(bool display)
    {
        _serverButton.gameObject.SetActive(display);
        _clientButton.gameObject.SetActive(display);
        _hostButton.gameObject.SetActive(display);
        _disconnectButton.gameObject.SetActive(!display);
    }
}

