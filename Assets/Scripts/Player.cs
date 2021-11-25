using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public ParticleSystem explosion;
    public GameObject ennemyHitSound;
    public GameObject hitPlayerSound;
    public GameObject hitSoundObject;
    public GameObject bouncerSound;
    public GameObject ShieldAura;
    public GameManager gm;
    public Anchors anchor;

    private Rigidbody2D rb;
    public float speed;
    public int life = 4;
    public Image[] energies;
    public bool hit = false;
    public int shieldStack = 0;
    private Vector2 modVelocity;
    private bool charged;
    private bool shieldEngaged = false;
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        charged = false;
        GetComponent<SpriteRenderer>().color = new Color(255, 0, 255);
    }

    private void FixedUpdate()
    {

        CheckForSpeedLimit();
    }

    private void CheckForSpeedLimit()
    {
        if (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) >= 30) {
            charged = true;
            GetComponent<SpriteRenderer>().color = new Color(255, 215, 4);
            shieldEngaged = true;
        }           
        if (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) < 30) {
            if(shieldEngaged)
                StartCoroutine(SpeedShield());
        }       
    }

    private void Update()
    {
        for (int i = 0; i < energies.Length; i++) {
            if (i < life)
                energies[i].enabled = true;
            else
                energies[i].enabled = false;
        }
    }

    public void PlayerMovement(Anchors anchor)
    {
        Rigidbody2D rbToAttract = anchor.GetComponent<Rigidbody2D>();
        modVelocity = rb.velocity;
        modVelocity.x = Mathf.Clamp(modVelocity.x, -25, 25);
        modVelocity.y = Mathf.Clamp(modVelocity.y, -25, 25);
        rb.velocity = modVelocity;
        rb.AddForce(((rbToAttract.position - rb.position)+new Vector2(1,1)) * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Bouncer") {
            Rigidbody2D rbToAttract = other.GetComponent<Rigidbody2D>();
            if(rb.velocity != Vector2.zero) {
                modVelocity = rb.velocity;
                modVelocity.x = Mathf.Clamp(modVelocity.x, -45, 45);
                modVelocity.y = Mathf.Clamp(modVelocity.y, -45, 45);
                rb.velocity = modVelocity;
                rb.AddForce((rbToAttract.position - rb.position * 3) * speed * 2f * Time.fixedDeltaTime, ForceMode2D.Impulse);
                Instantiate(bouncerSound, this.transform);
            }
        }
        else if (other.name == "EdgeCollider") {
            Rigidbody2D rbToAttract = other.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            Instantiate(hitPlayerSound, this.transform);
            charged = false;
            GetComponent<SpriteRenderer>().color = new Color(255, 0, 255);
        }
        else if (other.tag == "Ennemy") {
            if (shieldStack > 0 || charged) {
                Instantiate(ennemyHitSound, this.transform);
                Instantiate(explosion, other.transform.position, Quaternion.identity, this.transform);
                Destroy(other.gameObject);
                shieldStack--;
                if(shieldStack == 0 && charged) { // Ici, si on fonce dans un paquet d'ennemis, on en tue un et on mange les autres :-/ 
                    Destroy(transform.Find("ShieldAura(Clone)").gameObject);
                    charged = false;
                }
            }
            else {
                hit = true;
                Instantiate(hitSoundObject);
                Instantiate(explosion, other.transform.position, Quaternion.identity);
                life -= 1;
                Destroy(other.gameObject);
            }

        }
        else if (other.tag == "Shield") {
            gm.shieldactive = false;
            Instantiate(ShieldAura, transform.position,Quaternion.identity, transform);
            shieldStack++;
            Destroy(other.gameObject);
        }
    }

    IEnumerator SpeedShield()
    {
        shieldEngaged = !shieldEngaged;
        yield return new WaitForSecondsRealtime(3);
        charged = false;
        GetComponent<SpriteRenderer>().color = new Color (255, 0, 255);
    }
}
