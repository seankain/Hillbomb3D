using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotRiderController : BoardControllerBase
{
    //public event BailEventHandler PlayerBailed;
    //public event EventHandler PlayerRespawned;

    public bool Grounded = false;
    public Vector3 LevelDirection = Vector3.forward;
    public Vector3 SurfaceAngleEuler = Vector3.zero;
    public float ArbitraryConstant = 1;
    private Vector3 OriginalPosition;
    private Vector3 SteerDirection = Vector3.forward;
    public float TurnForceConstant = 1;
    public float MaxTurnTime = 3f;
    public float PowerSlideSpeed = 10f;
    public float MaxPushSpeed = 10f;
    public float PushCooldown = 1.0f;
    public Vector3 OlliePopForce = new Vector3(0, 10, 0);
    public Vector3 PushForce = new Vector3(0, 0, 10);
    public Vector3 DownwardForce = new Vector3(0, -1, 0);
    public GameObject Deck;
    public float MaxLeanZ = 10f;
    public float MaxLeanY = 10f;
    public Animator SkateboardAnimator;

    [SerializeField]
    private CharacterState characterState;
    private Transform characterBoardPosition;
    private Transform characterParent;

    private Rigidbody rb;

    const float radConvert = Mathf.PI / 180f;
    // Approximated a skateboard rolling down a hill using a solid cylinder rolling down an angled plane
    const float downhillAcc = (2f / 3f) * 9.81f;
    private float horizontal = 0;
    private float vertical = 0;
    private Vector3 powerSlideRotation = new Vector3(0, 90, 15);
    private bool backsideSlide = false;
    private bool frontsideSlide = false;
    private bool isPowerSliding = false;
    private float pushTimer = 0f;
    private bool pushing = false;
    //increments during turns to indicate how hard the player is trying to turn
    private float turnTime = 0;
    private bool ollieStarted = false;
    private float lastJump = 0;
    private bool bailed = false;
    private float bailTime = 5f;
    private float bailElapsed = 0f;
    private PlayerInput playerInput;
    private PlayerGruntEmitter gruntEmitter;

    protected override void OnPlayerBailed(BailEventArgs e)
    {
        base.OnPlayerBailed(e);
    }

    protected override void OnPlayerRespawned(EventArgs e)
    {
        base.OnPlayerRespawned(e);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        gruntEmitter = GetComponent<PlayerGruntEmitter>();
    }

    // Start is called before the first frame update
    void Start()
    {

        OriginalPosition = this.transform.position;
        playerInput = GetComponent<PlayerInput>();
        characterBoardPosition = characterState.gameObject.transform;
        characterParent = characterState.gameObject.transform.parent.transform;
        characterState.Respawn();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Respawn") > 0)
        {
            Respawn();
        }
        if (bailed) { bailElapsed += Time.deltaTime; if (bailElapsed >= bailTime) { Respawn(); } }
        horizontal = playerInput.Horizontal;
        vertical = playerInput.Vertical;
        var jump = playerInput.Jump;
        characterState.Grounded = Grounded;
        if (jump > 0) { ollieStarted = true; }
        if (jump < lastJump && ollieStarted && Grounded)
        {
            if (!isPowerSliding)
            {
                Ollie();
            }

        }
        // Don't want a delay based on axis returning to zero
        lastJump = jump;
        SkateboardAnimator.SetFloat("Turn", horizontal);
        if (Grounded && vertical > 0 && rb.velocity.magnitude < MaxPushSpeed)
        {
            pushing = true;
        }
        // Holding back on vertical and turning signals an intentional powerslide
        if (vertical < 0 && horizontal != 0 && !isPowerSliding)
        {

            backsideSlide = horizontal > 0;
            frontsideSlide = horizontal < 0;
            isPowerSliding = true;
            if (backsideSlide)
            {

                // StartCoroutine(Rotate(0.2f, 90f));
                SkateboardAnimator.SetBool("BacksidePowerslide", true);

            }
            else if (frontsideSlide)
            {

                //   StartCoroutine(Rotate(0.2f, -90f));
                SkateboardAnimator.SetBool("FrontsidePowerslide", true);
            }

        }
        else if (vertical == 0)
        {
            //once player is powersliding, they can stop by letting go on vertical
            if (isPowerSliding)
            {
                isPowerSliding = false;
                if (backsideSlide)
                {
                    //StartCoroutine(Rotate(0.2f, 0.1f));
                    SkateboardAnimator.SetBool("BacksidePowerslide", false);
                }
                else if (frontsideSlide)
                {
                    //StartCoroutine(Rotate(0.2f, 0.1f, -90f));
                    SkateboardAnimator.SetBool("FrontsidePowerslide", false);
                }
            }
            if (horizontal != 0)
            {
                if (turnTime < MaxTurnTime)
                {
                    turnTime += Time.deltaTime;
                }
            }
            else
            {
                turnTime = 0;
                //StartCoroutine(UnLean(0, 0));
            }

        }


    }

    private void Ollie()
    {
        characterState.Ollie();
        SkateboardAnimator.SetTrigger("Ollie");
        rb.AddForce(OlliePopForce);
    }

    private IEnumerator UnLean(float directionY, float directionZ, float duration = 0.1f)
    {
        var startLean = transform.rotation.eulerAngles;
        var endLean = new Vector3(startLean.x, 0, 0);
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            //Debug.Log($"{t}/{duration} = {t/duration}");
            var increment = Vector3.Lerp(startLean, endLean, t);
            //float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration);
            transform.rotation = Quaternion.Euler(increment);
            //Debug.Log($"{t/duration} {transform.rotation.eulerAngles}");
            yield return null;
        }
    }

    private IEnumerator Rotate(float duration, float rot, float originalRotation = Mathf.NegativeInfinity)
    {
        float startRotation = transform.eulerAngles.y;
        if (originalRotation != Mathf.NegativeInfinity)
        {
            startRotation = originalRotation;
        }
        //float endRotation = startRotation + rot;
        float endRotation = rot;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            //Debug.Log($"{t}/{duration} = {t/duration}");
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            //Debug.Log($"{t/duration} {transform.rotation.eulerAngles}");
            yield return null;
        }
    }

    private IEnumerator RotateReverse(float duration, float rot, float originalRotation = Mathf.NegativeInfinity)
    {
        float startRotation = transform.eulerAngles.y;
        if (originalRotation != Mathf.NegativeInfinity)
        {
            startRotation = originalRotation;
        }

        //float endRotation = startRotation + rot;
        float endRotation = rot;
        float t = duration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            //Debug.Log($"{t}/{duration} = {t/duration}");
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            Debug.Log($"{t / duration} {transform.rotation.eulerAngles}");
            yield return null;
        }
    }


    private void FixedUpdate()
    {
        var acc = ArbitraryConstant * (LevelDirection * (2f / 3f) * 9.8f * Mathf.Sin(radConvert * SurfaceAngleEuler.x));
        // var turnForce = (TurnForceConstant * rb.velocity.magnitude + turnTime) * Vector3.right * horizontal;
        var turnForce = TurnForceConstant * rb.velocity.z * Vector3.right * horizontal;
        //Debug.Log(acc);
        //Debug.Log(turnForce);
        //Debug.Log(Vector3.Cross(acc, turnForce));
        if (!isPowerSliding)
        {
            rb.AddForce(turnForce, ForceMode.Force);
        }
        if (pushing)
        {
            if (pushTimer == 0)
            {
                rb.AddForce(PushForce, ForceMode.VelocityChange);
            }
            pushTimer += Time.deltaTime;
            if (pushTimer >= PushCooldown)
            {
                pushing = false;
                pushTimer = 0;
            }
        }
        rb.AddForce(acc);
        rb.AddForce(DownwardForce);
    }

    private void Bail()
    {
        characterState.gameObject.transform.parent = null;

        //characterState.anim.SetTrigger("KnockedOff");
        //var ragdoll = GetComponentInChildren<RagdollSpawner>().Spawn();
        characterState.Bail();
        OnPlayerBailed(new BailEventArgs { RagdollInstance = characterState.gameObject });
        //PlayerBailed.Invoke(this, new BailEventArgs { RagdollInstance = characterState.gameObject });
        bailed = true;
        gruntEmitter.PlayRandom();
    }

    public void Respawn()
    {
        bailed = false;
        bailElapsed = 0f;
        rb.velocity = Vector3.zero;
        characterState.Respawn();
        var respawn = GameObject.FindGameObjectsWithTag("Respawn")[0];
        characterState.gameObject.transform.parent = characterParent;
        characterState.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 90, 0));
        //characterState.gameObject.transform.SetPositionAndRotation(characterBoardPosition.position, characterBoardPosition.rotation);
        this.transform.position = respawn.transform.position;
        this.transform.rotation = Quaternion.identity;
        OnPlayerRespawned(new EventArgs());
        //PlayerRespawned.Invoke(this, new EventArgs());


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "RidingSurface") { return; }
        //Debug.Log($"Player hitting {collision.collider.name} at {collision.relativeVelocity}");
        if (Mathf.Abs(collision.relativeVelocity.magnitude) > 20)
        {
            Bail();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Grounded = collision.gameObject.tag == "RidingSurface";
        if (Grounded)
        {
            //var contacts = new ContactPoint[collision.contactCount];
            //collision.GetContacts(contacts);
            SurfaceAngleEuler = collision.gameObject.transform.rotation.eulerAngles;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(SurfaceAngleEuler.x, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z));
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "RidingSurface")
        {
            Grounded = false;
        };
    }
}
