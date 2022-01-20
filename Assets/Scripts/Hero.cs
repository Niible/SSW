using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArtifactMode
{
    None = -1,
    Rest = 0,
    Jump,
    Weapon,
}

public class Hero : MonoBehaviour
{
    [Header("Variables")] [SerializeField] private float maxSpeed = 4.5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float jumpSpeed = 4.5f;
    [SerializeField] private float passiveJumpForce = 3.75f;
    [SerializeField] private float passiveJumpSpeed = 2f;
    [SerializeField] private int artifactModeIndex = 0;
    [SerializeField] private List<ArtifactMode> artifactModeList;
    [SerializeField] public PlayerController playerController;
    [SerializeField] private float secondsToWaitBeforeJump = 0.2f;
    [SerializeField] private float secondsToWaitBeforeFire = 0.2f;
    [SerializeField] private float maxPassiveJumpDistance = 1f;
    [SerializeField] private float minCliffDistanceToTriggerPassiveJump = 0.5f;
    [SerializeField] private float secondsBetweenPassiveProjectiles = 1.5f;
    [SerializeField] private GameObject projectilePrefab;

    /**
     * -1 (left), 0 (not moving), 1 (right)
     */
    [SerializeField] private int currentHorizontalDirection = -1;

    [Header("Effects")] [SerializeField] private GameObject runStopDust;
    [SerializeField] private GameObject jumpDust;
    [SerializeField] private GameObject landingDust;

    public bool isMovementDisabled = false;
    private bool _shouldJump = false;
    private bool _isJumping = false;
    private bool _shouldFire = false;
    private bool _isFiring = false;
    private Animator _animator;
    private Rigidbody2D _body2d;
    private CapsuleCollider2D _capsuleCollider2D;
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
    private float _secondsToWaitBeforeNextPassiveProjectile = 0.0f;

    // Use this for initialization
    private void Start()
    {
        var uiGameObject = TriggersUI.FindMainUIGameObject();
        _mainUIComponent = uiGameObject.transform.Find("MainUI").GetComponent<MainUI>();
        _mainUIComponent.RefreshArtifactList(this);
        _animator = GetComponent<Animator>();
        _body2d = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioManager = AudioManager.Instance;
        var groundSensor = transform.Find("GroundSensor");
        _groundSensor = groundSensor.GetComponent<Sensor_Prototype>();
    }

