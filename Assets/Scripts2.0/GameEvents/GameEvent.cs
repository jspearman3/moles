using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

[Serializable]
public abstract class GameEvent
{
    public abstract string EventName
    {
        get;
    }
}