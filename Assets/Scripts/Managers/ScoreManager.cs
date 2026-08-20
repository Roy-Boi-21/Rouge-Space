using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Tooltip("The TMP text asset that will display the score.")]
    [SerializeField] private TMP_Text scoreText;

    [Tooltip("The text before the score.")]
    [SerializeField] private string scorePrefix;

    public static ScoreManager instance;
    private int score;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        UpdateScore();
    }

    public int GetScore()
    {
        return score;
    }

    public void UpdateScore()
    {
        scoreText.text = scorePrefix + score;
    }

    public void AddScore(int gainedScore)
    {
        score += gainedScore;
        UpdateScore();
    }
}
