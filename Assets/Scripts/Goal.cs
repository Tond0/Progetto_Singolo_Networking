using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class Goal : NetworkBehaviour
{
    //NOTA BENE: Questa porta sar� Owned dal client che dovr� farci goal! non da chi dovr� difendere questa porta!


    //Il player che dovr� difendere questa porta
    public static Action<NetworkConnectionToClient> OnGoal;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    [Server]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent<Pong_Ball>(out Pong_Ball ball)) return;

        //Diciamo che � stato fatto goal e passiamo chi la subito.
        OnGoal?.Invoke(connectionToClient);

        GameManager.OnRoundEnded?.Invoke();
        GameManager.OnRoundStarted?.Invoke();
    }
}
