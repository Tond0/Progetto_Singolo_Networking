using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Org.BouncyCastle.Asn1.Crmf;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sprite_renderer;
    
    [Header("Stats")]
    [SerializeField] private float movement_speed = .1f;
    private float movement_direction = 0;


    #region Input

    Controls controls;

    private void Awake()
    {
        controls = new();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    #endregion 

    private void Update()
    {
        if (!isLocalPlayer) return;

        movement_direction = controls.Gameplay.Move.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        Move(movement_direction);
    }

    #region Client

    [Command]
    private void Move(float direction)
    {
        //Movement
        Vector2 movement = new Vector2(0, direction) * movement_speed;
        rb.velocity = movement;
    }

    #endregion
}
