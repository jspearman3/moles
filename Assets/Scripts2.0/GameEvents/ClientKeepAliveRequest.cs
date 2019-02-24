using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ClientKeepAliveRequest : ClientRequest
{
    public override string EventName
    {
        get { return "heartbeat"; }
    }
}
