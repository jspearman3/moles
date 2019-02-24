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

struct ServerUpdateConnectionsJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeList<NetworkConnection> connections;
    public NativeList<byte> connectionEventBytesToRead;
    public NativeList<int> connectionEventLengthsToRead;

    private void addNewEvent(byte[] newEventBytes)
    {
        NativeArray<byte> nativeNewEventBytes = new NativeArray<byte>(newEventBytes, Allocator.Persistent);
        connectionEventBytesToRead.AddRange(nativeNewEventBytes);
        nativeNewEventBytes.Dispose();
        connectionEventLengthsToRead.Add(newEventBytes.Length);
    }

    public void Execute()
    {
        // CleanUpConnections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
        // AcceptNewConnections
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            byte[] connectEventBytes = GameEventWriter.GetEventBytes(new ClientConnectEvent(c));
            addNewEvent(connectEventBytes);
            Debug.Log("Accepted a connection");
        }
    }
}

struct ClientCommunicationJob : IJob
{
    public UdpCNetworkDriver.Concurrent driver;
    public NativeArray<NetworkConnection> connection;
    public NativeList<byte> clientRequestEventBytes;
    public NativeList<int> clientRequestEventLengths;
    public NativeList<byte> serverCommandEventBytes;
    public NativeList<int> serverCommandEventLengths;
    public NativeArray<byte> didClientDisconnect;
    private void addNewClientRequest(byte[] newEventBytes)
    {
        NativeArray<byte> nativeNewEventBytes = new NativeArray<byte>(newEventBytes, Allocator.Persistent);
        clientRequestEventBytes.AddRange(nativeNewEventBytes);
        nativeNewEventBytes.Dispose();
        clientRequestEventLengths.Add(newEventBytes.Length);
    }

    public void Execute()
    {
        DataStreamReader stream;
        if (!connection[0].IsCreated)
            Debug.Log("connection is not created!");

        //send all commands to the client
        byte[] commandBytes = serverCommandEventBytes.ToArray();
        int[] commandLengths = serverCommandEventLengths.ToArray();
        while (commandLengths.Length != 0)
        {
            byte[] nextCommand = commandBytes.Take(commandLengths[0]).ToArray();

            //Send out server commands to the client
            using (var writer = new DataStreamWriter(4, Allocator.Temp))
            {
                
                GameEventWriter eventWriter = new GameEventWriter(driver, connection[0]);
                eventWriter.SendEvent(nextCommand);
            }
            commandBytes = commandBytes.Skip(commandLengths[0]).ToArray();
            commandLengths = commandLengths.Skip(1).ToArray();
        }

        //clear nativearrays after commands sent
        serverCommandEventBytes.Clear();
        serverCommandEventLengths.Clear();

        //Read in new requests from a client
        NetworkEvent.Type cmd;
        while ((cmd = driver.PopEventForConnection(connection[0], out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                GameEventReader eventReader = new GameEventReader(stream);
                byte[] newEventBytes = eventReader.ReadEventBytes(readerCtx);
                addNewClientRequest(newEventBytes);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                didClientDisconnect[0] = 1;
            }
        }
    }
}



public class ServerConnectionManager : IDisposable
{
    private class ServerConnectionByteStreams : IDisposable
    {
        //ClientRequest events for each network connection
        public NativeList<byte> clientRequestBytesToRead = new NativeList<byte>(Allocator.Persistent);
        public NativeList<int> clientRequestLengthsToRead = new NativeList<int>(Allocator.Persistent);

        //ServerCommand events for each network connection
        public NativeList<byte> serverCommandBytesToSend = new NativeList<byte>(Allocator.Persistent);
        public NativeList<int> serverCommandLengthsToSend = new NativeList<int>(Allocator.Persistent);

        //Indication that client disconnected
        public NativeArray<byte> didClientDisconnect = new NativeArray<byte>(new byte[] { 0 }, Allocator.Persistent);

        public void Dispose()
        {
            clientRequestBytesToRead.Dispose();
            clientRequestLengthsToRead.Dispose();
            serverCommandBytesToSend.Dispose();
            serverCommandLengthsToSend.Dispose();
            didClientDisconnect.Dispose();
        }
    }
    //Unity.Networking.Transport internals
    private UdpCNetworkDriver m_Driver;
    private JobHandle ServerJobHandle;

