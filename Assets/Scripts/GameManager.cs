using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // Add this line to use LINQ methods like Any


public class GameManager : Singleton<GameManager>
{
    private const string HiScoreKey = "HiScore";

    [Header("Game Loop")]
    [SerializeField] private int _initialLives = 3;
    [SerializeField] private Button startButton; 
    [SerializeField] private MoleOrBomb moleOrBombPrefab; 
    [SerializeField] private List<MoleSpawnLocation> moleSpawnLocations; 
    [SerializeField] private TextMeshProUGUI timerText; // Timer UI
     public bool IsGameOver => isGameOver; 
    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitMoleSound; 
    [SerializeField] private AudioClip hitBombSound; 

    [Header("Spawn Control")]
    private int _moleSpawnCount ; 
    private int _bombSpawnThreshold; 
    private int _moleWithCasdaThreshold;
    private List<MoleOrBomb> _activeMoleOrBombs = new List<MoleOrBomb>();
    private List<MoleSpawnLocation> _availableSpawnLocations = new List<MoleSpawnLocation>(); // Available spawn locations

    private int _currentLives;
    private int _currentScore;
    private int _hiScore;
    private bool isGameOver = true;
    private bool _isTimerRunning = false;

    [Header("Timer")]
    [SerializeField] private float _gameDuration = 60f; // Game duration
    private float _timeRemaining;
    private float _spawnInterval = 2.5f; // Base spawn interval
    private float _difficultyIncreaseInterval = 15f; // Time interval for increasing difficulty
    private void Start()
    {
        if (moleSpawnLocations.Count == 0)
        {
            moleSpawnLocations.AddRange(FindObjectsOfType<MoleSpawnLocation>());
        }
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        if (moleOrBombPrefab == null)
        {
            return;
        }

        startButton.onClick.AddListener(OnStartButtonClicked);
        startButton.gameObject.SetActive(false); // Hide the start button initially
        StartGame(); // Start the game initially
    }

    public void StartGame()
    {
        _hiScore = PlayerPrefs.GetInt(HiScoreKey, 0);
        _currentLives = _initialLives;
        _currentScore = 0;
        _timeRemaining = _gameDuration;
        _isTimerRunning = true;
        isGameOver = false;
        _moleSpawnCount = 0;
        _bombSpawnThreshold = 4;
        _moleWithCasdaThreshold = 3;
        UpdateTimerUI();
        CanvasManager.Instance.UpdateLives(_currentLives);
        CanvasManager.Instance.UpdateCurrentScore(_currentScore);
        CanvasManager.Instance.UpdateHiScore(_hiScore);
        // hide the game button
        startButton.gameObject.SetActive(false);
        // Reset available spawn locations
        ResetSpawnLocations();
        // Stop any existing spawning coroutines before starting new ones
        StopAllCoroutines();
        StartCoroutine(SpawnObjectsRandomly());
        StartCoroutine(IncreaseDifficulty());

    }

    private void Update()
    {
        if (isGameOver)
            return; // Do nothing if the game is over

        if (_isTimerRunning)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0) // timer end 
            {
                EndGame(); 
            }

            UpdateTimerUI(); 
        }
    }

