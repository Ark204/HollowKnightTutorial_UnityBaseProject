using System;
using System.Collections;
using Core.UI;
using Core.Util;
using UnityEngine;

namespace Core.Character
{
    public class PlayerController : MonoBehaviour
    {
        public bool isPlayerControlled = true;

        public float speed = 100.0f;
        public float accelerationMultiplier = 2.5f;//���ٶȵļ��ٶ�
        public float maxAcceleration = 2.0f;
        public bool Jumpable { get => jumpable.GetRef();
            set {
                if (value) jumpable.Decrease();//set true=>-- =>true
                else jumpable.Increase();//set false=>++ =>false
            }
        }
        //�Ƿ�����Ծ
      [SerializeField] SharedCounter<bool> jumpable = new SharedCounter<bool>(true,() => { return false; }, (ref bool refbool) => { refbool = true;Debug.Log("default"); });
        public float jumpForce = 350.0f;
        public float airDrag = 0.8f;//��������
        public Transform bottomTransform;

        public GameObject hurtFlashObject;
        
        public ParticleSystem runningDustParticles;
        public ParticleSystem landingSmokeParticles;
        public ParticleSystem jumpingParticles;

        public Animator animator;
        private Rigidbody2D body;
        private BoxCollider2D collider;
        private AudioSource audioSource;

        // Moving axis
        private float horizontal;
        private float vertical;
        private Vector2 currentVelocity;
        private float movingVelocityX;

        private readonly Collider2D[] currentColliders = new Collider2D[5];
        private int currentColliderHits;

        private float previousPositionX;
        private float previousPositionY;
        private float acceleration = 1.0f;
        private float externalAcceleration = 1.0f;
        private float maxSpeed;
        private float afterJumpCooldown;

        public int facingDirection = 1;
        private float baseScaleX;

        public bool isOnGround;
        private bool isJumping;
        private bool wasJumping;

        private int environmentLayerMask;
        private bool isDying;

        // Hit Protection(�ܻ��޵�)
        private float hitProtectionDuration = 1.0f;
        private float hitProtectionTimer;
        public bool CanBeHit => hitProtectionTimer <= 0;

        // Camera
        private CameraController camController;
        private Camera cam;

        // Physics
        private PhysicsMaterial2D physicsMaterial;

        private bool blockingUI;
        private bool useGamepad;

        [SerializeField] private LayerMask combatLayerMask;
        
        // Properties
        public bool Controllable
        {
            get => controllable.GetRef() && !BlockingUI;
            set
            {
                if (value) controllable.Decrease();
                else controllable.Increase();
                if (!controllable.GetRef())
                {
                    body.velocity = new Vector2(0, body.velocity.y);
                    animator.SetFloat("Speed", 0);
                }
            }
        }
        [SerializeField] SharedCounter<bool> controllable = new SharedCounter<bool>(true,() => { return false; }, (ref bool ber) => { ber = true; });

        public bool BlockingUI
        {
            get => blockingUI;
            set
            {
                blockingUI = value;
                if (blockingUI)
                {
                    body.velocity = new Vector2(0, body.velocity.y);
                    animator.SetFloat("Speed", 0);
                }
            }
        }

        public bool Invincible { get; set; }

        public event Action OnDeath;
        public event Action OnLanded;//��ػص�
        public event Action OnTeleported;
        public event Action OnStunned;

        public static PlayerController Instance;//����

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            body = GetComponent<Rigidbody2D>();
            collider = GetComponent<BoxCollider2D>();
            audioSource = GetComponent<AudioSource>();
            //attackController = GetComponent<AttackController>();
            cam = Camera.main;
            camController = cam.GetComponent<CameraController>();

            physicsMaterial = collider.sharedMaterial;

            combatLayerMask = LayerMask.GetMask("Environment");
        }

        // Start is called before the first frame update
        void Start()
        {
            baseScaleX = transform.localScale.x;
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            environmentLayerMask = LayerMask.GetMask("Environment");

            maxSpeed = speed * maxAcceleration;
        }

