using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchors : MonoBehaviour
{
    public GameObject anchorSound;
    public GameObject zapSound;
    public Player player;
    public Ennemies ennemy;
    private Touch touch;
    private bool firstWake = true;
    private float repulsiveWave = 0;
    public List<GameObject> repulseArea;

    public GameObject ennemyHitSound;
    public ParticleSystem explosion;


    private void Awake()
    {
        this.enabled = false;
        repulseArea = new List<GameObject>();
    }

    private void RepulseWave()
    {
        foreach (GameObject item in repulseArea) {
            if(item != null) {
                if (item.tag == "Ennemy") {
                    Rigidbody2D otherRB = GetComponent<Rigidbody2D>();
                    item.GetComponent<Ennemies>().RepulseEnnemy(otherRB);
                }
                else {
                    Rigidbody2D itemRB = item.GetComponent<Rigidbody2D>();
                    itemRB.velocity += ((new Vector2(Random.Range(-10, 10), Random.Range(-10, 10))) * Time.fixedDeltaTime) * 120f;
                    Instantiate(zapSound, this.transform);
                }
            }
        }
        repulsiveWave = 0;
        StartCoroutine(WaitForReturn());
    }

    IEnumerator WaitForReturn()
    {
        yield return new WaitForSecondsRealtime(.5f);
        
        GetComponent<SpriteRenderer>().color = new Color(0, 255, 255);
    }

    private void FixedUpdate()
    {
        if(repulsiveWave >= 2.9)
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
        else
            GetComponent<SpriteRenderer>().color = new Color(0, 1, 1 - (repulsiveWave / 2), 1);
        if ( repulsiveWave >= 3 && repulseArea.Contains(player.gameObject)) {         
            RepulseWave();
            enabled = false;
            repulseArea = new List<GameObject>();
        }       
    }

    IEnumerator ScalePlayerSpeed()
    {
        yield return new WaitForSecondsRealtime(1f);
        player.speed = 75f;
    }

    private void OnEnable()
    {
        GameManager.anchorsPool.Add(this);
        player.speed = 120f;
        StartCoroutine(ScalePlayerSpeed());
    }

    private void OnDisable()
    {
        if (firstWake) {
            firstWake = false;
        }
        else {
            MoveAnchors();
            GameManager.anchorsPool.Remove(this);
        }
    }

    void MoveAnchors()
    {
        Vector3 randomMove = new Vector3(Random.Range(-6.0f, 6.0f), Random.Range(-6.0f, 6.0f), 0.0f);
        randomMove += transform.position;
        float newX, newY;
        if (transform.position.x <= 0) {
            if(transform.position.y <= 0) {
                newX = Mathf.Clamp(randomMove.x, -9.0f, -20.0f);
                newY = Mathf.Clamp(randomMove.y, -4.0f, -10.0f);
            }
            else {
                newX = Mathf.Clamp(randomMove.x, -9.0f, -20.0f);
                newY = Mathf.Clamp(randomMove.y, 4.0f, 10.0f);
            }
        }
        else {
            if (transform.position.y <= 0) {
                newX = Mathf.Clamp(randomMove.x, 9.0f, 20.0f);
                newY = Mathf.Clamp(randomMove.y, -4.0f, -10.0f);
            }
            else {
                newX = Mathf.Clamp(randomMove.x, 9.0f, 20.0f);
                newY = Mathf.Clamp(randomMove.y, 4.0f, 10.0f);
            }
        }
        Vector3 newPos = new Vector3(newX, newY, 0.0f);
        transform.position = newPos;
    }

    private void OnMouseDown()
    {
        this.enabled = true;
        Instantiate(anchorSound, this.transform);
    }

    private void OnMouseDrag()
    {
        if (repulseArea.Count >= 1)
            repulsiveWave += 1 * Time.deltaTime;
    }

    private void OnMouseUp()
    {
        this.enabled = false;
    }
}
