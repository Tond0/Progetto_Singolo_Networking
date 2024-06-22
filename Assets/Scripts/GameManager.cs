using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool startRound;

    public static GameManager current;

    private void Awake()
    {
        if (current)
            NetworkServer.Destroy(gameObject);
        else
            current = this;
    }

    public static Action OnRoundStarted;
    public static Action OnRoundEnded;

    private void OnEnable()
    {
        CustomNetworkManager.OnAllClientConnected += StartGame;
    }

    private void OnDisable()
    {
        CustomNetworkManager.OnAllClientConnected -= StartGame;
    }

    [Server]
    private void StartGame() => OnRoundStarted?.Invoke();

    [Server]
    private void OnValidate()
    {
        base.OnValidate();

        if (startRound)
            StartGame();
    }
}