private IEnumerator SpawnObjectsRandomly()
{

    while (!IsGameOver) // Spawn while the game is not over
    {

        // Check if we have available spawn locations
        if (moleSpawnLocations.Count > 0)
        {
           yield return new WaitForSeconds(_spawnInterval); // Wait between spawns

            _moleSpawnCount++;
            bool spawnAsBomb = _moleSpawnCount % _bombSpawnThreshold == 0; // Every fourth MoleOrBomb is a bomb
            bool isMoleWithCasda = false;

            if(!spawnAsBomb) // if not bomb
            {
                isMoleWithCasda = _moleSpawnCount % _moleWithCasdaThreshold == 0; // Every third mole is a special mole

            }

            int randomIndex = Random.Range(0, moleSpawnLocations.Count);
            
            MoleSpawnLocation selectedLocation = moleSpawnLocations[randomIndex];
        
           // Check if position  avilable
            if (!_activeMoleOrBombs.Any(m => m.transform.position == selectedLocation.transform.position && m.IsVisible)) 
            {
                SpawnMoleOrBomb(spawnAsBomb, isMoleWithCasda, selectedLocation);
            }
        }
    }
}

 private IEnumerator IncreaseDifficulty()
    {
        
        while (!IsGameOver)
        {
            yield return new WaitForSeconds(_difficultyIncreaseInterval); 
            _spawnInterval *= 0.7f;
            if(_spawnInterval < 1f)
            {
                _spawnInterval = 1f;
            }

            if (_timeRemaining <= 20)
            {
                _bombSpawnThreshold = 2; // Every second MoleOrBomb object is a bomb
                _moleWithCasdaThreshold = 1;// Every second mole is moleWithCasda


            }
              else if (_timeRemaining <= 30)
            {
                _moleWithCasdaThreshold = 2;// Every second mole is moleWithCasda

            }
            else if (_timeRemaining <= 40)
            {
                _bombSpawnThreshold = 3; 

            }
        }
    }

    private void SpawnMoleOrBomb(bool isBomb, bool isMoleWithCasda, MoleSpawnLocation spawnLocation)
    {
        if (_activeMoleOrBombs.Exists(m => m.transform.position == spawnLocation.transform.position))
        {
            return; // already exits obj in the position
        }

        // Instantiate the MoleOrBomb prefab at the chosen location's position
        MoleOrBomb newObject = Instantiate(moleOrBombPrefab, spawnLocation.transform.position, Quaternion.identity);

        if (newObject == null)
        {
            return;
        }

        // Initialize a a mole or bomb it can also be MoleWithCasda
        newObject.Initialize(isBomb, isMoleWithCasda); 
        _activeMoleOrBombs.Add(newObject); 
        _availableSpawnLocations.Remove(spawnLocation); //the spawnlocation is now non avilable
    }

    private void ClearAllObjects()
    {
        foreach (var moleOrBomb in _activeMoleOrBombs)
        {
            if (moleOrBomb != null)
            {
                Destroy(moleOrBomb.gameObject); 
            }
        }
        _activeMoleOrBombs.Clear();
    }

    private void EndGame()
    {
        isGameOver = true;
        StopAllCoroutines(); // Stop spawning
        _isTimerRunning = false; 
         _timeRemaining = 0; 
        UpdateTimerUI(); 
        CanvasManager.Instance.ShowStartButton();
        ClearAllObjects();
        startButton.gameObject.SetActive(true); 

    }

    private void OnStartButtonClicked()
    {
        if (isGameOver)
        {
            StartGame(); // Restart the game
        }
    }

    private void UpdateTimerUI() // timer in canvas
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(_timeRemaining / 60);
            int seconds = Mathf.FloorToInt(_timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void OnHammerCollision(MoleOrBomb moleOrBomb)
    {
        if (moleOrBomb == null) return; 

        if (moleOrBomb.IsBomb)
        {
            _currentLives--;
            CanvasManager.Instance.UpdateLives(_currentLives);
            _currentScore -= 50;
            audioSource.PlayOneShot(hitBombSound); // Play the hit sound
           
        }
        else 
        {
            if(moleOrBomb.IsMoleWithCasda)
            {
            _currentScore += 200;
            }

            else
            {
               _currentScore += 100;
            }

            audioSource.PlayOneShot(hitMoleSound); // Play the hit sound

            if (_currentScore > _hiScore)
            {
                _hiScore = _currentScore;
                CanvasManager.Instance.UpdateHiScore(_hiScore);
                PlayerPrefs.SetInt(HiScoreKey, _hiScore);
            }
        }

        CanvasManager.Instance.UpdateCurrentScore(_currentScore);
        HideMoleOrBomb(moleOrBomb);
         if (_currentLives <= 0)
            {
                EndGame();
               
            }
    }


    public void HideMoleOrBomb(MoleOrBomb moleOrBomb)
    {
        if (moleOrBomb == null) return; 
        if (_activeMoleOrBombs.Contains(moleOrBomb))
        {
            _activeMoleOrBombs.Remove(moleOrBomb);
            Destroy(moleOrBomb.gameObject, 1f); // Adjust delay if needed
        }
    }

    private void ResetSpawnLocations()
    {
        _availableSpawnLocations.Clear();
        _availableSpawnLocations.AddRange(moleSpawnLocations); // Re-add them to ensure they are all available
    }
}
