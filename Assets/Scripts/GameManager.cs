using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static List<Anchors> anchorsPool;
    public static List<Ennemies> ennemiesPool;

    public static bool gameIsPaused = false;
    public GameObject InGame, Pause;
    public ScoreDisplayer displayer;
    public ScoreHolder holder;

    public static int deadEnnemies = 0;
    public Text ennemiesText;
    public Text scoreText;

    private float timeStart;
    private float timeEnd;

    private int spawnPos = 0;
    private Vector2[] positions;
    public GameObject startButton;

    bool moveR = true;
    bool moveUp = true;

    public GameObject Shield;
    public GameObject bouncer;
    public GameObject[] bouncers;
    public GameObject ennemySpawned;
    public GameObject[] proxies;
    public Player player;
    public Ennemies ennemy;
    private Rigidbody2D playerRB;
    private LineRenderer lr;
    public bool shieldactive;

    private float timeToWait = 3.0f;
    private IEnumerator coroutine;
    public Image flash;
    private float move = 3.0f;

    private int maxEnnemies = 12;
    private float timeBetweenEnnemies = 5f;

    private void Awake()
    {
        anchorsPool = new List<Anchors>();
        ennemiesPool = new List<Ennemies>();
        positions = new Vector2[4];
        timeStart = Time.time;
        shieldactive = true;
    }

    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        lr = player.GetComponent<LineRenderer>();
        lr.enabled = false;
        flash.enabled = false;
        coroutine = StopFlash();
        RandomSpawn();
        FirstSpawn();
        StartCoroutine(WaitForSpawnShield());
        timeToWait = 15.0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void FirstSpawn()
    {
        for (int i = 0; i < positions.Length; i++) {
            Instantiate(ennemySpawned, positions[i], Quaternion.identity, Instantiate(ennemy, positions[i], Quaternion.identity, this.transform).transform);
        }
    }


    private IEnumerator StopFlash()
    {
        yield return new WaitForSeconds(0.07f);
        flash.enabled = false;
    }

    private void Update()
    {
        timeEnd = Time.time;
        ScoreHolder.finalScore = (int)(timeEnd - timeStart) * 10 * (deadEnnemies + 1);
        if (player.life <= 0) {
            Destroy(GameObject.Find("AudioManager"));
            
            SceneManager.LoadScene("DeathScene", LoadSceneMode.Single);
        }
        if (timeBetweenEnnemies <= 0 && maxEnnemies <= 12) {
            EnnemySpawn();
            timeBetweenEnnemies = 2.5f;
        }
        timeBetweenEnnemies -= Time.deltaTime;
        if (player.hit) {
            flash.enabled = true;
            StartCoroutine(coroutine);
            player.hit = false;
            coroutine = StopFlash();
        }

        ennemiesText.text = deadEnnemies.ToString();
        scoreText.text = ScoreHolder.finalScore.ToString();
        if (!shieldactive) {
            StartCoroutine(WaitForSpawnShield());
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused) {
                Resume();
            }
            else {
                PauseGame();
            }
        }
    }

    private void ChangeAlpha()
    {
        proxies[spawnPos].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, -1/3f + 1/timeBetweenEnnemies);       
    }

    private void MoveBouncer(GameObject bouncy)
    {
        if (bouncy.transform.position.x >= 24)
            moveR = false;
        else if(bouncy.transform.position.x <= -24)
            moveR = true;
        if (bouncy.transform.position.y >= 12)
            moveUp = false;
        else if (bouncy.transform.position.y <= -12)
            moveUp = true;

        if (bouncy.transform.rotation.z == 0) {
            // Bouge sur l'axe y entre -12 et +12
            if (moveR) {
                bouncy.transform.position += new Vector3(move * Time.deltaTime, 0, 0);
                move = -move;
            }
            else {
                bouncy.transform.position += new Vector3(-move * Time.deltaTime, 0, 0);
                //move = -move;
            }         
        }
        else {
            // Bouge sur l'axe x entre -23 et +23
            if (moveUp) {
                bouncy.transform.position += new Vector3(0, move * Time.deltaTime, 0);
                move = -move;
            }
            else {
                bouncy.transform.position += new Vector3(0, -move * Time.deltaTime, 0);
                //move = -move;
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        InGame.SetActive(true);
        Pause.SetActive(false);
        gameIsPaused = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        InGame.SetActive(false);
        Pause.SetActive(true);
        gameIsPaused = true;
    }

    public void BeginGame()
    {
        SceneManager.LoadScene("countDown", LoadSceneMode.Single);
    }

    public void Reload()
    {
        SceneManager.LoadScene("countDown", LoadSceneMode.Single);
    }

    void FixedUpdate()
    {
        foreach (Ennemies ennemy in ennemiesPool) {
            ennemy.HuntPlayer();
        }

        foreach (GameObject bouncy in bouncers) {
            MoveBouncer(bouncy);
        }
        ChangeAlpha();
        if (anchorsPool.Count != 0) {
            player.PlayerMovement(anchorsPool[0]);
            DrawLine();
        }
        else {
            lr.enabled = false;
            player.speed = 30.0f;
        }
    }

    IEnumerator WaitForSpawnShield()
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        GenerateShield();
    }

    private void GenerateShield()
    {
        shieldactive = true;
        Vector2 ranPos = new Vector2 (positions[Mathf.RoundToInt(Random.Range(0.0f, 3.0f))].x, positions[Mathf.RoundToInt(Random.Range(0.0f, 3.0f))].y);
        Instantiate(Shield,ranPos, Quaternion.identity, transform);
    }

    private void DrawLine()
    {
        lr.enabled = true;
        Vector3 []points = new Vector3[2];
        points[0] = new Vector3 (player.transform.position.x, player.transform.position.y, 0.0f);
        points[1] = new Vector3 (anchorsPool[0].transform.position.x, anchorsPool[0].transform.position.y, 0.0f);
        lr.SetPositions(points);
    }

    private void EnnemySpawn()
    {
        Instantiate(ennemySpawned, positions[spawnPos], Quaternion.identity, Instantiate(ennemy, positions[spawnPos], Quaternion.identity, this.transform).transform);
        proxies[spawnPos].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0 ,0);
        spawnPos++;
        if (spawnPos >= 4)
            spawnPos = 0;
    }

    private void RandomSpawn()
    {
        positions[0] = new Vector2(11f, 6.5f);
        positions[1] = new Vector2(10f,-6f);
        positions[2] = new Vector2(-12.5f, 4.75f);
        positions[3] = new Vector2(-13.5f, -7.75f);
    }

}
