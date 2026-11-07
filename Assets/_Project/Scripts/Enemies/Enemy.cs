using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Health))]
public class Enemy : MonoBehaviour
{
    protected Health health;
    protected Transform player;
    protected Rigidbody2D rb;
    protected EnemyHit hit;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        health = GetComponent<Health>();
        hit = GetComponent<EnemyHit>();
    }

    protected virtual void Start()
    {

        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

}
