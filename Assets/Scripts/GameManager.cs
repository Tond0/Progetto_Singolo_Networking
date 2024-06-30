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

    //FIXME: magari fargli dare dei nomi invece di fare p1 o p2?
    //Bool sta per Is_P1?
    public static Action<bool> OnMatchOver;
    
    public static Action<bool, int> OnScoreUpdate;


    [Header("Match Settings")]

    #region Score Variables
    [SerializeField, Tooltip("How many points to win a match?")] private int points_to_win = 3;

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
    /// <param "name="p1_scored">Il player che ha fatto goal</param>
    
    public void UpdateScore(NetworkConnectionToClient conn)
    {
        if(!scoreboard.TryGetValue(conn, out int score)) return;

        score += 1;
        scoreboard[conn] = score;

        bool is_P1 = conn.connectionId == 0 ? true : false;
        OnScoreUpdate?.Invoke(is_P1, score);

        //Ha vinto?
        if (score < points_to_win) return;

        RpcMatchOver(is_P1);
    }

    //Call match over on all clients
    [ClientRpc]
    private void RpcMatchOver(bool is_P1) => OnMatchOver?.Invoke(is_P1); 
}
