using System.Net;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.SceneManagement;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System.Collections;

internal enum ClientState { Disconnected, Initializing, Playing }

public class MolesClient : MonoBehaviour
{
    public string MainMenuScenePath;
    public string WorldScenePath;

    private ClientState clientState;

    private ClientConnectionManager connectionManager;

    void Start()
    {
        connectionManager = new ClientConnectionManager();
        DontDestroyOnLoad(this.gameObject);
        clientState = ClientState.Disconnected;
        SceneManager.LoadScene(MainMenuScenePath);
    }

    public void OnDestroy()
    {
        connectionManager.Dispose();
    }

    void handleIncomingEvents()
    {
        GameEvent[] requests = connectionManager.GetNewIncomingEvents();
        foreach (GameEvent e in requests)
        {

            if (e as ClientConnectEvent != null)
            {
                ClientConnectEvent conn = e as ClientConnectEvent;
                Debug.Log("CLIENT: We have connected!");
                SceneManager.LoadScene(WorldScenePath);
            }
            else if (e as ClientDisconnectEvent != null)
            {
                ClientDisconnectEvent conn = e as ClientDisconnectEvent;
                Debug.Log("CLIENT: We have disconnected!");
                SceneManager.LoadScene(MainMenuScenePath);
            }
            else if (e as ServerCommand != null)
            {
                ServerCommand conn = e as ServerCommand;
                Debug.Log("Server sent event: " + conn.EventName);
            }
        }
    }

    public void connect(string host, ushort port, string username)
    {
        //TODO: save username for auth flow
        connectionManager.connect(host, port);
    }

    public void disconnect()
    {
        connectionManager.disconnect();
    }

    void Update()
    {
        connectionManager.Update();
        handleIncomingEvents();
    }
}