        // Update is called once per frame
        void Update()
        {
            // Update timers
            if (hitProtectionTimer > 0)
                hitProtectionTimer -= Time.deltaTime;

            if (afterJumpCooldown > 0)
                afterJumpCooldown -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            GatherInput();

            Move();

            HandleCollisions();
            FinalCollisionCheck();

            previousPositionX = transform.position.x;
            previousPositionY = transform.position.y;
        }
        private void Move()
        {
            if (!Controllable)
                return;

            movingVelocityX = horizontal * speed * acceleration;

            // Air Drag
            if (!isOnGround)
            {
                movingVelocityX *= airDrag;
            }

            // Horizontal Movement and climbing
                body.velocity = Vector2.SmoothDamp(body.velocity, new Vector2(movingVelocityX, body.velocity.y),
                    ref currentVelocity, 0.02f);

            UpdateAcceleration();
            SetFacing();
            DoJump();

            // Artificially limit horizontal and vertical velocity
            float downwardsLimit = -24.0f;
            float upwardsLimit = 14.0f;
            body.velocity = new Vector2(Mathf.Clamp(body.velocity.x, -14.0f, 14.0f),
                Mathf.Clamp(body.velocity.y, downwardsLimit, upwardsLimit));


            HandleFalling();
            UpdateAnimator();

            wasJumping = isJumping;
        }
        //����
        private void GatherInput()
        {
            if (!Controllable)
                return;

            horizontal = 0;
            vertical = 0;
            isJumping = false;

            if (isPlayerControlled)
            {
                // Then overwrite with keyboard input (if present)
                horizontal = horizontal == 0
                    ? Input.GetAxis("Horizontal")
                    : horizontal;
                vertical = vertical == 0 ? Input.GetAxisRaw("Vertical") : vertical;

                float snapThreshold = 1;
                if (useGamepad)
                    snapThreshold = 0;

                // Normalize inputs
                if (horizontal > snapThreshold) horizontal = 1;
                else if (horizontal < -snapThreshold) horizontal = -1;
                if (vertical > snapThreshold) vertical = 1;
                else if (vertical < -snapThreshold) vertical = -1;

                isJumping = isJumping || (Jumpable&&Input.GetButton("Jump"));

            }
        }

        private void HandleCollisions()
        {
            bool wasOnGround = isOnGround;
            isOnGround = false;

            currentColliderHits = Physics2D.OverlapBoxNonAlloc(bottomTransform.position,
                new Vector2(0.24f, 0.2f), 0, currentColliders, combatLayerMask);
            //Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(bottomTransform.position, new Vector2(0.24f, 0.2f), 0, combatLayerMask);
            //bool haveCollision = false;
            //foreach (var coll in collider2Ds)
            //{
            //    Debug.Log(coll.gameObject.layer);
            //    if (!coll.isTrigger)//��ײ�����зǴ�����
            //    {
            //        haveCollision = true; break;
            //    }
            //}
            if (/*haveCollision*/currentColliderHits > 0)
            {
                isOnGround = true;

                if (!wasOnGround && afterJumpCooldown <= 0 && body.velocity.y < 3.0f)
                    HandleLanding();
            }


            if (wasOnGround && !isOnGround)
            {
                // Dropping off ledge
                animator.SetBool(CharacterAnimations.IsJumping, true);
                animator.SetTrigger(CharacterAnimations.StartJump);
                //runningDustParticles.gameObject.SetActive(false);
            }
        }

        private void FinalCollisionCheck()
        {
            // Predict velocity in next physics step -> y value??
            Vector2 moveDirection = new Vector2(body.velocity.x * Time.fixedDeltaTime, 0.02f);

            // Get bounds of Collider
            var topRight = new Vector2(collider.bounds.max.x, collider.bounds.max.y);
            var bottomLeft = new Vector2(collider.bounds.min.x, collider.bounds.min.y);

            // Move collider in direction that we are moving
            topRight += moveDirection;
            bottomLeft += moveDirection;

            // Check if the body's current velocity will result in a collision
            var hitCollider = Physics2D.OverlapArea(bottomLeft, topRight, environmentLayerMask);
            if (hitCollider != null)
            {
                body.velocity = new Vector3(body.velocity.x * 0.2f, body.velocity.y, 0);
                animator.SetFloat(CharacterAnimations.Speed, 0);
            }
        }

        private void HandleLanding()
        {
            animator.SetBool(CharacterAnimations.IsJumping, false);
            OnLanded?.Invoke();
            EffectManager.Instance.PlayOneShot(landingSmokeParticles, transform.position);

            camController.UpdateVertically();
        }

