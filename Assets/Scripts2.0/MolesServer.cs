
using UnityEngine;

public class MolesServer : MonoBehaviour
{
    public ushort PortToRunOn = 30135;
    public ushort MaxNumOfPlayers = 20;
    private ServerConnectionManager connectionManager;

    void Start()
    {
        Debug.Log("hello world");
        connectionManager = new ServerConnectionManager(PortToRunOn, MaxNumOfPlayers);
    }

    public void OnDestroy()
    {
        // Make sure we run our jobs to completion before exiting.
        connectionManager.Dispose();
    }

    void LateUpdate()
    {
        connectionManager.LateUpdate();
    }

    float eventTimer = 5.0f;

    void handleClientRequests()
    {
        GameEvent[] requests = connectionManager.GetNewClientEvents();
        foreach (GameEvent e in requests)
        {

            if (e as ClientConnectEvent != null)
            {
                ClientConnectEvent conn = e as ClientConnectEvent;
                Debug.Log("Client " + conn.NetworkConnectionID + " has connected!");
            } else if (e as ClientDisconnectEvent != null)
            {
                ClientDisconnectEvent conn = e as ClientDisconnectEvent;
                Debug.Log("Client " + conn.NetworkConnectionID + " has disconnected!");
            } else if (e as ClientRequest != null)
            {
                ClientRequest conn = e as ClientRequest;
                Debug.Log("Client sent event: " + conn.EventName);
            }
        }
    }

    void Update()
    {
        connectionManager.Update();

        eventTimer -= Time.deltaTime;
        if (eventTimer < 0)
        {
            connectionManager.SendCommandToAllClients(new ServerCommand1("event 1 woo"));
            eventTimer = 5.0f;
        }

        handleClientRequests();
    }
}