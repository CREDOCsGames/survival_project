using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEditor.Animations;
using System.Linq;

public enum CHARACTER_NUM
{
    Bagic,
    Legendary,
    Count,
}

public class Character : Singleton<Character>
{
    [SerializeField] SpriteRenderer rendUpper;
    [SerializeField] SpriteRenderer rendLower;
    [SerializeField] public Animator anim;
    [SerializeField] ParticleSystem particle;
    [SerializeField] float particleScale;

    [SerializeField] Slider playerHpBar;
    [SerializeField] SpriteRenderer[] weaponImages;

    [Header("Stat")]
    [SerializeField] RuntimeAnimatorController[] currentController;

    [HideInInspector] public float dashCoolTime;
    [HideInInspector] public float initDashCoolTime;
    [HideInInspector] public int dashCount;

    [HideInInspector] public int maxHp;
    [HideInInspector] public int currentHp;
    [HideInInspector] public int recoverHpRatio;
    [HideInInspector] public float speed;
    [HideInInspector] public int avoid;
    [HideInInspector] public float attackSpeed;
    /*[HideInInspector]*/ public int defence;
    [SerializeField] float invincibleTime;
    public int percentDamage;
    public int percentDefence;

    [HideInInspector] public int maxRecoveryGauge;
    int initMaxRecGauge;
    [HideInInspector] public int currentRecoveryGauge;
    public GameObject getItemUI;
    int recoveryValue = 10;
    public int RecoveryValue => recoveryValue;

    [Header("Summon")]
    [SerializeField] GameObject tamedPet;

    Collider ground;

    bool isRun, isAttacked = false;
    bool isAvoid = false;
    [HideInInspector] public bool isDead = false;

    GameManager gameManager;

    [HideInInspector] Vector3 dir;
    [HideInInspector] public Vector3 charDir => dir;
    [HideInInspector] public float x;
    [HideInInspector] public float z;

    Coroutine currentCoroutine;

    [HideInInspector] public CharacterInfo currentCharacterInfo;

    public Transform weaponParent;
    [HideInInspector] public bool canWeaponChange = true;
    [HideInInspector] public int currentWeaponIndex;

    NavMeshAgent agent;

    [HideInInspector] public bool isCanControll = true;
    public bool canFlip = true;

    bool isTamingPet = false;
    int getPetRound = 0;

    public bool IsTamingPet => isTamingPet;
    public int GetPetRound => getPetRound;

    public GameObject TamedPed => tamedPet;

    GamesceneManager gamesceneManager;

    public bool IsFlip => rendUpper.flipX;

    Vector3 initParticleScale;

    [SerializeField] AnimationClip[] loggingClips;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        particle.GetComponentInChildren<Renderer>().enabled = false;
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        gameManager = GameManager.Instance;

        initMaxRecGauge = maxRecoveryGauge;
        currentRecoveryGauge = 0;

        currentWeaponIndex = 0;

        dashCoolTime = 4;
        dashCount = gameManager.dashCount;
        initDashCoolTime = dashCoolTime;

        getItemUI.gameObject.SetActive(false);

        tamedPet.SetActive(false);

        initParticleScale = particle.transform.localScale;

        ChangeAnimationController(0);

        rendLower.gameObject.SetActive(false);

