using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _hiScoreText;
    [SerializeField] private List<Image> _livesImages; // List of UI Image elements representing player lives

    [SerializeField] private GameObject _canvasButtonsContainer;

    private void Awake()
    {
        // Check if we should display the canvas buttons on mobile platforms
        var shouldPresentCanvasButtons = false;
        #if UNITY_ANDROID || UNITY_IOS
        shouldPresentCanvasButtons = true;
        #endif
        _canvasButtonsContainer.SetActive(shouldPresentCanvasButtons);
    }

    public void UpdateHiScore(int hiScore)
    {
        if (_hiScoreText != null)
        {
            _hiScoreText.text = hiScore.ToString();
        }
        else
        {
            Debug.LogError("High Score Text UI element is not assigned in the CanvasManager!");
        }
    }

     public void ShowStartButton()
    {
        _canvasButtonsContainer.gameObject.SetActive(true); // Show the start button when the game ends
    }
    
    public void UpdateCurrentScore(int currentScore)
    {
        if (_currentScoreText != null)
        {
            _currentScoreText.text = currentScore.ToString();
        }
        else
        {
            Debug.LogError("Current Score Text UI element is not assigned in the CanvasManager!");
        }
    }
    
    public void UpdateLives(int lives)
    {
        if (_livesImages != null && _livesImages.Count > 0)
        {
            for (var i = 0; i < _livesImages.Count; i++)
            {
                _livesImages[i].enabled = i < lives;
            }
        }
        else
        {
            Debug.LogError("Lives Image elements are not assigned or the list is empty in the CanvasManager!");
        }
    }
}
