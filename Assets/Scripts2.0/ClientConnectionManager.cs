using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using System;
using System.Net;
using System.Linq;

struct ServerCommunicationJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeList<byte> serverCommandEventBytes;
    public NativeList<int> serverCommandEventLengths;
    public NativeList<byte> clientRequestEventBytes;
    public NativeList<int> clientRequestEventLengths;
    public NativeArray<byte> didClientConnect;
    public NativeArray<byte> didClientDisconnect;
    private void addNewServerCommand(byte[] newEventBytes)
    {
        NativeArray<byte> nativeNewEventBytes = new NativeArray<byte>(newEventBytes, Allocator.Persistent);
        serverCommandEventBytes.AddRange(nativeNewEventBytes);
        nativeNewEventBytes.Dispose();
        serverCommandEventLengths.Add(newEventBytes.Length);
    }

    public void Execute()
    {
        DataStreamReader stream;
        if (!connection[0].IsCreated)
            Debug.Log("connection is not created!");

        //send all commands to the client
        byte[] commandBytes = clientRequestEventBytes.ToArray();
        int[] commandLengths = clientRequestEventLengths.ToArray();
        while (commandLengths.Length != 0)
        {
            byte[] nextCommand = commandBytes.Take(commandLengths[0]).ToArray();

            //Send out client requests to the server
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {

                GameEventWriter eventWriter = new GameEventWriter(driver, connection[0]);
                eventWriter.SendEvent(nextCommand);
            }
            commandBytes = commandBytes.Skip(commandLengths[0]).ToArray();
            commandLengths = commandLengths.Skip(1).ToArray();
        }

        //clear nativearrays after commands sent
        clientRequestEventBytes.Clear();
        clientRequestEventLengths.Clear();

        //Read in new commands from the server
        NetworkEvent.Type cmd;
        while ((cmd = connection[0].PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");
                didClientConnect[0] = 1;
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                GameEventReader eventReader = new GameEventReader(stream);
                byte[] newEventBytes = eventReader.ReadEventBytes(readerCtx);
                addNewServerCommand(newEventBytes);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                didClientDisconnect[0] = 1;
            }
        }
    }
}

public class ClientConnectionManager : IDisposable
{
    private class ClientConnectionByteStreams : IDisposable
    {
        //ClientRequest events for each network connection
        public NativeList<byte> clientRequestBytesToSend = new NativeList<byte>(Allocator.Persistent);
        public NativeList<int> clientRequestLengthsToSend = new NativeList<int>(Allocator.Persistent);

        //ServerCommand events for each network connection
        public NativeList<byte> serverCommandBytesToRead = new NativeList<byte>(Allocator.Persistent);
        public NativeList<int> serverCommandLengthsToRead = new NativeList<int>(Allocator.Persistent);

        //Indication that client connected
        public NativeArray<byte> didClientConnect = new NativeArray<byte>(new byte[] { 0 }, Allocator.Persistent);
        //Indication that client disconnected
        public NativeArray<byte> didClientDisconnect = new NativeArray<byte>(new byte[] { 0 }, Allocator.Persistent);

        public void Clear()
        {
            clientRequestBytesToSend.Clear();
            clientRequestLengthsToSend.Clear();
            serverCommandBytesToRead.Clear();
            serverCommandLengthsToRead.Clear();
            didClientConnect[0] = 0;
            didClientDisconnect[0] = 0;
        }

        public void Dispose()
        {
            clientRequestBytesToSend.Dispose();
            clientRequestLengthsToSend.Dispose();
            serverCommandBytesToRead.Dispose();
            serverCommandLengthsToRead.Dispose();
            didClientConnect.Dispose();
            didClientDisconnect.Dispose();
        }
    }
    //Moles server endpoint
    public static NetworkEndPoint ServerEndPoint { get; private set; }

    private UdpCNetworkDriver m_Driver;
    private NativeArray<NetworkConnection> m_Connection;

    private JobHandle ClientJobHandle;

    public uint heartbeatLengthInSeconds = 1;
    private float heartbeatTimer = 0;

    private bool connectRequested;
    private bool disconnectRequested;

    private ClientConnectionByteStreams byteStreams;

    //Lists that are populated/returned by connection manager to outside world
    private List<ClientRequest> pendingClientRequests = new List<ClientRequest>();
    private List<GameEvent> eventsToReturn = new List<GameEvent>();

    public ClientConnectionManager()
    {
        INetworkParameter[] netParams = new INetworkParameter[1];
        netParams[0] = new NetworkConfigParameter { connectTimeoutMS = 1000, maxConnectAttempts = 5, disconnectTimeoutMS = 15000 };
        m_Driver = new UdpCNetworkDriver(netParams);
        m_Connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        ServerEndPoint = default(NetworkEndPoint);
        heartbeatTimer = heartbeatLengthInSeconds;
        byteStreams = new ClientConnectionByteStreams();
    }

    private List<GameEvent> readBytesAndLengthsIntoEvents(byte[] bytes, int[] lengths)
    {
        List<GameEvent> newEvents = new List<GameEvent>();
        byte[] allEventBytes = bytes.ToArray();
        foreach (int eventLength in lengths.ToArray())
        {
            byte[] firstEventBytes = allEventBytes.Take(eventLength).ToArray();
            allEventBytes = allEventBytes.Skip(eventLength).ToArray();
            GameEvent gameEvent = GameEventReader.ReadEventFromBytes(firstEventBytes);
            newEvents.Add(gameEvent);
        }
        return newEvents;
    }