    private void HandleHorizontalDirectionChange(RaycastHit2D contact)
    {
        if (!_grounded) return;

        // allows +0.5 -0.5 for wall angle
        // may have to be adjusted later
        var roundedContactNormalX = Mathf.RoundToInt(contact.normal.x);
        if (roundedContactNormalX == Mathf.RoundToInt(Vector2.left.x))
        {
            currentHorizontalDirection = -1;
        }
        else if (roundedContactNormalX == Mathf.RoundToInt(Vector2.right.x))
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

        CheckAndHandlePassiveWeapon();
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

        var directionForCast = horizontalMovementDirection > 0 ? Vector2.right : Vector2.left;
        var nextHorizontalMovementCastHits = new RaycastHit2D[2];
        var nextHorizontalMovementCast =
            _capsuleCollider2D.Cast(directionForCast, new ContactFilter2D{useTriggers = false}, nextHorizontalMovementCastHits, _body2d.velocity.x * Time.deltaTime + 0.1f);
        var wouldHitWallInAir = !_grounded && nextHorizontalMovementCast > 0;
        if (wouldHitWallInAir)
        {
            Debug.DrawRay(nextHorizontalMovementCastHits[0].point, nextHorizontalMovementCastHits[0].normal, Color.blue);
        }
        
        // Check if current move input is greater than very small and the move direction is equal to the characters facing direction
        if (!isMovementDisabled && Mathf.Abs(horizontalMovementDirection) > Mathf.Epsilon &&
            (int) Mathf.Sign(horizontalMovementDirection) == _facingDirection &&
            // not in Rest mode
            (artifactModeList.Count == 0 || artifactModeList[artifactModeIndex] != ArtifactMode.Rest) &&
            // if hitting wall while in air, stop movement to allow falling
            (!wouldHitWallInAir) &&
            // if not jumping
            !_shouldJump &&
            // if not firing
            !_shouldFire
        )
        {
            if (nextHorizontalMovementCast > 0)
            {
                HandleHorizontalDirectionChange(nextHorizontalMovementCastHits[0]);
            }
            CheckAndHandlePassiveJump(directionForCast);
            if (!_grounded)
            {
                newVelocity = _body2d.velocity.x;
            }
            else
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
            else
            {
                HandleEndOfMovement();
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

    private void CheckAndHandlePassiveJump(Vector2 directionForCast)
    {
        if (!_grounded || _isJumping || !IsCurrentArtifactMode(ArtifactMode.Jump)) return;
        var capsuleBounds = _capsuleCollider2D.bounds;
        var halfCapsuleWidth = capsuleBounds.size.x / 2;
        var capsuleColliderPosition = (Vector2) _capsuleCollider2D.transform.position + Vector2.up * 0.3f;
        var firstGroundSensorHits = new RaycastHit2D[2];
        var secondGroundSensorHits = new RaycastHit2D[2];
        var colliderHits = new RaycastHit2D[2];
        var shouldJumpOverSmallSpace =
            _capsuleCollider2D.Cast(directionForCast, new ContactFilter2D(){useTriggers = false}, colliderHits, halfCapsuleWidth + maxPassiveJumpDistance) == 0 &&
            Physics2D.Raycast(
                capsuleColliderPosition + directionForCast * (halfCapsuleWidth + minCliffDistanceToTriggerPassiveJump),
                Vector2.down, new ContactFilter2D { useTriggers = false }, firstGroundSensorHits, 0.5f) == 0 &&
            Physics2D.Raycast(
                capsuleColliderPosition + directionForCast *
                (halfCapsuleWidth + minCliffDistanceToTriggerPassiveJump + maxPassiveJumpDistance), Vector2.down,
                new ContactFilter2D { useTriggers = false }, secondGroundSensorHits, 5f) > 0;
        if (firstGroundSensorHits.Length > 0)
        {
            Debug.DrawRay(firstGroundSensorHits[0].point, firstGroundSensorHits[0].normal, Color.cyan, 25);
        }
        if (secondGroundSensorHits.Length > 0)
        {
            Debug.DrawRay(secondGroundSensorHits[0].point, secondGroundSensorHits[0].normal, Color.yellow, 25);
        }
        if (colliderHits.Length > 0)
        {
            Debug.DrawRay(colliderHits[0].point, colliderHits[0].normal, Color.green, 25);
        }

        if (!shouldJumpOverSmallSpace) return;

        HandleJump(passiveJumpForce, passiveJumpSpeed);
    }

    private void CheckAndHandlePassiveWeapon()
    {
        if (!IsCurrentArtifactMode(ArtifactMode.Weapon)) return;
        _secondsToWaitBeforeNextPassiveProjectile -= Time.deltaTime;

        if (_secondsToWaitBeforeNextPassiveProjectile > 0) return;
        if (_secondsToWaitBeforeNextPassiveProjectile < 0) _secondsToWaitBeforeNextPassiveProjectile = 0;
        HandleFireProjectile(ProjectileSize.Small);
    }

    private void HandleFireProjectile(ProjectileSize size)
    {
        var direction = currentHorizontalDirection == 1 ? Vector2.right : Vector2.left;
        var newProjectile = Instantiate(projectilePrefab);
        var newProjectileComponent = newProjectile.AddComponent<Projectile>();
        newProjectileComponent.size = size;
        newProjectileComponent.direction = direction;
        var newProjectilePosition = newProjectile.transform.position;
        var newProjectileScale = newProjectile.transform.localScale;
        var scale = size == ProjectileSize.Small ? 0.1f : 0.3f;
        newProjectile.transform.localScale = new Vector3(scale, scale, newProjectileScale.z);
        var position = transform.position;
        newProjectile.transform.position = new Vector3(position.x + _capsuleCollider2D.bounds.size.x / 2 + 0.2f, position.y + _capsuleCollider2D.bounds.size.y / 2 , newProjectilePosition.z);
        _secondsToWaitBeforeNextPassiveProjectile = secondsBetweenPassiveProjectiles;
        _shouldFire = false;
        _isFiring = false;
    }
    
    private void HandleEndOfMovement() {
        if (_shouldJump && !_isJumping)
        {
            StartCoroutine(WaitBeforeJump(secondsToWaitBeforeJump));
        }
        
        if(_shouldFire && !_isFiring)
        {
            StartCoroutine(WaitBeforeFire(secondsToWaitBeforeFire));
        }
    }

    /**
     * On Single Press from Input System
     */
    private void OnSinglePress()
    {
        if (isMovementDisabled) return;

        HandleArtifactModeChange();
        _mainUIComponent.RefreshArtifactList(this);
    }

    private void OnLongPress()
    {
        if (isMovementDisabled) return;
        
        if (artifactModeList.Count == 0) return;
        var currentArtifactMode = artifactModeList[artifactModeIndex];
        switch (currentArtifactMode)
        {
            case ArtifactMode.Jump:
                // Jump
                if (!_grounded) return;
                _shouldJump = true;
                break;
            case ArtifactMode.Weapon:
                // Jump
                if (!_grounded) return;
                _shouldFire = true;
                break;
        }
    }

    private bool IsCurrentArtifactMode(ArtifactMode mode)
    {
        return artifactModeList.Count > 0 && artifactModeList[artifactModeIndex] == mode;
    }

    private IEnumerator WaitBeforeJump(float seconds)
    {
        _isJumping = true;
        yield return new WaitForSeconds(seconds);
        HandleJump(jumpForce, jumpSpeed);
    }

    private IEnumerator WaitBeforeFire(float seconds)
    {
        _isFiring = true;
        yield return new WaitForSeconds(seconds);
        HandleFireProjectile(ProjectileSize.Big);
    }

    private void HandleJump(float jf, float speed)
    {
        
        _animator.SetTrigger(Jump);
        _grounded = false;
        _animator.SetBool(Grounded, _grounded);
        _body2d.velocity = new Vector2(currentHorizontalDirection * speed, jf);
        _groundSensor.Disable(0.2f);
        _shouldJump = false;
        _isJumping = false;
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

    public void SetCurrentHorizontalDirection(int direction)
    {
        currentHorizontalDirection = direction;
    }

    public void SetNotGrounded()
    {
        _groundSensor.Disable(0.2f);
        _grounded = false;
    }
}