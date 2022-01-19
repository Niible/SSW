using System.Collections.Generic;
using UnityEngine;

public enum ArtifactMode
{
    None = -1,
    Rest = 0,
    Jump,
}

public class Hero : MonoBehaviour
{
    [Header("Variables")] [SerializeField] private float maxSpeed = 4.5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private int artifactModeIndex = 0;
    [SerializeField] private List<ArtifactMode> artifactModeList;
    [SerializeField] public PlayerController playerController;

    /**
     * -1 (left), 0 (not moving), 1 (right)
     */
    [SerializeField] private int currentHorizontalDirection = -1;

    [Header("Effects")] [SerializeField] private GameObject runStopDust;
    [SerializeField] private GameObject jumpDust;
    [SerializeField] private GameObject landingDust;

    public bool isMovementDisabled = false;
    private Animator _animator;
    private Rigidbody2D _body2d;
    private Sensor_Prototype _groundSensor;
    private AudioSource _audioSource;
    private AudioManager _audioManager;
    private MainUI _mainUIComponent;
    private SpriteRenderer _spriteRenderer;
    private bool _grounded = false;
    private bool _moving = false;
    private bool _isInMovementTransition = false;
    private int _facingDirection = 1;
    private float _disableMovementTimer = 0.0f;
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int AirSpeedY = Animator.StringToHash("AirSpeedY");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int AnimState = Animator.StringToHash("AnimState");

    // Use this for initialization
    private void Start()
    {
        var uiGameObject = TriggersUI.FindMainUIGameObject();
        _mainUIComponent = uiGameObject.transform.Find("MainUI").GetComponent<MainUI>();
        _mainUIComponent.RefreshArtifactList(this);
        _animator = GetComponent<Animator>();
        _body2d = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioManager = AudioManager.Instance;
        _groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var contactCount = collision.contactCount;
        for (var i = 0; i < contactCount; i++)
        {
            var contact = collision.GetContact(i);
            Debug.DrawRay(contact.point, contact.normal, Color.red, 25);

            if (contact.otherCollider.isTrigger) continue;
            HandleHorizontalDirectionChange(contact);
        }
    }

