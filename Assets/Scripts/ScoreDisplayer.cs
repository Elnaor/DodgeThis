using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    public Text score;
    public ScoreHolder sh;

    void Start()
    {
        score.text = "Your score is " + ScoreHolder.finalScore;
    }
}
