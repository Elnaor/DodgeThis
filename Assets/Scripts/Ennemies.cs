using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemies : MonoBehaviour
{

    public ParticleSystem explosion;
    public Player player;
    public GameObject hitSoundObject;
    private Rigidbody2D rb;
    private Vector2 distance = new Vector2(0,0);
    public Vector2 velocity = new Vector2(0, 0);

    private Vector2 acceleration;
    private float friction = 0.98f;
    private float dispertion = 1f;
    private float desengagement = 150f;
    private Rigidbody2D playerRB;
    private float x_sign;
    private float y_sign;

    public Anchors anchor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        playerRB = player.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        distance = rb.position;
    }

    private void FixedUpdate()
    {
        x_sign = Mathf.Sign(playerRB.position.x - rb.position.x);
        y_sign = Mathf.Sign(playerRB.position.y - rb.position.y);
        if (dispertion <= 2f) {
            dispertion += .05f * Time.deltaTime; 
        }
    }

    public void RepulseEnnemy(Rigidbody2D other)
    {
        velocity += ((rb.position - other.position) * Time.fixedDeltaTime) * desengagement;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ennemy" || other.tag == "Anchor") {
            RepulseEnnemy(other.GetComponent<Rigidbody2D>());
        }
        else if (other.name == "EdgeCollider") {
            Instantiate(hitSoundObject);
            Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public Vector2 SetAcceleration()
    {
        acceleration = new Vector2(Mathf.Pow(Mathf.Abs(playerRB.position.x - rb.position.x), dispertion) * x_sign * Time.fixedDeltaTime, 
            Mathf.Pow(Mathf.Abs(playerRB.position.y - rb.position.y), dispertion) * y_sign * Time.fixedDeltaTime);
        return acceleration;
    }

    public Vector2 SetVelocity()
    {
        velocity += SetAcceleration()/5f;
        velocity *= this.friction;
        return velocity;
    }

    public void HuntPlayer()
    {
        distance += SetVelocity()/20f;
        rb.MovePosition(distance);
    }

    private void OnEnable()
    {

        GameManager.ennemiesPool.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.ennemiesPool.Remove(this);
        GameManager.deadEnnemies++;
    }

}
