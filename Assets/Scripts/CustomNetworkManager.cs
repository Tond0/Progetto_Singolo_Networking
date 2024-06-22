using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public static Action OnAllClientConnected;
    
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        if(NetworkServer.connections.Count == 2)
        OnAllClientConnected?.Invoke();
    }
}