    private void heartbeat()
    {
        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer <= 0)
        {
            Debug.Log("CLIENT: sending heartbeat");
            pendingClientRequests.Add(new ClientKeepAliveRequest());
            heartbeatTimer = heartbeatLengthInSeconds;
        }
    }

    private JobHandle handlServerCommunication(JobHandle preHandle)
    {

        var serverUpdateJob = new ServerCommunicationJob
        {
            driver = m_Driver,
            connection = m_Connection,
            clientRequestEventBytes = byteStreams.clientRequestBytesToSend,
            clientRequestEventLengths = byteStreams.clientRequestLengthsToSend,
            serverCommandEventBytes = byteStreams.serverCommandBytesToRead,
            serverCommandEventLengths = byteStreams.serverCommandLengthsToRead,
            didClientConnect = byteStreams.didClientConnect,
            didClientDisconnect = byteStreams.didClientDisconnect
        };
        return serverUpdateJob.Schedule(preHandle);
    }

    private void updateEvents()
    {
        //add connection event if connected
        if (byteStreams.didClientConnect[0] == 1)
        {
            eventsToReturn.Add(new ClientConnectEvent(m_Connection[0]));
            byteStreams.didClientConnect[0] = 0;
        }
            

        //add disconnected event and reset connection/endpoint if disconnected
        if (byteStreams.didClientDisconnect[0] == 1 || disconnectRequested)
        {
            //if we didn't send disconnect, send it now
            if (byteStreams.didClientDisconnect[0] != 1)
                m_Connection[0].Disconnect(m_Driver);

            eventsToReturn.Add(new ClientDisconnectEvent(m_Connection[0]));
            ServerEndPoint = default(NetworkEndPoint);
            m_Connection[0] = default(NetworkConnection);
            disconnectRequested = false;
            byteStreams.Clear(); //We are disconnected. Drop all stream data and return.
            return;
        }

        //store all obtained events in the GameEvent list to be returned
        byte[] serverCommandBytes = byteStreams.serverCommandBytesToRead.ToArray();
        int[] serverCommandLengths = byteStreams.serverCommandLengthsToRead.ToArray();
        List<GameEvent> serverCommands =
                readBytesAndLengthsIntoEvents(serverCommandBytes, serverCommandLengths)
                .Where(e => e is GameEvent)
                .Select(e => e as GameEvent).ToList();
        byteStreams.serverCommandBytesToRead.Clear();
        byteStreams.serverCommandLengthsToRead.Clear();
        eventsToReturn.AddRange(serverCommands);

        //Parse ClientRequests into bytes to be sent to server
        List<byte> allRequestBytes = new List<byte>();
        List<int> allRequestLengths = new List<int>();
        foreach (ClientRequest request in pendingClientRequests)
        {
            byte[] requestBytes = GameEventWriter.GetEventBytes(request);
            allRequestBytes.AddRange(requestBytes);
            allRequestLengths.Add(requestBytes.Length);
        }
        pendingClientRequests.Clear();

        NativeArray<byte> nativeNewEventBytes = new NativeArray<byte>(allRequestBytes.ToArray(), Allocator.Persistent);
        byteStreams.clientRequestBytesToSend.AddRange(nativeNewEventBytes);
        nativeNewEventBytes.Dispose();

        NativeArray<int> nativeNewEventLengths = new NativeArray<int>(allRequestLengths.ToArray(), Allocator.Persistent);
        byteStreams.clientRequestLengthsToSend.AddRange(nativeNewEventLengths);
        nativeNewEventLengths.Dispose();
    }

    public void Update()
    {
        ClientJobHandle.Complete();
        if (ServerEndPoint.IsValid && m_Connection[0].IsCreated)
        {
            heartbeat();

            ClientJobHandle = m_Driver.ScheduleUpdate();
            ClientJobHandle = handlServerCommunication(ClientJobHandle);
            ClientJobHandle.Complete();
            //complete the job to bring back NativeArray ownership to main thread. Need to update events.
            updateEvents();
        }
        else
        {
            if (connectRequested)
            {
                Debug.Log("sending connect message");
                connectRequested = false;
                m_Connection[0] = m_Driver.Connect(ServerEndPoint);
            }
        }
    }

    public void connect(string host, ushort port)
    {
        if (!ServerEndPoint.IsValid)
        {
            Debug.Log("Attempting to connect");
            ServerEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            connectRequested = true;
        }
        else
        {
            Debug.Log("Already connected or attempting connection!");
        }
    }

    public void disconnect()
    {
        if (ServerEndPoint.IsValid)
        {
            Debug.Log("Attempting to disconnect");
            disconnectRequested = true;
        }
        else
        {
            Debug.Log("Already disconnected or attempting disconnection!");
        }
    }

    public GameEvent[] GetNewIncomingEvents()
    {
        GameEvent[] events = eventsToReturn.ToArray();
        eventsToReturn.Clear();
        return events;
    }

    public void SendCommandToServer(ClientRequest request)
    {
        pendingClientRequests.Add(request);
    }

    public void Dispose()
    {
        ClientJobHandle.Complete();
        m_Connection.Dispose();
        m_Driver.Dispose();
        byteStreams.Dispose();
    }
}
