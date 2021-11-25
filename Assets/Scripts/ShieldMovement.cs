using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Rigidbody2D rbToRepel;
    private Transform playerTM, selfTM;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        rbToRepel = player.GetComponent<Rigidbody2D>();
        playerTM = player.GetComponent<Transform>();
        selfTM = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(playerTM.position.x - selfTM.position.x) <= 1.5f || Mathf.Abs(playerTM.position.y - selfTM.position.y) <= 1.5f) {
            MoveTheShield();
        }
    }

    void MoveTheShield()
    {
        rb.AddForce((rbToRepel.position + rb.position) * 20f * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) // Marche pas, le shield se barre :-/ 
    {
        if(collision.name == "EdgeCollider") {
            Rigidbody2D otherRB = collision.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce((otherRB.position - rb.position) * Time.fixedDeltaTime * 305f);
        }
    }
}
