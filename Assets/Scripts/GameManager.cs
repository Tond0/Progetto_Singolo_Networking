using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static Action OnRoundStarted;
    public static Action OnRoundEnded;

    public static Action<NetworkConnectionToClient> OnMatchOver;
    
    public static Action<bool, int> OnScoreUpdate;


    [Header("Match Settings")]

    #region Score Variables
    [SerializeField, Tooltip("How many points to win a match?")] private int points_to_win;

    private Dictionary<NetworkConnectionToClient, int> scoreboard = new();

    #endregion

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


    private void OnEnable()
    {
        CustomNetworkManager.OnAllClientConnected += StartGame;
        
        CustomNetworkManager.OnPlayerConnected += AddPlayerToScoreboard;

        Goal.OnGoal += UpdateScore;
    }

    private void OnDisable()
    {
        CustomNetworkManager.OnAllClientConnected -= StartGame;
        CustomNetworkManager.OnPlayerConnected -= AddPlayerToScoreboard;
        Goal.OnGoal -= UpdateScore;
    }

    //Add a player to a scoreboard
    private void AddPlayerToScoreboard(NetworkConnectionToClient conn)
    {
        //Debug.LogError(conn);
        scoreboard.Add(conn, 0);
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

    /// <summary>
    /// Update the current score
    /// </summary>
    /// <param name="p1_scored">Il player che ha SUBITO il goal</param>
    public void UpdateScore(NetworkConnectionToClient conn)
    {
        if(!scoreboard.TryGetValue(conn, out int score)) return;

        score += 1;

        bool is_P1 = conn.identity.isServer;
        OnScoreUpdate?.Invoke(is_P1, score);

        //Ha vinto?
        if (score < points_to_win) return;

        OnMatchOver?.Invoke(conn);
    }
}
