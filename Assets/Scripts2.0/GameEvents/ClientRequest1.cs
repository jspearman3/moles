using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

[Serializable]
public class ClientRequest1 : ClientRequest
{
    private string name;

    public ClientRequest1(string name)
    {
        this.name = "CLIENT SAYS: " + name;
    }
    public override string EventName
    {
        get { return name; }
    }
}
