using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public enum CHARACTER_NUM
{
    Bagic,
    Legendary,
    Count,
}

public class Character : Singleton<Character>
{
    [SerializeField] public SpriteRenderer rend;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem particle;
    [SerializeField] float particleScale;

    [SerializeField] Slider playerHpBar;

    [Header("Stat")]
    [SerializeField] public int characterNum;
    [SerializeField] public CharacterInfo[] characterInfos;
    [SerializeField] RuntimeAnimatorController[] currentController;

    [HideInInspector] public float dashCoolTime;
    [HideInInspector] public float initDashCoolTime;
    [HideInInspector] public int dashCount;

    [Header("Weapon")]
    [SerializeField] GameObject WeaponParent;
    [SerializeField] public GameObject[] weapons;
    [SerializeField] public Transform[] weaponPoses;
    [SerializeField] public GameObject thunderMark;

    [HideInInspector] public float exp;
    [HideInInspector] public float maxHp;
    [HideInInspector] public float currentHp;
    [HideInInspector] public float recoverHpRatio;
    [HideInInspector] public float speed;
    [HideInInspector] public int avoid;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public int defence;

    public float maxRecoveryGauge;
    [HideInInspector] public float currentRecoveryGauge;
    public GameObject fruitUI;
    float recoveryValue = 10f;

    [Header("Summon")]
    [SerializeField] GameObject tamedPet;
    [SerializeField] GameObject ggoGgoPrefab;
    [SerializeField] GameObject ilsoonPrefab;
    [SerializeField] GameObject wakgoodPrefab;
    [SerializeField]  public Transform[] summonPos;
    [HideInInspector] public int summonNum;

    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask groundLayer;

    Collider ground;

    bool isRun, isAttacked = false;
    bool isAvoid = false;
    [HideInInspector] public bool isDead = false;

    GameManager gameManager;

    [HideInInspector] Vector3 dir;
    [HideInInspector] public Vector3 charDir => dir;
    [HideInInspector] public float x;
    [HideInInspector] public float z;

    float recoverTime;

    [HideInInspector] public bool isBuff = false;
    [HideInInspector] public float charBuffDmg = 0;
    [HideInInspector] public float buffTime = 5;

    [HideInInspector] public int thunderCount;

    Coroutine currentCoroutine;

    [HideInInspector] public float shield = 0;

    [HideInInspector] public CharacterInfo currentCharacterInfo;

    public Transform weaponParent;
    public bool canWeaponChange = true;

    NavMeshAgent agent;

    public bool isCanControll = true;

    bool isTamingPet = false;
    int getPetRound = 0;

    public bool IsTamingPet => isTamingPet;
    public int GetPetRound => getPetRound;

    public GameObject TamedPed => tamedPet;

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
        transform.position = new Vector3(0f, 0f, -40f);

        thunderMark.transform.localScale = new Vector3(Mathf.Clamp(4f + gameManager.range * 0.5f, 1, 12), Mathf.Clamp(4f + gameManager.range * 0.5f, 1, 12), 0);

        /*maxHp = gameManager.maxHp;
        currentHp = maxHp;
        speed = gameManager.speed;
        avoid = gameManager.avoid;
        recoverHpRatio = gameManager.recoverHp;
        attackSpeed = gameManager.attackSpeed;
        defence = gameManager.defence;*/

        maxHp = gameManager.status[Status.MAXHP];
        currentHp = maxHp;
        speed = gameManager.status[Status.SPEED];
        avoid = gameManager.status[Status.AVOID];
        recoverHpRatio = gameManager.status[Status.RECOVER];
        attackSpeed = gameManager.status[Status.ATTACK_SPEED];
        defence = gameManager.status[Status.DEFENCE];

        recoverTime = 1;
        dashCoolTime = 4;
        dashCount = gameManager.dashCount;
        initDashCoolTime = dashCoolTime;

        fruitUI.gameObject.SetActive(false);

