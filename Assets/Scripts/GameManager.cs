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
    public static Action<bool, int> OnMatchOver;
    
    public static Action<bool, int> OnScoreUpdate;


    [Header("Match Settings")]

    #region Score Variables
    [SerializeField, Tooltip("How many points to win a match?")] private int points_To_Win = 3;
    public int Points_To_Win { get { return points_To_Win; } }

    private Dictionary<int, int> scoreboard = new();
    public Dictionary<int, int> Scoreboard {  get { return scoreboard; } }

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
        CustomNetworkManager.OnAllPlayersReady += StartGame;
        CustomNetworkManager.OnPlayerReady += AddPlayerToScoreboard;
        Goal.OnGoal += UpdateScore;
    }

    private void OnDisable()
    {
        CustomNetworkManager.OnAllPlayersReady -= StartGame;
        CustomNetworkManager.OnPlayerReady -= AddPlayerToScoreboard;
        Goal.OnGoal -= UpdateScore;
    }

    //Add a player to a scoreboard
    private void AddPlayerToScoreboard(NetworkConnectionToClient conn)
    {
        //Debug.LogError(conn);
        scoreboard.Add(conn.connectionId, 0);
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
    /// <param "name="p1_scored">Il player che ha subito goal</param>
    [Server]
    public void UpdateScore(NetworkConnectionToClient conn)
    {
        OnRoundEnded?.Invoke();

        //L'ID di chi si è fatto fare goal
        int scoredPlayer_Id = conn.connectionId;
        
        //L'ID di chi ha fatto goal
        int scoringPlayer_Id;

        var P1_Id = CustomNetworkManager.singleton.Players_ID[0];
        var P2_Id = CustomNetworkManager.singleton.Players_ID[1];
        
        if (P1_Id == scoredPlayer_Id)
            scoringPlayer_Id = P2_Id;
        else
            scoringPlayer_Id = P1_Id;
        

        //Prendiamo il punteggio dalla scoreboard tramite l'ID
        if (!scoreboard.TryGetValue(scoringPlayer_Id, out int scoringPlayer_Score)) return;

        //Incremento score
        scoringPlayer_Score += 1;
        //Salvo in scoreboard
        scoreboard[scoringPlayer_Id] = scoringPlayer_Score;

        //Update dello score
        //FIXME: Se si ha tempo e voglia facciamolo più legato al player che alla posizione.
        //P1 sta sempre a sinistra perché è sempre il primo che joina
        bool is_P1 = scoringPlayer_Id == 0;
        RpcScoreUpdate(is_P1, scoringPlayer_Score);

        //Ha vinto?
        if (scoringPlayer_Score < points_To_Win)
        {
            //FIXME: Per ora lo metto qua.
            OnRoundStarted?.Invoke();
            return;
        }

        //Get score difference
        int winner_Score = GameManager.current.Points_To_Win;
        GameManager.current.Scoreboard.TryGetValue(scoredPlayer_Id, out int loser_Score);
        int difference_Score = winner_Score - loser_Score;

        //Run Method on the winning client
        NetworkServer.connections.TryGetValue(scoringPlayer_Id, out NetworkConnectionToClient client_Winner);
        TargetMatchOver(client_Winner, true, difference_Score);

        //Run Method on the losing client
        NetworkServer.connections.TryGetValue(scoredPlayer_Id, out NetworkConnectionToClient client_Loser);
        TargetMatchOver(client_Loser, false, difference_Score);
    }

    //FIXME: Serve?
    /// <summary>
    /// Ricevi lo score del player dando il numero del player che cerchi, ritorna -1 se il player cercato non esiste.
    /// </summary>
    /// <param name="player_Id">L'index del player (player 1 = 0)</param>
    /// <returns></returns>
    private int GetPlayerScore(int player_Id) 
    {
        if (!NetworkServer.connections.TryGetValue(player_Id, out NetworkConnectionToClient client)) return -1;

        Scoreboard.TryGetValue(client.connectionId, out int player_Score);

        return player_Score;
    }
    
    [TargetRpc]
    private void TargetMatchOver(NetworkConnectionToClient target, bool is_Winner, int point_difference) => OnMatchOver?.Invoke(is_Winner, point_difference);

    [ClientRpc]
    private void RpcScoreUpdate(bool is_P1, int scire) => OnScoreUpdate?.Invoke(is_P1, scire);
}
