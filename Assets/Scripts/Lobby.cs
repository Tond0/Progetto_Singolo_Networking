using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class Lobby : MonoBehaviour
{
    public void Host()
    {
        NetworkManager.singleton.StartHost();   
    }
}
