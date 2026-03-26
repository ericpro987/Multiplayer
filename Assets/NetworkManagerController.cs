using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerController : MonoBehaviour
{
    public event System.Action<string> OnDisplayText;

    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();

        _networkManager.OnConnectionEvent += ConnectionEventHandle;
    }

    private void OnDisable()
    {
        if (_networkManager)
            _networkManager.OnConnectionEvent -= ConnectionEventHandle;
    }

    private void ConnectionEventHandle(NetworkManager manager, ConnectionEventData data)
    {
        if (manager.IsHost)
            OnDisplayText?.Invoke("Host");
        else if (manager.IsServer)
            OnDisplayText?.Invoke("Server");
        else
            OnDisplayText?.Invoke("Client");
    }


}