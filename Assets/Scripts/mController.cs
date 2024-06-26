using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class mController : MonoBehaviour
{
    #region fields
    [Header("Functional Options")]
    [SerializeField] public bool canLook = true;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool useFlashlight = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootsteps = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode flashlightKey = KeyCode.T;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float crouchSpeed = 3.0f;
    [SerializeField] private float slopeSlideSpeed = 10.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 90)] private float upperLookLimit = 88.0f;
    [SerializeField, Range(1, 90)] private float lowerLookLimit = 88.0f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private Transform Camera;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 9.0f;
    [SerializeField] private float gravity = 45.0f;
    private bool justJumped = false;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchingHeight = 0.4f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter 
        = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter 
        = new Vector3(0, 0, 0);
    private bool isCrouching;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.05f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] public float zoomFOV = 30f;
    public float defaultFOV;
    private Coroutine zoomRoutine;

    [Header("Flashlight")]
    [SerializeField] private Light flashlightSpot;
    //[SerializeField] private float flashlightSpotIntensity = 20f;
    //[SerializeField] private float flashlightSpotRange = 7f;
    [SerializeField] private Light flashlightPoint;
    //[SerializeField] private float flashlightPointIntensity = 20f;
    //[SerializeField] private float flashlightPointRange = 7f;
    [SerializeField] private AudioClip flashlightOn;
    [SerializeField] private AudioClip flashlightOff;
    private bool isFlashlightOn = false;

    // [Header("Interaction")]
    // [SerializeField] private Vector3 interactionRayPoint
    //     = new Vector3(0.5f, 0.5f, 0);
    // [SerializeField] private float interactionDistance = 4.0f;
    // [SerializeField] private LayerMask interactionLayer = default;
    // private C3Interactable currentInteractable;

    [Header("General Sound Parameters")]
    [SerializeField] private AudioSource onPlayerAudioSource;
    [SerializeField, Range(0, 1)] 
        private float walkVolumeScale = 0.7f;
    [SerializeField, Range(0, 1)] 
        private float sprintVolumeScale = 1f;

    [Header("Wet Step Sound Parameters")]
    [SerializeField] private bool useWetStep = true;
    [SerializeField, Range(0, 1)] 
        private float wetStepVolumeScale = 1f;
    
    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float sprintStepMultipier = 0.6f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;

    [Header("Footstep Walk Sounds")]
    // sand is default, others: concrete, metal, [?] wet
    [SerializeField] private AudioSource 
        footstepAudioSource = default;
    [SerializeField] private AudioSource 
        wetFootstepAudioSource = default;
    [SerializeField] private AudioClip[] sandClips = default;
    [SerializeField] private AudioClip[] concreteClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] wetClips = default;
    [SerializeField] private AudioClip[] deepwetClips = default;

    [Header("Footstep Jump & Land Sounds")]
    [SerializeField] private AudioClip[] sandJumpClips = default;
    [SerializeField] private AudioClip[] sandLandClips = default;
    [SerializeField] private AudioClip[] concreteJumpClips = default;
    [SerializeField] private AudioClip[] concreteLandClips = default;
    [SerializeField] private AudioClip[] metalJumpClips = default;
    [SerializeField] private AudioClip[] metalLandClips = default;
    //[SerializeField] private AudioClip[] wetJumpClips = default;
    [SerializeField] private AudioClip[] wetLandClips = default;
    [SerializeField] private AudioClip[] deepwetLandClips = default;
    private int lastRandomIndex = 0;
    #endregion

    private Vector3 hitPointNormal; // used for sliding
    private float footstepTimer = 0;
    private Camera playerCamera;
    private CharacterController controller;

    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0;

    [SerializeField, ReadOnly]
    private bool isGrounded;
    private RaycastHit raycastHit;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        isGrounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out raycastHit, 2);

        if (canLook){
            HandleMovementInput();
            HandleMouseLook();
        }
        if(canJump){
            HandleJump();
        }
        if(canCrouch){
            HandleCrouch();
        }
        if(canUseHeadBob){
            HandleHeadBob();
        }
        if(canZoom){
            HandleZoom();
        }
        if(canInteract){
            //HandleInteractionCheck();
            //HandleInteractionInput();
        }
        if(useFlashlight){
            HandleFlashlight();
        }
        if(useFootsteps){
            HandleFootsteps();
        }
        ApplyFinalMovements();
    }

    #region Movement

    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey);
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && controller.isGrounded;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : IsSprinting ? baseStepSpeed * sprintStepMultipier : baseStepSpeed;
    private float CurrentSpeed => isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;
    private bool IsSliding
    {
        get
        {
            if(controller.isGrounded && isGrounded)
            {
                hitPointNormal = raycastHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > controller.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2(
            CurrentSpeed * Input.GetAxis("Vertical"),
            CurrentSpeed * Input.GetAxis("Horizontal")
        );
        float moveDirYCache = moveDirection.y;
        moveDirection = transform.forward * currentInput.x + transform.right * currentInput.y;
        moveDirection.y = moveDirYCache;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, upperLookLimit);
        cameraRoot.transform.localRotation = 
            Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        if(ShouldJump && isGrounded)
        {
            justJumped = true;
            if (isGrounded)
            {
                if(useFootsteps)
                    PlayJumpSound(raycastHit);
            }

            moveDirection.y = jumpForce;

        }
        if (controller.isGrounded && !ShouldJump && justJumped == true)
        {
            justJumped = false;
            if (isGrounded)
            {   
                if(useFootsteps)
                    PlayLandSound(raycastHit);
            }
        }
    }
    private void PlayJumpSound(RaycastHit hit)
    {
        switch(hit.collider.tag)
        {
            case "footsteps/concrete":
                PlaySound(concreteJumpClips, 1f);
                break;
            case "footsteps/metal":
                PlaySound(metalJumpClips, 1f);
                break;
            case "footsteps/sand":
                PlaySound(sandJumpClips, 1f);
                break;
            default:
                PlaySound(concreteJumpClips, 1f);
                break;
        }

        //if(useWetStep)
            //PlaySound(wetJumpClips, 1f, wetFootstepAudioSource);
    }
    private void PlayLandSound(RaycastHit hit)
    {
        switch(hit.collider.tag)
        {
            case "footsteps/concrete":
                PlaySound(concreteLandClips, 1f);
                break;
            case "footsteps/metal":
                PlaySound(metalLandClips, 1f);
                break;
            case "footsteps/sand":
                PlaySound(sandLandClips, 1f);
                break;
            case "footsteps/wet":
                PlaySound(wetLandClips, 1f);
                break;
            case "footsteps/deepwet":
                PlaySound(deepwetLandClips, 1f);
                break;
            default:
                PlaySound(concreteLandClips, 1f);
                break;
        }

        //if(useWetStep)
            //PlaySound(wetLandClips, 1f, wetFootstepAudioSource);
    }

    private void HandleFootsteps()
    {
        if (!controller.isGrounded) return;
        if (currentInput == Vector2.zero) return;
        footstepTimer -= Time.deltaTime;

        if(footstepTimer <= 0)
        {
            if (isGrounded)
            {
                if(IsSprinting)
                {
                    PlayFootstepSound(raycastHit);
                }
                else
                {
                    PlayFootstepSound(raycastHit);
                }
            }
            footstepTimer = GetCurrentOffset;
        }
    }
    private void PlayFootstepSound(RaycastHit hit)
    {
        switch(hit.collider.tag)
        {
            case "footsteps/concrete":
                PlaySound(concreteClips);
                break;
            case "footsteps/metal":
                PlaySound(metalClips);
                break;
            case "footsteps/sand":
                PlaySound(sandClips);
                break;
            case "footsteps/wet":
                PlaySound(wetClips);
                break;
            case "footsteps/deepwet":
                PlaySound(deepwetClips);
                break;
            default:
                PlaySound(concreteClips);
                break;
        }

        if(useWetStep)
            PlaySound(wetClips, wetStepVolumeScale, wetFootstepAudioSource);
    }

    private void HandleCrouch()
    {
        if(ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void ApplyFinalMovements()
    {
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (willSlideOnSlopes && IsSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSlideSpeed;
        }

        controller.Move(moveDirection * Time.deltaTime);
    }
    #endregion

    private IEnumerator CrouchStand()
    {
        if (isCrouching && isGrounded)
        {
            yield break;
        }

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchingHeight;
        float currentHeight = controller.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }

    private void HandleHeadBob()
    {
        if (!controller.isGrounded) return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount), 
                playerCamera.transform.localPosition.z);
        }
    }

    private void HandleZoom()
    {
        //Zooms in if zoomKey is being held down
        if (Input.GetKeyDown(zoomKey))
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        //Zooms out if zoomKey is released
        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private void HandleFlashlight()
    {
        if (Input.GetKeyDown(flashlightKey) && flashlightSpot != null
            && flashlightPoint != null)
        {
            if (flashlightSpot.enabled)
            {
                onPlayerAudioSource.PlayOneShot(flashlightOff);
            }
            else
            {
                onPlayerAudioSource.PlayOneShot(flashlightOn);
            }
            flashlightSpot.enabled = !flashlightSpot.enabled;
            flashlightPoint.enabled = !flashlightPoint.enabled;
        }

        //flashlightSpot.intensity = flashlightIntensity;
        //flashlightSpot.range = flashlightRange;
    }

    private void PlaySound(AudioClip[] clips, float volume, AudioSource source)
    {
        int random = 0;
        if(clips.Length > 1)
        {
            random = Random.Range(0, clips.Length - 1);
            if(random == lastRandomIndex)
                random = Random.Range(0, clips.Length - 1);
            if(random == lastRandomIndex)
                random = Random.Range(0, clips.Length - 1);
            if(random == lastRandomIndex)
                random = Random.Range(0, clips.Length - 1);
        }

        source.PlayOneShot(clips[random]);
        lastRandomIndex = random;
    }
    private void PlaySound(AudioClip[] clips, float volume)
    {
        PlaySound(clips, volume, footstepAudioSource);
    }
    private void PlaySound(AudioClip[] clips)
    {
        float volume = walkVolumeScale;
        if(IsSprinting)
            volume = sprintVolumeScale;

        PlaySound(clips, volume, footstepAudioSource);
    }
}
