using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _beatTempo;
    [SerializeField] GameObject _referenceTile;
    [SerializeField] BeatManager _beatManager;
    [SerializeField] ParticleSystem _lavaParticleSystem;
    [SerializeField] ParticleSystem _completedLoopParticleSystem;
    [SerializeField] float _tempoIncrease = 1.2f;
    [SerializeField] Image _spaceBarImage;
    [SerializeField] float _steps = 1f;
    [SerializeField] float _jumpForgivenessTimer = 0.1f;

    Rigidbody2D _myRigidbody;
    bool _isJumping;
    bool _musicHasStarted;

    Animator _myAnimator;

    bool _isAlive = true;

    Vector2 _startPosition;
    GameManager _gameManager;

    bool _isInJumpingTimer = false;
    bool _hasJumped = false;


    void Awake()
    {
        _myRigidbody = GetComponent<Rigidbody2D>();
        _startPosition = gameObject.transform.position;
        _gameManager = FindObjectOfType<GameManager>();
        _myAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        _gameManager.UpdateLoop(_beatTempo);
        // StartCoroutine(StartCountdown(3 * (60 / beatTempo)));
        // StartCoroutine(StartMusic(3 * (60 / beatTempo)));
    }

    public void MovePlayerOnBeat()
    {
        StartCoroutine((60 / _beatTempo * _steps).Tweeng(
            (g) => _spaceBarImage.transform.localScale = g,
            new Vector3(_spaceBarImage.transform.localScale.x * 1.1f, _spaceBarImage.transform.localScale.y * 1.1f, _spaceBarImage.transform.localScale.z),
            _spaceBarImage.transform.localScale
        ));
        _myRigidbody.MovePosition(new Vector2(
            _myRigidbody.position.x + _referenceTile.GetComponent<Renderer>().bounds.size.x, 
            _myRigidbody.position.y));
    }

    public void ListenForJump()
    {
        StartCoroutine(WaitForJump());
    }

    IEnumerator WaitForJump()
    {
        _isInJumpingTimer = true;
        float timer = 0f;

        while (timer <= 0.3 && !_hasJumped)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _isInJumpingTimer = true;
        _hasJumped = false;
    }

    void OnJump(InputValue value)
    {
        if (!_isInJumpingTimer || !_isAlive || _isJumping) { return; }

        // TODO jump doesn't seem to clear pits properly in the faster tempos
        
        if (value.isPressed)
        {
            _hasJumped = true;
            Debug.Log("CORRECT TIMING");
            _myRigidbody.MovePosition(new Vector2(
                _myRigidbody.position.x + _referenceTile.GetComponent<Renderer>().bounds.size.x,
                _myRigidbody.position.y + _referenceTile.GetComponent<Renderer>().bounds.size.y + 0.1f
            ));
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        _isJumping = false;
    }

    void OnCollisionExit2D(Collision2D other)
    {
        _isJumping = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isAlive) { return; }

        if (_myRigidbody.IsTouchingLayers(LayerMask.GetMask("Hazard")))
        {
            _isAlive = false;
            // myAnimator.SetTrigger("Dying");
            // audioSource.Play();

            _lavaParticleSystem.Play();

            _gameManager.RestartGame();
        }
        else if (_myRigidbody.IsTouchingLayers(LayerMask.GetMask("Exit")))
        {
            _beatTempo *= _tempoIncrease;

            // play particle effect
            _completedLoopParticleSystem.Play();

            _myAnimator.speed *= _tempoIncrease;

            // TODO: play completed loop sound effect

            _beatManager.UpdateBPM(_tempoIncrease);

            // move player back to beginning of the level
            gameObject.transform.position = _startPosition;

            _myAnimator.speed *= _tempoIncrease;

            _gameManager.UpdateLoop(_beatTempo);
        }
    }

    IEnumerator StartCountdown(float secondsToWait)
    {
        yield return new WaitForSecondsRealtime(secondsToWait);
    }

    // IEnumerator StartMusic(float secondsToWait)
    // {
    //     yield return new WaitForSecondsRealtime(secondsToWait);
    //     music.Play();
    //     musicHasStarted = true;
    //     Debug.Log("movement starting");
    //     myRigidbody.MovePosition(new Vector2(myRigidbody.position.x + referenceTile.GetComponent<Renderer>().bounds.size.x, myRigidbody.position.y));
    // }
}
