using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class GameEventWriter
{
    private UdpCNetworkDriver driver;
    private UdpCNetworkDriver.Concurrent concurrentDriver;
    private bool isConcurrentDriver;
    private NetworkConnection conn;
    public GameEventWriter(UdpCNetworkDriver driver, NetworkConnection conn)
    {
        this.driver = driver;
        this.conn = conn;
        this.isConcurrentDriver = false;
    }

    public GameEventWriter(UdpCNetworkDriver.Concurrent driver, NetworkConnection conn)
    {
        this.concurrentDriver = driver;
        this.conn = conn;
        this.isConcurrentDriver = true;
    }

    public static byte[] GetEventBytes(GameEvent e)
    {
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, e);
        return stream.ToArray();
    }

    public void SendEvent(byte[] bytesToSend)
    {
        using (var writer = new DataStreamWriter(bytesToSend.Length + 4, Allocator.Temp))
        {
            writer.Write(bytesToSend.Length); //send the length of the event to tell reader how much to read
            writer.Write(bytesToSend);

            if (isConcurrentDriver)
            {
                concurrentDriver.Send(conn, writer);
            }
            else
            {
                driver.Send(conn, writer);
            }
        }
    }

    public void SendEvent(GameEvent e)
    {
        byte[] bytesToSend = GetEventBytes(e);
        SendEvent(bytesToSend);
    }
}
