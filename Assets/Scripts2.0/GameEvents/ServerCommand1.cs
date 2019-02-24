using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ServerCommand1 : ServerCommand
{
    private string name;

    public ServerCommand1(string name)
    {
        this.name = "SERVER SAYS: " + name;
    }
    public override string EventName
    {
        get { return name; }
    }
}
