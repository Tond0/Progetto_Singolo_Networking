using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(Rigidbody2D))]
public class Ball : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [Header("Settings")]
    [SerializeField, Tooltip("How MORE fast does it get each time a player touch it?")] private float speed_boost = 5;
    [SerializeField, Tooltip("The first push force multiplaier")] private float intialPush;
    
    private float min_max_speed = 10;

    [SerializeField] private Transform spawn;

    private void OnEnable()
    {
        GameManager.OnRoundStarted += InitialKick;
        GameManager.OnRoundEnded += ResetBall;
    }

    private void OnDisable()
    {
        GameManager.OnRoundStarted -= InitialKick;
        GameManager.OnRoundEnded -= ResetBall;
    }

    [Server]
    private void Start()
    {
        min_max_speed = intialPush;
    }

    [Server]
    private void FixedUpdate()
    {
        ApplyMinSpeed();
    }

    [Server]
    private void ApplyMinSpeed()
    {
        float currentSpeed = rb.velocity.magnitude;
        Vector2 normalizedVelocity = rb.velocity.normalized;

        if (currentSpeed > min_max_speed)
            rb.velocity = normalizedVelocity * min_max_speed;
        else if (currentSpeed < min_max_speed)
            rb.velocity = normalizedVelocity * min_max_speed;
    }

    [Server]
    private void ResetBall()
    {
        rb.velocity = Vector2.zero;
        transform.position = spawn.position;
    }

    [Server]
    private void InitialKick()
    {
        Vector2 randomDir;

        float random_x = Random.Range(-1f, 1f);
        float random_y = Random.Range(-.4f, .4f);

        randomDir = new(random_x, random_y);
        randomDir.Normalize();

        Vector2 push = randomDir * intialPush;

        rb.AddForce(push, ForceMode2D.Impulse);
    }

    [Server]

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent<Player>(out Player collided_player)) return;

        min_max_speed += speed_boost;
    }
}
