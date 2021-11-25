using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreHolder : MonoBehaviour
{
    public static int finalScore;

    private void Awake()
    {
        finalScore = 0;
        DontDestroyOnLoad(this);
    }
}