        transform.position = gameManager.characterSpawnPos;
    }
    
    void Update()
    {
        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            HpSetting();

            if (!isCanControll)
                return;

            if (currentHp > 0)
            {
                UseRecoveyGauege();
                Dash();
                //AutoRecoverHp();
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            if (!isCanControll)
            {
                Flip();
                isRun = false;
                anim.SetBool("isRun", isRun);
                return;
            }

            isRun = false;

            if (currentHp > 0)
                Move();

            anim.SetFloat("moveSpeed", 1 + (speed * 0.1f));
            anim.SetBool("isRun", isRun);
        }
    }

    public void UpdateStat()
    {
        if(gamesceneManager == null)
            gamesceneManager = GamesceneManager.Instance;

        maxHp = gameManager.status[Status.Maxhp];
        currentHp = maxHp;
        speed = gameManager.status[Status.MoveSpeed];
        avoid = gameManager.status[Status.Avoid];
        recoverHpRatio = gameManager.status[Status.Recover];
        attackSpeed = gameManager.status[Status.AttackSpeed];
        defence = gameManager.status[Status.Defence];

        maxRecoveryGauge = gameManager.specialStatus[SpecialStatus.Grape] ? initMaxRecGauge + 10 : initMaxRecGauge;
        speed = gameManager.specialStatus[SpecialStatus.Tabatiere] & !gamesceneManager.isNight ? speed * 1.1f : speed;
        invincibleTime = gameManager.specialStatus[SpecialStatus.Invincible] ? 0.2f : 0;
        defence = gameManager.specialStatus[SpecialStatus.SilverBullet] ? defence + 5 : defence;

        percentDamage = gameManager.percentDamage;
        percentDamage = gameManager.specialStatus[SpecialStatus.RottenCheese] ? percentDamage + 30 : percentDamage;

        percentDefence = gameManager.percentDefence;
        percentDefence = gameManager.specialStatus[SpecialStatus.TurTle] ? percentDefence + 30 : percentDefence;
        percentDefence = gameManager.specialStatus[SpecialStatus.RottenCheese] ? percentDefence - 20 : percentDefence;
    }

    void HpSetting()
    {
        if (currentHp > maxHp)
            currentHp = maxHp;

        playerHpBar.value = 1 - ((float)currentHp / maxHp);
    }

    void UseRecoveyGauege()
    {
        if (currentRecoveryGauge >= recoveryValue && Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ConvertRecoveryGauge());
        }
    }

    IEnumerator ConvertRecoveryGauge()
    {
        isCanControll = false;
        currentHp += Mathf.CeilToInt(recoveryValue * (100 + recoverHpRatio) * 0.01f);
        currentRecoveryGauge -= recoveryValue;

        yield return CoroutineCaching.WaitForSeconds(0.5f);

        isCanControll = true;
    }

    public void TamingPet(int round)
    {
        isTamingPet = true;
        getPetRound = round;

        tamedPet.SetActive(true);
    }

    public void RunAwayPet()
    {
        isTamingPet = false;
        tamedPet.SetActive(false);
    }

    void AutoRecoverHp()
    {
        /*if (gameManager.recoverHp > 0 && currentHp < maxHp)
        {
            recoverTime -= Time.deltaTime;
            if (recoverTime <= 0)
            {
                recoverTime = 1;
                currentHp += gameManager.recoverHp;
            }
        }*/
    }

    void Dash()
    {
        if (gameManager.dashCount <= 0)
            return;

        if (dashCount > 0 && isCanControll)
        {
            Vector3 afterPos;

            particle.transform.localScale = rendUpper.flipX ? initParticleScale * particleScale : new Vector3(-initParticleScale.x, initParticleScale.y, initParticleScale.z) * particleScale;

            if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Key_Dash")))
            {
                particle.GetComponentInChildren<Renderer>().enabled = true;

                if (x == 0 && z == 0)
                    afterPos = new Vector3(transform.position.x + 2, 0, transform.position.z);

                else
                    afterPos = transform.position + new Vector3(x, 0, z) * 4;

                NavMeshHit hit;

                if (NavMesh.SamplePosition(afterPos, out hit, 1000, NavMesh.AllAreas))
                {
                    afterPos = hit.position;
                }

                afterPos = new Vector3(afterPos.x, transform.position.y, afterPos.z);

                agent.enabled = false;

                //transform.position = Vector3.Lerp(beforePos, afterPos, 1);
                transform.position = afterPos;

                agent.enabled = true;

                dashCount--;
                Invoke("ParticleOff", 0.2f);

                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);

                currentCoroutine = StartCoroutine(IEDashInvincible());
            }
        }

        if (dashCount < gameManager.dashCount)
        {
            dashCoolTime -= Time.deltaTime;

            if (dashCoolTime <= 0)
            {
                dashCount++;
                dashCoolTime = initDashCoolTime;
            }
        }
    }

    public void InitailizeDashCool()
    {
        dashCoolTime = initDashCoolTime;
    }

    void ParticleOff()
    {
        particle.GetComponentInChildren<Renderer>().enabled = false;
    }

    bool isDownUp = false;
    bool isLeftRight = false;

    void Move()
    {
        bool xInput = (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Left"))) || (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")));
        bool zInput = (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Up"))) || (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Down")));

        if (!xInput)
            x = 0;

        if (!zInput)
            z = 0;

        if (zInput)
        {
            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Up")))
            {
                z = 1;

                if (!isDownUp && Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Down")))
                    z = -1;
            }

            else if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Down")))
            {
                z = -1;
                isDownUp = true;
            }

            if (Input.GetKeyUp((KeyCode)PlayerPrefs.GetInt("Key_Down")))
                isDownUp = false;
        }

        if (xInput)
        {
            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Left")))
            {
                x = -1;

                if (!isLeftRight && Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")))
                {
                    x = 1;
                }
            }

            else if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")))
            {
                x = 1;
                isLeftRight = true;
            }

            if (Input.GetKeyUp((KeyCode)PlayerPrefs.GetInt("Key_Right")))
                isLeftRight = false;
        }

        dir = (Vector3.right * x + Vector3.forward * z).normalized;

        agent.speed = speed;
        agent.Move(dir * speed * Time.deltaTime);
        
        if (ground == null)
            ground = GamesceneManager.Instance.walkableArea;

        if (dir != Vector3.zero)
            isRun = true;

        else if (dir == Vector3.zero)
            isRun = false;

        Flip();
    }

    void Flip()
    {
        if (!canFlip)
            return;

        if (gamesceneManager == null)
            gamesceneManager = GamesceneManager.Instance;

        if (!gamesceneManager.isNight)
        {
            rendLower.gameObject.SetActive(false);

            if (dir.x == 0)
                return;

            rendUpper.flipX = dir.x > 0;
        }

        else
        {
            rendLower.gameObject.SetActive(true);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float dirX = transform.position.x - mousePos.x;

            if (dirX == 0)
                return;

            rendUpper.flipX = dirX < 0;
            rendLower.flipX = dirX < 0;
        }
    }

    int avoidRand;

    public void OnDamaged(float damage, GameObject damagedObject)
    {
        if (!isAttacked)
        {
            avoidRand = Random.Range(1, 100);

            isAvoid = avoidRand <= gameManager.status[Status.Avoid];

            if (currentHp > 0)
            {
                if (!isAvoid)
                {
                    SoundManager.Instance.PlayES("Hit");

                    int trueDamage = Mathf.RoundToInt((damage - gameManager.status[Status.Defence]) * (100 + percentDefence) * 0.01f);

                    if (gameManager.specialStatus[SpecialStatus.Rum])
                        trueDamage = Mathf.RoundToInt(trueDamage * 1.5f);

                    currentHp -= trueDamage;

                    if (gameManager.specialStatus[SpecialStatus.Mirror])
                    {
                        damagedObject.GetComponent<IDamageable>().Attacked(Mathf.Round(trueDamage / 2), this.gameObject);
                        damagedObject.GetComponent<IDamageable>().RendDamageUI(Mathf.Round(trueDamage / 2), damagedObject.transform.position, false, false);
                    }


                    if (gameManager.specialStatus[SpecialStatus.BloodMadness])
                        gameManager.bloodDamage = Mathf.Clamp(gameManager.bloodDamage + 2, 0, 20);
                }

                ChangeHitColor();
            }

            if (currentHp <= 0)
                OnDead();
        }
    }

    void OnDead()
    {
        currentHp = 0;

        isAttacked = true;
        isRun = false;
        isDead = true;

        StopAllCoroutines();

        gameObject.SetActive(false);
    }

    void ChangeHitColor()
    {
        isAttacked = true;

        if (currentHp > 0 && !isAvoid)
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(PlayerColorBlink());
        }

        if(currentHp > 0 && isAvoid)
        {
            SoundManager.Instance.PlayES("Avoid");
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(PlayerColorInvincible());
        }
    }

    public IEnumerator IEDashInvincible()
    {
        Color semiWhite = Color.white;

        isAttacked = true;
        semiWhite.a = 0.5f;
        rendUpper.color = semiWhite;
        rendLower.color = semiWhite;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = semiWhite;

        yield return CoroutineCaching.WaitForSeconds(0.5f);
        
        rendUpper.color = Color.white;
        rendLower.color = Color.white;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = Color.white;

        isAttacked = false;
    }

    private IEnumerator PlayerColorInvincible()
    {
        Color semiWhite = new Color(1, 1, 1, 0.5f);

        rendUpper.color = semiWhite;
        rendLower.color = semiWhite;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = semiWhite;

        float waitTime = invincibleTime <= 0 ? 0 : invincibleTime;

        yield return CoroutineCaching.WaitForSeconds(waitTime);

        rendUpper.color = Color.white;
        rendLower.color = Color.white;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = Color.white;

        isAttacked = false;
    }

    private IEnumerator PlayerColorBlink()
    {
        Color semiRed = new Color(1, 0, 0, 0.5f);
        Color semiWhite = new Color(1, 1, 1, 0.5f);

        rendUpper.color = semiRed;
        rendLower.color = semiRed;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = semiRed;
        yield return CoroutineCaching.WaitForSeconds(0.1f);

        rendUpper.color = semiWhite;
        rendLower.color = semiWhite;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = semiWhite;
        yield return CoroutineCaching.WaitForSeconds(0.1f);

        rendUpper.color = semiRed;
        rendLower.color = semiRed;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = semiRed;
        yield return CoroutineCaching.WaitForSeconds(0.1f);

        rendUpper.color = semiWhite;
        rendLower.color = semiWhite;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = semiWhite;

        float waitTime = invincibleTime - 0.3f <= 0 ? 0 : invincibleTime;

        yield return CoroutineCaching.WaitForSeconds(waitTime);

        rendUpper.color = Color.white;
        rendLower.color = Color.white;
        if (weaponImages[currentWeaponIndex] != null)
            weaponImages[currentWeaponIndex].color = Color.white;

        isAttacked = false;
    }

    public IEnumerator MoveToInteractableObject(Vector3 movePos, GameObject interactionObejct, float animTime, int animNum, int clipNum = -1)
    {
        isCanControll = false;

        dir = (movePos - transform.position).normalized;

        Flip();

        ChangeAnimationController(animNum);
        if (clipNum != -1)
            ChangeAnimClip(clipNum);

        while (transform.position != movePos)
        {
            if(gamesceneManager.isNight)
            {
                isCanControll = true;
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, movePos, Time.deltaTime * speed);

            anim.SetFloat("moveSpeed", 1 + (speed * 0.1f));
            anim.SetBool("isRun", true);

            if (transform.position == movePos && !gamesceneManager.isNight)
            {
                anim.SetBool("isLogging", true);
                StartCoroutine(interactionObejct.GetComponent<IMouseInteraction>().EndInteraction(anim, animTime));
            }

            yield return null;
        }
    }

    void ChangeAnimClip(int num)
    {
        var controller = (AnimatorController)anim.runtimeAnimatorController;
        var state = controller.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name.Equals("Logging")).state;
        controller.SetStateEffectiveMotion(state, loggingClips[num]);
    }

    public void ChangeAnimationController(int num)
    {
        anim.runtimeAnimatorController = currentController[num];
    }
}