        tamedPet.SetActive(false);
    }
    
    void Update()
    {
        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            HpSetting();

            if (currentHp > 0 && (!gameManager.isClear || !gameManager.isBossDead))
            {
                UseRecoveyGauege();
                Dash();
                //AutoRecoverHp();
            }

            else if (gameManager.isClear && gameManager.isBossDead)
            {
                isBuff = false;
                buffTime = 5;
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            if (!isCanControll)
            {
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

    void HpSetting()
    {
        if (currentHp > maxHp)
            currentHp = maxHp;

        playerHpBar.value = 1 - (currentHp / maxHp);
    }

    void UseRecoveyGauege()
    {
        if (!isCanControll)
            return;

        if (currentRecoveryGauge >= recoveryValue && Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ConvertRecoveryGauge());
        }
    }

    IEnumerator ConvertRecoveryGauge()
    {
        isCanControll = false;
        currentHp += recoveryValue * recoverHpRatio;
        currentRecoveryGauge -= recoveryValue;

        yield return new WaitForSeconds(1f);

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
        if (gameManager.recoverHp > 0 && currentHp < maxHp)
        {
            recoverTime -= Time.deltaTime;
            if (recoverTime <= 0)
            {
                recoverTime = 1;
                currentHp += gameManager.recoverHp;
            }
        }
    }

    void Dash()
    {
        if (!isCanControll)
            return;

        if (gameManager.dashCount > 0)
        {
            Vector3 beforePos;
            Vector3 afterPos;

            if (dashCount > 0)
            {
                if (rend.flipX == true)
                    particle.transform.localScale = new Vector3(-1, 1, 1) * particleScale;

                else if (rend.flipX == false)
                    particle.transform.localScale = new Vector3(1, 1, 1) * particleScale;

                if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Key_Dash")))
                {
                    particle.GetComponentInChildren<Renderer>().enabled = true;

                    beforePos = transform.position;

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

                    transform.position = Vector3.Lerp(beforePos, afterPos, 1);

                    agent.enabled = true;

                    dashCount--;
                    Invoke("ParticleOff", 0.2f);

                    if (currentCoroutine != null)
                        StopCoroutine(currentCoroutine);

                    currentCoroutine = StartCoroutine(IEDashInvincible());
                }
            }

            if (dashCount != gameManager.dashCount)
            {
                dashCoolTime -= Time.deltaTime;

                if (dashCoolTime <= 0)
                {
                    dashCount++;
                    dashCoolTime = initDashCoolTime;
                }
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

    public void ReleaseEquip(int num)
    {
        Destroy(weaponPoses[num].GetChild(0).gameObject);
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

       /* else
            transform.position =  NavMesh.poi ground.bounds.ClosestPoint(transform.position);*/

        if (dir != Vector3.zero)
            isRun = true;

        else if (dir == Vector3.zero)
            isRun = false;

        Flip();
    }

    void Flip()
    {
        if (dir.x == 0)
            return;

        rend.flipX = dir.x > 0;
    }

    int avoidRand;

    public void OnDamaged(float damage)
    {
        if (!isAttacked)
        {
            avoidRand = Random.Range(1, 100);

            if (avoidRand <= gameManager.avoid)
                isAvoid = true;

            else
                isAvoid = false;

            if (currentHp > 0)
            {
                if (!isAvoid)
                {
                    SoundManager.Instance.PlayES("Hit");
                    if (shield > 0)
                    {
                        currentHp -= (Mathf.Round(((damage - shield) * ((100 - gameManager.defence) / 100)) * 10) * 0.1f);
                        shield = Mathf.Clamp(shield - damage, 0f, 10f);
                    }

                    else if (shield <= 0)
                        currentHp -= Mathf.Round((damage * ((100 - gameManager.defence) / 100)) * 10) * 0.1f;
                }

                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);

                currentCoroutine = StartCoroutine(OnInvincible());
            }

            if (currentHp <= 0)
                OnDead();
        }
    }

    void OnDead()
    {
        isAttacked = true;
        isRun = false;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    public IEnumerator OnInvincible()
    {
        isAttacked = true;

        if (currentHp > 0 && !isAvoid)
        {
            anim.SetTrigger("isAttacked");

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

        yield return new WaitForSeconds(characterInfos[characterNum].InvincibleTime);
        rend.color = Color.white;
        isAttacked = false;
    }

    public IEnumerator IEDashInvincible()
    {
        Color color = Color.white;

        isAttacked = true;
        color.a = 0.5f;
        rend.color = color;

        yield return new WaitForSeconds(characterInfos[characterNum].InvincibleTime);
        color.a = 1f;
        rend.color = color;
        rend.color = Color.white;
        isAttacked = false;
    }

    private IEnumerator PlayerColorInvincible()
    {
        Color white = new Color(1, 1, 1, 0.5f);

        rend.color = white;

        yield return new WaitForSeconds(characterInfos[characterNum].InvincibleTime - 0.3f);
    }

    private IEnumerator PlayerColorBlink()
    {
        Color red = new Color(1, 0, 0, 0.5f);
        Color white = new Color(1, 1, 1, 0.5f);

        rend.color = red;
        yield return new WaitForSeconds(0.1f);

        rend.color = white;
        yield return new WaitForSeconds(0.1f);

        rend.color = red;
        yield return new WaitForSeconds(0.1f);

        rend.color = white;
        yield return new WaitForSeconds(characterInfos[characterNum].InvincibleTime - 0.3f);
    }

    public IEnumerator MoveToInteractableObject(Vector3 logPos, GameObject interactionObejct)
    {
        isCanControll = false;

        dir = (logPos - transform.position).normalized;

        Flip();

        while (transform.position != logPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, logPos, Time.deltaTime * speed);

            anim.SetFloat("moveSpeed", 1 + (speed * 0.1f));
            anim.SetBool("isRun", true);

            if (transform.position == logPos)
            {
                ChangeAnimationController(1);
                anim.SetBool("isLogging", true);
                StartCoroutine(interactionObejct.GetComponent<IMouseInteraction>().EndInteraction(anim, 3));
            }

            yield return null;
        }
    }

    public void ChangeAnimationController(int num)
    {
        anim.runtimeAnimatorController = currentController[num];
    }
}
