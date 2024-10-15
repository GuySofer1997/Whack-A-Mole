using UnityEngine;
using System.Collections;
public class MoleOrBomb : MonoBehaviour
{
    private Animator _animator;
    private bool _isVisible = false;
    public bool IsVisible => _isVisible; // Property to expose animation state

    public bool IsMoleWithCasda { get; private set; } 
    private int _hitCount = 0; // Counter for hits
    private bool _isAnimating = false; // New field to track animation state

    public bool IsAnimating => _isAnimating; // Property to expose animation state

    public bool IsBomb { get; private set; }
    [SerializeField] private Collider2D _collider;

    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float visibleDuration = 5f; // You can change this in the inspector

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider =  GetComponent<Collider2D>();
        _rigidbody =  GetComponent<Rigidbody2D>();
    }

    public void Initialize(bool isBomb, bool isMoleWithCasda)
    {
    IsBomb = isBomb;
    IsMoleWithCasda = isMoleWithCasda; 
    _hitCount = 0; // Reset hit count 

    if (_animator != null)
    {
        if (isBomb)
        {
            _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("BombPrefab");
        }
        else if(isMoleWithCasda)
        {
          _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("MoleCasdaPrefab");

        } 
        else
        {
            _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("MolePrefab");

        }
        
        Show();

    }
   }

    public void Show()
    {
        if (_animator != null)
        {
             _isAnimating = true; // set animating state to true
            _animator.ResetTrigger("Hit"); // Reset Hit trigger
            _isVisible = true; 
            StartCoroutine(ShowForDuration()); // manage visibility duration
            _animator.SetTrigger("Show"); 
        }
    }

    private IEnumerator ShowForDuration()
    {
        yield return new WaitForSeconds(visibleDuration);
        // Hide to make the mole disappear after the visibleDuration
        Hide();
    }
    
    private IEnumerator HideAfterAnimation()
    {
        _collider.enabled = false; 
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length); 
        Hide();
        _collider.enabled = true; 
    }

    private void Hide()
    {
        _isVisible = false;
        gameObject.SetActive(false); 
        GameManager.Instance.HideMoleOrBomb(this); 
        _isAnimating = false; // Set animating state to false when hiding is complete
    }

    private void OnMouseDown()
    {
        OnHit(); 
    }

    public void OnHit()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver) // Check if the game is over
        {
            return; // Exit if the game is over
        }

        if (_isVisible && IsAnimating)
        {
            if(IsMoleWithCasda)
            {

               _hitCount++; 

            }

            HammerController.Instance.ShowHammer(transform.position); 

            if ((!IsMoleWithCasda) || (_hitCount == 2))
            {
                _animator.SetTrigger("Hit");
                StartCoroutine(HideAfterAnimation());
                GameManager.Instance.OnHammerCollision(this);
                _hitCount = 0; 
            }
        }
    }
}
