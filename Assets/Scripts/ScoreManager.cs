using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentScore;

    public void IncreaseScore(int amount)
    {
        currentScore += amount;
        Debug.Log("Current score: " + currentScore);
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}