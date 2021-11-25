using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour
{

    private int countDown = 50;
    public Text text;

    void FixedUpdate()
    {
        text.text = (countDown/10).ToString();

        if (countDown == 0)
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        else 
        {
            countDown -= Mathf.RoundToInt(Time.timeSinceLevelLoad);
        }
    }
}
