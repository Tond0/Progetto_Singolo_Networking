using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class Goal : NetworkBehaviour
{
    //NOTA BENE: Questa porta sarà Owned dal client che dovrà farci goal! non da chi dovrà difendere questa porta!


    //Il player che dovrà difendere questa porta
    public static Action<NetworkConnectionToClient> OnGoal;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent<Ball>(out Ball ball)) return;

        if (isServer)
        {
            //Diciamo che è stato fatto goal e passiamo chi l'ha subito.
            OnGoal?.Invoke(connectionToClient);
        }
    }
}
