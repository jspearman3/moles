using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Unity.Networking.Transport;
using System.Linq;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class GameEventReader
{
    private DataStreamReader reader;
    public GameEventReader(DataStreamReader reader)
    {
        this.reader = reader;
    }

    public static GameEvent ReadEventFromBytes(byte[] eventBytes)
    {
        System.IO.MemoryStream stream = new System.IO.MemoryStream(eventBytes);
        BinaryFormatter formatter = new BinaryFormatter();
        GameEvent e = formatter.Deserialize(stream) as GameEvent;
        if (e == null)
            throw new System.Exception("Read unrecognized event");
        return e;
    }

    public byte[] ReadEventBytes(DataStreamReader.Context ctx)
    {
        int msgLength = reader.ReadInt(ref ctx);
        return reader.ReadBytesAsArray(ref ctx, msgLength);
    }

    public GameEvent ReadEvent(DataStreamReader.Context ctx)
    {
        byte[] eventBytes = ReadEventBytes(ctx);
        return ReadEventFromBytes(eventBytes);
    }
}
