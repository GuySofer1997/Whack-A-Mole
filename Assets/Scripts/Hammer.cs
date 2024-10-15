using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        gameObject.SetActive(false); // Hide the hammer initially
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        // Move the hammer to the mouse position while it's active
        if (Input.GetMouseButton(0)) // Check for left mouse button press
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            targetPosition.z = 0f;
            transform.position = targetPosition; // Move the hammer to the mouse position
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false); // Disable the hammer
    }

    public void Show(Vector3 position)
    {
        transform.position = position; // Set the hammer position
        gameObject.SetActive(true); // Show the hammer
    }
}
