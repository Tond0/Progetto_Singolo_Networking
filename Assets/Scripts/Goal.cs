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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent<Ball>(out Ball ball)) return;

        if (isServer)
        {
            //Diciamo che � stato fatto goal e passiamo chi l'ha subito.
            OnGoal?.Invoke(connectionToClient);
        }
    }
}