        private void SetFacing()
        {
            
                if (movingVelocityX < 0)
                {
                    transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
                    facingDirection = -1;
                }
                else if (movingVelocityX > 0)
                {
                    transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
                    facingDirection = 1;
                }
            
        }

        private void DoJump()
        {
            bool canJump = isOnGround;
            canJump = canJump && isJumping && !wasJumping;
            if (canJump)
            {
                StartJump();
            }

            if (!isOnGround && !isJumping && body.velocity.y > 0.01f)
            {
                CancelJump();
            }
        }

        private void StartJump()
        {
            animator.SetBool(CharacterAnimations.IsJumping, true);
            animator.SetTrigger(CharacterAnimations.StartJump);
            isOnGround = false;

            body.AddForce(new Vector2(0, jumpForce));

            afterJumpCooldown = 0.1f;
        }

        private void CancelJump()
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.72f);
        }

        private void HandleFalling()
        {
            if (body.velocity.y < -9.0f)
            {
                camController.UpdateVertically();
            }
        }

        private void UpdateAcceleration()
        {
            if (Mathf.Abs(horizontal) > 0)
            {
                acceleration += Time.fixedDeltaTime * accelerationMultiplier;
                acceleration = Mathf.Min(acceleration, maxAcceleration);
            }
            else
            {
                acceleration *= 0.95f;
                acceleration = Mathf.Max(acceleration, 1.0f);
            }

            externalAcceleration *= 0.9f;
            externalAcceleration = Mathf.Max(externalAcceleration, 1.0f);
        }

        private void UpdateAnimator()
        {
            animator.SetFloat(CharacterAnimations.Speed, Mathf.Abs(movingVelocityX * 1.5f));
        }

        /// <summary>
        /// �ⲿ����ʹ�������˺���
        /// </summary>
        /// <param name="damage">�˺�ֵ</param>
        /// <param name="recoilForce">������</param>
        /// <param name="vignetteIntensity">��֪��</param>
        /// <param name="killRecoil">��֪��</param>
        public void Hurt(int damage, Vector2 recoilForce, float vignetteIntensity = 0.7f, bool killRecoil = true)
        {
            //if (hitProtectionTimer > 0 || isDying)//�����޵�״̬||��������=>����
            //    return;

            //SetHealth(PlayerData.HP - damage);//��Ѫ

            //float shakeIntensity = 0.5f;
            //camController.ShakeCamera(shakeIntensity);//�������

            //if (PlayerData.HP <= 0)//Ѫ�����ڵ���0
            //{
            //    if (!Invincible)//���޵�
            //    {
            //        KillPlayer(killRecoil);//Ĩɱ����
            //        animator.SetTrigger(CharacterAnimations.Die);//������������
            //    }
            //}

            //hitProtectionTimer = hitProtectionDuration;//ˢ���ܻ��޵�ʱ��
            //Instantiate(hurtFlashObject, transform.position, Quaternion.identity);//������Ч
            //GuiManager.Instance.FadeHurtVignette(vignetteIntensity);//��֪��
            //GameManager.Instance.FreezeTime(0.02f);//����ʱ��

            //// Recoil
            //DoRecoil(recoilForce);
        }
        
        public void Concuss()
        {
            camController.ShakeCamera(0.3f);
        }

        public void Stun()
        {
            OnStunned?.Invoke();
            Concuss();
        }

        /// <summary>
        /// ʹ�������
        /// </summary>
        /// <param name="recoilForce">������</param>
        /// <param name="resetVelocity">�Ƿ������ٶ�</param>
        public void DoRecoil(Vector2 recoilForce, bool resetVelocity = false)
        {
            if (resetVelocity)
                body.velocity = Vector3.zero;

            body.AddForce(recoilForce);
        }
        /// <summary>
        /// ʹ��ɫ����
        /// </summary>
        /// <param name="recoil"></param>
        public void KillPlayer(bool recoil)
        {
            if (isDying)
                return;

            // Fade screen and respawn player at last save position
            // Restore HP and other stuff
            Invincible = true;

            if (recoil)
            {
                DoRecoil(new Vector2(0, 700.0f));
            }

            //SoundManager.Instance.PlaySound(deathSound, 1, audioSource);//������Ч
            isDying = true;//��������
            animator.SetBool("IsDying", true);//������������

            OnDeath?.Invoke();//����ί��
        }

    }
}