    private void HandleHorizontalDirectionChange(ContactPoint2D contact)
    {
        if (!_grounded) return;

        if (contact.normal == Vector2.left)
        {
            currentHorizontalDirection = -1;
        }
        else if (contact.normal == Vector2.right)
        {
            currentHorizontalDirection = 1;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Decrease timer that disables input movement. Used when attacking
        _disableMovementTimer -= Time.deltaTime;

        //Check if character just landed on the ground
        if (!_grounded && _groundSensor.State())
        {
            _grounded = true;
            _animator.SetBool(Grounded, _grounded);
        }

        //Check if character just started falling
        if (_grounded && !_groundSensor.State())
        {
            _grounded = false;
            _animator.SetBool(Grounded, _grounded);
        }

        HandleHorizontalMovement();
    }

    private void HandleHorizontalMovement()
    {
        var horizontalMovementDirection = 0.0f;

        if (_disableMovementTimer <= 0.0f)
            horizontalMovementDirection = currentHorizontalDirection;

        switch (horizontalMovementDirection)
        {
            // Swap direction of sprite depending on move direction
            case > 0:
                _spriteRenderer.flipX = false;
                _facingDirection = 1;
                break;
            case < 0:
                _spriteRenderer.flipX = true;
                _facingDirection = -1;
                break;
        }

        var newVelocity = 0f;
        // Check if current move input is greater than very small and the move direction is equal to the characters facing direction
        if (Mathf.Abs(horizontalMovementDirection) > Mathf.Epsilon &&
            (int) Mathf.Sign(horizontalMovementDirection) == _facingDirection && !isMovementDisabled &&
            // not in Rest mode
            (artifactModeList.Count == 0 || artifactModeList[artifactModeIndex] != ArtifactMode.Rest) && _grounded)
        {
            if (!_moving)
            {
                newVelocity = horizontalMovementDirection * maxSpeed * 0.5f;
                _moving = true;
                _isInMovementTransition = true;
            }
            else
            {
                if (_isInMovementTransition)
                {
                    _isInMovementTransition = false;
                }

                newVelocity = horizontalMovementDirection * maxSpeed;
            }
        }
        else
        {
            if (_moving)
            {
                newVelocity = horizontalMovementDirection * maxSpeed * 0.5f;
                _moving = false;
                _isInMovementTransition = true;
            }
            else if (_isInMovementTransition)
            {
                _isInMovementTransition = false;
            }
        }

        // Set movement
        _body2d.velocity = new Vector2(newVelocity, _body2d.velocity.y);

        // Set AirSpeed in animator
        _animator.SetFloat(AirSpeedY, _body2d.velocity.y);

        // Set Animation layer to hide sword
        _animator.SetLayerWeight(1, 1);

        // -- Handle Animations --

        // Run animation
        _animator.SetInteger(AnimState, _moving ? 1 : 0);
    }

    /**
     * On Single Press from Input System
     */
    private void OnSinglePress()
    {
        if (isMovementDisabled) return;

        HandleArtifactModeChange();
        _mainUIComponent.RefreshArtifactList(this);

        // //Jump
        // if (!_grounded) return;
        // _animator.SetTrigger(Jump);
        // _grounded = false;
        // _animator.SetBool(Grounded, _grounded);
        // _body2d.velocity = new Vector2(_body2d.velocity.x, jumpForce);
        // _groundSensor.Disable(0.2f);
    }

    private void OnLongPress()
    {
        Debug.Log("Longpress");
        HandleArtifactModeChange();
    }

    private void HandleArtifactModeChange()
    {
        if (artifactModeList.Count == 0) return;

        artifactModeIndex = (artifactModeIndex + 1) % artifactModeList.Count;
        _mainUIComponent.RefreshArtifactList(this);
    }

    // Function used to spawn a dust effect
    // All dust effects spawns on the floor
    // dustXoffset controls how far from the player the effects spawns.
    // Default dustXoffset is zero
    private void SpawnDustEffect(GameObject dust, float dustXOffset = 0)
    {
        if (dust == null) return;
        // Set dust spawn position
        var dustSpawnPosition = transform.position + new Vector3(dustXOffset * _facingDirection, 0.0f, 0.0f);
        var newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity) as GameObject;
        newDust.transform.parent = gameObject.transform;
        // Turn dust in correct X direction
        newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(_facingDirection, 1, 1);
    }

    // Animation Events
    // These functions are called inside the animation files
    private void AE_runStop()
    {
        _audioManager.PlayCustomSound("RunStop");
        // Spawn Dust
        const float dustXOffset = 0.6f;
        SpawnDustEffect(runStopDust, dustXOffset);
    }

    private void AE_footstep()
    {
        _audioManager.PlayCustomSound("Footstep");
    }

    private void AE_Jump()
    {
        _audioManager.PlayCustomSound("Jump");
        // Spawn Dust
        SpawnDustEffect(jumpDust);
    }

    private void AE_Landing()
    {
        _audioManager.PlayCustomSound("Landing");
        // Spawn Dust
        SpawnDustEffect(landingDust);
    }

    public int GetArtifactModeIndex()
    {
        return artifactModeIndex;
    }

    public List<ArtifactMode> GetArtifactModeList()
    {
        return artifactModeList;
    }

    public void SetArtifactModeIndex(int value)
    {
        artifactModeIndex = value;
        if (!_mainUIComponent) return;
        _mainUIComponent.RefreshArtifactList(this);
    }

    public void SetArtifactModeList(List<ArtifactMode> value)
    {
        artifactModeList = value;
        if (!_mainUIComponent) return;
        _mainUIComponent.RefreshArtifactList(this);
    }

    public bool IsImmobile()
    {
        return !_moving && !_isInMovementTransition;
    }

    public void SetLastRespawnPoint(RespawnPoint respawnPoint)
    {
        playerController.SetLastRespawnPoint(respawnPoint);
    }

    public void SetFacingDirection(int facingDirection)
    {
        _facingDirection = facingDirection;
    }
}