    //List of connections
    private NativeList<NetworkConnection> m_Connections;

    //ClientConnectEvents in byte form
    private NativeList<byte> connectionEventBytesToRead;
    private NativeList<int> connectionEventLengthsToRead;

    //client byte stream refs
    private Dictionary<NativeList<NetworkConnection>, ServerConnectionByteStreams> connectionByteStreamMap;

    //Lists that are populated/returned by connection manager to outside world
    private List<ServerCommand> pendingServerAllClientCommands = new List<ServerCommand>();
    private List<GameEvent> eventsToReturn = new List<GameEvent>();

    public ServerConnectionManager(ushort port, ushort maxConnections)
    {
        m_Connections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        connectionEventBytesToRead = new NativeList<byte>(Allocator.Persistent);
        connectionEventLengthsToRead = new NativeList<int>(Allocator.Persistent);
        connectionByteStreamMap = new Dictionary<NativeList<NetworkConnection>, ServerConnectionByteStreams>();
        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, port)) != 0)
            throw new Exception("Failed to bind to port " + port);
        else
            m_Driver.Listen();
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

    private JobHandle updateClientConnections(JobHandle preHandle)
    {
        var connectionJob = new ServerUpdateConnectionsJob
        {
            driver = m_Driver,
            connections = m_Connections,
            connectionEventBytesToRead = connectionEventBytesToRead,
            connectionEventLengthsToRead = connectionEventLengthsToRead
        };

        return connectionJob.Schedule(preHandle);
    }

    private void updateMapsAndEvents()
    {
        //Remove all disconnected clients and add disconnected event
        List<NativeList<NetworkConnection>> connectionsToRemove = new List<NativeList<NetworkConnection>>();
        foreach (KeyValuePair<NativeList<NetworkConnection>, ServerConnectionByteStreams> entry in connectionByteStreamMap)
        {
            if (entry.Value.didClientDisconnect[0] == 1)
            {
                eventsToReturn.Add(new ClientDisconnectEvent(entry.Key[0]));
                connectionsToRemove.Add(entry.Key);
                entry.Value.Dispose();
            }
        }
        foreach (NativeList<NetworkConnection> entryKey in connectionsToRemove)
        {
            int indexToRemove = Array.IndexOf(m_Connections.ToArray(), entryKey[0]);
            if (indexToRemove != -1)
                m_Connections.RemoveAtSwapBack(indexToRemove);
            //remove map entries for the disconnected client
            connectionByteStreamMap.Remove(entryKey);
            entryKey.Dispose();
        }

        //store all obtained events in the GameEvent list to be returned
        foreach (KeyValuePair<NativeList<NetworkConnection>, ServerConnectionByteStreams> entry in connectionByteStreamMap)
        {
            byte[] clientRequestBytes = entry.Value.clientRequestBytesToRead.ToArray();
            int[] clientRequestLengths = entry.Value.clientRequestLengthsToRead.ToArray();
            List<ClientRequest> clientRequests =
                 readBytesAndLengthsIntoEvents(clientRequestBytes, clientRequestLengths)
                    .Where(e => e is ClientRequest)
                    .Select(e => e as ClientRequest).ToList();
            entry.Value.clientRequestBytesToRead.Clear();
            entry.Value.clientRequestLengthsToRead.Clear();
            eventsToReturn.AddRange(clientRequests);
        }

        //Parse ServerCommands into bytes to be sent to clients
        foreach (KeyValuePair<NativeList<NetworkConnection>, ServerConnectionByteStreams> entry in connectionByteStreamMap)
        {
            List<byte> allCommandBytes = new List<byte>();
            List<int> allCommandLengths = new List<int>();
            foreach (ServerCommand command in pendingServerAllClientCommands)
            {
                byte[] commandBytes = GameEventWriter.GetEventBytes(command);
                allCommandBytes.AddRange(commandBytes);
                allCommandLengths.Add(commandBytes.Length);
            }

            NativeArray<byte> nativeNewEventBytes = new NativeArray<byte>(allCommandBytes.ToArray(), Allocator.Persistent);
            entry.Value.serverCommandBytesToSend.AddRange(nativeNewEventBytes);
            nativeNewEventBytes.Dispose();

            NativeArray<int> nativeNewEventLengths = new NativeArray<int>(allCommandLengths.ToArray(), Allocator.Persistent);
            entry.Value.serverCommandLengthsToSend.AddRange(nativeNewEventLengths);
            nativeNewEventLengths.Dispose();
        }
        pendingServerAllClientCommands.Clear();


        //Add new clients that may have connected
        byte[] connectionEventBytes = connectionEventBytesToRead.ToArray();
        int[] lengths = connectionEventLengthsToRead.ToArray();
        List<ConnectionEvent> connectionEvents =
             readBytesAndLengthsIntoEvents(connectionEventBytes, lengths)
                .Where(e => e is ConnectionEvent)
                .Select(e => e as ConnectionEvent).ToList();
        connectionEventBytesToRead.Clear();
        connectionEventLengthsToRead.Clear();

        foreach (ConnectionEvent e in connectionEvents)
        {
            eventsToReturn.Add(e);

            NativeList<NetworkConnection> connectionKey = new NativeList<NetworkConnection>(1, Allocator.Persistent);

            NetworkConnection[] maybeConnections = m_Connections.ToArray().Where(c => c.InternalId == e.NetworkConnectionID).ToArray();
            Debug.Log("maybeConnections length: " + maybeConnections.Length);
            if (maybeConnections.Length == 0)
            {
                Debug.Log("Warning. Unable to find connection with ID " + e.NetworkConnectionID);
            } else
            {
                connectionKey.Add(maybeConnections[0]);
                connectionByteStreamMap[connectionKey] = new ServerConnectionByteStreams();
            }
        }
    }

    private JobHandle handleClientCommunications(JobHandle preHandle)
    {
        if (connectionByteStreamMap.Count <= 0)
            return preHandle;

        JobHandle baseHandle = default(JobHandle);
        foreach (KeyValuePair<NativeList<NetworkConnection>, ServerConnectionByteStreams> entry in connectionByteStreamMap)
        {
            var serverUpdateJob = new ClientCommunicationJob
            {
                driver = m_Driver.ToConcurrent(),
                connection = entry.Key,
                clientRequestEventBytes = entry.Value.clientRequestBytesToRead,
                clientRequestEventLengths = entry.Value.clientRequestLengthsToRead,
                serverCommandEventBytes = entry.Value.serverCommandBytesToSend,
                serverCommandEventLengths = entry.Value.serverCommandLengthsToSend,
                didClientDisconnect = entry.Value.didClientDisconnect
            };
            JobHandle handle = serverUpdateJob.Schedule(preHandle);
            baseHandle = JobHandle.CombineDependencies(baseHandle, handle);
        }

        return baseHandle;
    }

    public void Update()
    {
        ServerJobHandle.Complete();
        ServerJobHandle = m_Driver.ScheduleUpdate(ServerJobHandle);
        ServerJobHandle = updateClientConnections(ServerJobHandle);
        //complete the job to bring back NativeArray ownership to main thread. Need to update connections map.
        ServerJobHandle.Complete();
        updateMapsAndEvents();
        ServerJobHandle = handleClientCommunications(ServerJobHandle);
    }

    public void LateUpdate()
    {
        // On fast clients we can get more than 4 frames per fixed update, this call prevents warnings about TempJob
        // allocation longer than 4 frames in those cases
        ServerJobHandle.Complete();
    }

    public GameEvent[] GetNewClientEvents()
    {
        GameEvent[] events = eventsToReturn.ToArray();
        eventsToReturn.Clear();
        return events;
    }

    public void SendCommandToAllClients(ServerCommand command)
    {
        pendingServerAllClientCommands.Add(command);
    }

    public void Dispose()
    {
        ServerJobHandle.Complete();
        m_Connections.Dispose();
        m_Driver.Dispose();
        connectionEventBytesToRead.Dispose();
        connectionEventLengthsToRead.Dispose();
        foreach (KeyValuePair<NativeList<NetworkConnection>, ServerConnectionByteStreams> entry in connectionByteStreamMap)
        {
            entry.Key.Dispose();
            entry.Value.Dispose();
        }
    }
}
