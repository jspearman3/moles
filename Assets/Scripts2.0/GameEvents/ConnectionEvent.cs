using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using System;

[Serializable]
public abstract class ConnectionEvent : GameEvent
{
    public abstract int NetworkConnectionID
    {
        get;
    }
}

[Serializable]
public class ClientConnectEvent : ConnectionEvent
{
    private int conn;
    public ClientConnectEvent(NetworkConnection conn)
    {
        this.conn = conn.InternalId;
    }
    public override int NetworkConnectionID
    {
        get { return conn; }
    }
    public override string EventName
    {
        get { return "connection " + conn; }
    }
}

[Serializable]
public class ClientDisconnectEvent : ConnectionEvent
{
    private int conn;
    public ClientDisconnectEvent(NetworkConnection conn)
    {
        this.conn = conn.InternalId;
    }
    public override int NetworkConnectionID
    {
        get { return conn; }
    }
    public override string EventName
    {
        get { return "disconnection " + conn; }
    }
}
