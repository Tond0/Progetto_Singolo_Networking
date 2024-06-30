using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public static Action OnAllPlayersReady;
    public static Action<NetworkConnectionToClient> OnPlayerReady;

    [Space(5)]
    [Header("Goals")]
    [SerializeField] private GameObject prefab_goal;
    [SerializeField, Tooltip("Dove spawna la porta che dovrà difendere P1?")] private Transform spawn_P1_Goal;
    [SerializeField, Tooltip("Dove spawna la porta che dovrà difendere P2?")] private Transform spawn_P2_Goal;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Vector3 pos = Vector3.zero;

        switch(numPlayers)
        {
            case 0:
                Debug.LogError("Bruh");
                break;

            case 1:
                pos = spawn_P2_Goal.transform.position;
                break;

            case 2:
                pos = spawn_P1_Goal.transform.position;
                break;
        }

        GameObject goal = Instantiate(prefab_goal, pos, prefab_goal.transform.rotation);
        NetworkServer.Spawn(goal, conn);

        OnPlayerReady?.Invoke(conn);

        //FIXME: un modo migliore? siccome già me lo chiedo sopra così che non debba chiederlo due volte?
        if(numPlayers == 2)
            OnAllPlayersReady?.Invoke();
    }
}
