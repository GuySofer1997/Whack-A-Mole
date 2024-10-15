using UnityEngine;
using System.Collections; // Required for IEnumerator

public class HammerController : Singleton<HammerController>
{
    [Header("Hammer")]
    [SerializeField] private Hammer _hammerPrefab;
    private Hammer _hammer;

 private void Awake()
    {
        // Instantiate the hammer but keep it inactive
        _hammer = Instantiate(_hammerPrefab);
        _hammer.gameObject.SetActive(false); // Hide the hammer initially
    }

    public void ShowHammer(Vector3 position)
    {
        _hammer.Show(position); // Use the new Show method to position the hammer
        StartCoroutine(HideHammerAfterDelay(0.5f)); // Call the coroutine to hide it after 1 second
    }

    private IEnumerator HideHammerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        _hammer.Hide(); // Call the Hide method from the Hammer class
    }

    public void InitializeHammerLocation()
{
    if (_hammer != null)
    {
        // Adjust to a position where it won't overlap other objects initially
        _hammer.transform.position = new Vector3(-5, -5, 0); // Adjust the coordinates as needed
        Debug.Log("Hammer position initialized to: " + _hammer.transform.position);
    }
    else
    {
        Debug.LogError("Hammer instance is null!");
    }
}

    public void EndGame()
    {
        Debug.Log("Game Over");
    }
}
