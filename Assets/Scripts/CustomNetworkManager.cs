using JetBrains.Annotations;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkRoomManager
{
    #region Events
    public static Action OnAllPlayersReady;
    public static Action<NetworkConnectionToClient> OnPlayerReady;
    
    public static Action OnClientConnected;
    public static Action OnClientDisconnected;
    #endregion

    //FIXME: Se ci mettessi "new" cambierebbe qualcosa?
    public static CustomNetworkManager singleton { get; internal set; }

    private void Awake()
    {
        base.Awake();

        if (singleton == null)
            singleton = this;
        else
            NetworkServer.Destroy(gameObject);
    }

    private int[] players_Id = new int[2];
    public int[] Players_ID {  get { return players_Id; } }

    [Space(5)]
    [Header("Goals")]
    [SerializeField] private GameObject prefab_goal;

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    //NumPlayers non funziona siccome sono già addati in precedenza e quando vengono aggiunti alla scena Game il counter è già a 2
    int gamePlayer_Spawned = 0;
    [Server]
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        gamePlayer_Spawned++;

        //FIXME: Da fare da un'altra parte non qua (tipo quando cambia la scena)
        SpawnGoal(conn, gamePlayer_Spawned);

        //FIXME: Serve?
        players_Id[numPlayers - 1] = conn.connectionId;

        OnPlayerReady?.Invoke(conn);

        //FIXME: un modo migliore? siccome già me lo chiedo sopra così che non debba chiederlo due volte?
        if (numPlayers == 2)
            OnAllPlayersReady?.Invoke();

        return true;
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        //Se stiamo tornando alla room...
        if (RoomScene != newSceneName) return;

        gamePlayer_Spawned = 0;
    }

    private void SpawnGoal(NetworkConnectionToClient conn, int player_index)
    {
        Vector3 pos = Vector3.zero;
        switch(player_index)
        {
            case 1:
                pos.x = -17.5f;
                break;

            case 2:
                pos.x = 17.5f;
                break;
        }


        GameObject goal = Instantiate(prefab_goal, pos, prefab_goal.transform.rotation);
        NetworkServer.Spawn(goal, conn);
    }
}
