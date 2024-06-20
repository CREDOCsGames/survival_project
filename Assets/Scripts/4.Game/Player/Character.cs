using System.Collections;
using UnityEngine;
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
    [SerializeField] ParticleSystem particle;
    [SerializeField] GameObject gardianAngel;
    [SerializeField] GameObject gardianEffect;

    [SerializeField] Slider playerHpBar;
    [SerializeField] Slider shielBar;

    [Header("Stat")]
    [SerializeField] public int characterNum;
    public int level;
    [SerializeField] public float maxExp;
    [SerializeField] public CharacterInfo[] characterInfos;
    //[SerializeField] public CharacterInfo[] characterInfos2;

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
    [HideInInspector] public float speed;

    [Header("Summon")]
    [SerializeField] GameObject ggoGgoPrefab;
    [SerializeField] GameObject ilsoonPrefab;
    [SerializeField] GameObject wakgoodPrefab;
    [SerializeField]  public Transform[] summonPos;
    [HideInInspector] public int summonNum;

    Animator anim;
    Collider ground;

    bool isRun, isAttacked = false;
    bool isAvoid = false;
    [HideInInspector] public bool isDead = false;

    GameManager gameManager;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public Vector3 charDir => dir;
    [HideInInspector] public float x;
    [HideInInspector] public float z;

    float recoverTime;

    [HideInInspector] public int levelUpCount;

    [HideInInspector] public bool isBuff = false;
    [HideInInspector] public float charBuffDmg = 0;
    [HideInInspector] public float buffTime = 5;

    [HideInInspector] public int thunderCount;

    Coroutine currentCoroutine;

    [HideInInspector] public float shield = 0;

    [HideInInspector] public CharacterInfo currentCharacterInfo;

    public Transform weaponParent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        particle.GetComponentInChildren<Renderer>().enabled = false;
        gameObject.GetComponent<SpecialAttack>().enabled = false;

        gameManager = GameManager.Instance;
        transform.position = new Vector3(0f, 0f, -40f);

        thunderMark.transform.localScale = new Vector3(Mathf.Clamp(4f + gameManager.range * 0.5f, 1, 12), Mathf.Clamp(4f + gameManager.range * 0.5f, 1, 12), 0);
        gardianAngel.SetActive(false);
        gardianEffect.SetActive(false);

        CharacterSetting(characterNum);
        maxExp = 10;
        level = 1;
        levelUpCount = 0;
        recoverTime = 1;
        dashCoolTime = 4;
        dashCount = gameManager.dashCount;
        initDashCoolTime = dashCoolTime;

        gameObject.SetActive(false);
    }

    public void CharacterSetting(int num)
    {
        StatChangeCheck(num);

        anim.runtimeAnimatorController = characterInfos[characterNum].CharacterAnim;
        gameManager.stats[0] = Mathf.Round(gameManager.stats[0] * characterInfos[characterNum].HpRate);
        maxHp = gameManager.stats[0];
        currentHp = maxHp;
        gameManager.stats[9] += characterInfos[characterNum].CharacterSpeed;
        gameManager.stats[13] += characterInfos[characterNum].DamageRatio;
        gameManager.stats[14] += characterInfos[characterNum].Avoid;
    }

    void StatChangeCheck(int num)
    {
        int beforeNum = characterNum;
        characterNum = num;
        currentCharacterInfo = characterInfos[characterNum];

        if (beforeNum == characterNum)
            return;

        gameManager.stats[0] = Mathf.Round(gameManager.stats[0] / characterInfos[beforeNum].HpRate);
        gameManager.stats[9] -= characterInfos[beforeNum].CharacterSpeed;
        gameManager.stats[13] -= characterInfos[beforeNum].DamageRatio;
        gameManager.stats[14] -= characterInfos[beforeNum].Avoid;
    }

    void Update()
    {
        if (exp >= maxExp)
        {
            SoundManager.Instance.PlayES("LevelUp");
            level++;
            levelUpCount++;
            gameManager.stats[0] += 1;
            if (gameManager.maxHp > 1)
                maxHp = gameManager.maxHp;

            else
                maxHp = 1f;
            exp = exp - maxExp;
            maxExp = 10 * level;
        }

        if (gameManager.revive)
            gardianAngel.SetActive(true);

        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            HpSetting();

            //isRun = false;

            if (currentHp > 0 && (!gameManager.isClear || !gameManager.isBossDead))
            {
                //Move();
                Dash();
                AutoRecoverHp();
            }

            else if (gameManager.isClear && gameManager.isBossDead)
            {
                isBuff = false;
                buffTime = 5;
            }

            /*anim.SetFloat("moveSpeed", 1 + (speed * 0.1f));
            anim.SetBool("isRun", isRun);*/
        }

        SummonPet();
    }

    private void FixedUpdate()
    {
        if (gameManager.currentScene == "Game" && !gameManager.isPause)
        {
            isRun = false;

            if (currentHp > 0 && (!gameManager.isClear || !gameManager.isBossDead))
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
        shielBar.value = (shield / 10f);
    }

    void SummonPet()
    {
        if (gameManager.ggoGgoSummon)
        {
            GameObject summon = Instantiate(ggoGgoPrefab);
            summon.transform.position = summonPos[summonNum].position;
            summon.GetComponent<Chick>().summonPosNum = summonNum;
            summon.transform.SetParent(gameManager.transform);
            summonNum++;
            gameManager.passiveBoolVariables[5] = false;
        }

        else if (gameManager.ilsoonSummon)
        {
            GameObject summon = Instantiate(ilsoonPrefab);
            summon.transform.position = summonPos[summonNum].position;
            summon.transform.SetParent(gameManager.transform);
            summonNum++;
            gameManager.passiveBoolVariables[6] = false;
        }

        else if (gameManager.wakgoodSummon)
        {
            GameObject summon = Instantiate(wakgoodPrefab);
            summon.transform.position = summonPos[summonNum].position;
            summon.transform.SetParent(gameManager.transform);
            summonNum++;
            gameManager.passiveBoolVariables[7] = false;
        }

        if (summonNum >= 3)
            summonNum = 0;
    }

    void OnBuff()
    {
        if (gameManager.buffNum == 1)
        {
            if (gameManager.speed <= 1f)
                speed = 1f;

            else
                speed = gameManager.speed;

            gameManager.attackSpeed = gameManager.stats[8] + 5f;
            gameManager.percentDamage = gameManager.stats[13];
        }

        else if (gameManager.buffNum == 2)
        {
            if (gameManager.speed <= 1f)
                speed = 1f + 2f;

            else
                speed = gameManager.speed + 2f;

            gameManager.attackSpeed = gameManager.stats[8];
            gameManager.percentDamage = gameManager.stats[13];
        }

        else if (gameManager.buffNum == 3)
        {
            if (gameManager.speed <= 1f)
                speed = 1f;

            else
                speed = gameManager.speed;

            gameManager.attackSpeed = gameManager.stats[8];
            gameManager.percentDamage = gameManager.stats[13] + 0.5f;
        }
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
        if (gameManager.dashCount > 0)
        {
            Vector3 beforePos;
            Vector3 afterPos;

            if (dashCount > 0)
            {
                if (rend.flipX == true)
                    particle.transform.localScale = new Vector3(-1, 1, 1);

                else if (rend.flipX == false)
                    particle.transform.localScale = new Vector3(1, 1, 1);

                if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Key_Dash")))
                {
                    particle.GetComponentInChildren<Renderer>().enabled = true;

                    beforePos = transform.position;

                    if (x == 0 && z == 0)
                        afterPos = new Vector3(transform.position.x + 2, 0, transform.position.z);

                    else
                        afterPos = transform.position + new Vector3(x, 0, z) * 4;

                    if (ground == null)
                        ground = GameSceneUI.Instance.ground;

                    else
                        afterPos = ground.bounds.ClosestPoint(afterPos);

                    transform.position = Vector3.Lerp(beforePos, afterPos, 1);

                    dashCount--;
                    Invoke("ParticleOff", 0.4f);

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

    void ParticleOff()
    {
        particle.GetComponentInChildren<Renderer>().enabled = false;
    }

    public void Equip()
    {
        int count = ItemManager.Instance.weaponCount;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].GetComponent<Weapon>().weaponInfo.WeaponName == ItemManager.Instance.storedWeapon[count].WeaponName)
               Instantiate(weapons[i], weaponPoses[count]);
        }
    }

    public void ReleaseEquip(int num)
    {
        Destroy(weaponPoses[num].GetChild(0).gameObject);
    }

    public bool CheckEquipWeapon(int num)
    {
        for(int i = 0; i< weaponPoses.Length;i++)
        {
            if (weaponPoses[i].childCount != 0 && (int)ItemManager.Instance.storedWeapon[i].Type == num)
                return true;
        }

        return false;
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
                rend.flipX = true;

                if (!isLeftRight && Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")))
                {
                    x = 1;
                    rend.flipX = false;
                }
            }

            else if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("Key_Right")))
            {
                x = 1;
                rend.flipX = false;
                isLeftRight = true;
            }

            if (Input.GetKeyUp((KeyCode)PlayerPrefs.GetInt("Key_Right")))
                isLeftRight = false;
        }

        if (!isBuff)
        {
            if (gameManager.speed <= 1)
                speed = 1;

            else
                speed = gameManager.speed;
        }

        else if (isBuff)
        {
            OnBuff();
            buffTime -= Time.deltaTime;

            if (buffTime <= 0)
            {
                isBuff = false;
                buffTime = 5;
            }
        }

        dir = (Vector3.right * x + Vector3.forward * z).normalized;

        transform.position += dir * speed * Time.deltaTime;
        //rigid.MovePosition(transform.position + dir * speed * Time.deltaTime);

        if (ground == null)
            ground = GameSceneUI.Instance.ground;

        else
            transform.position = ground.bounds.ClosestPoint(transform.position);

        if (dir != Vector3.zero)
            isRun = true;

        else if (dir == Vector3.zero)
            isRun = false;
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
        if (!gameManager.revive)
        {
            WeaponParent.SetActive(false);
            currentHp = 0;
            isDead = true;
            isAttacked = true;

            anim.SetBool("isDead", isDead);

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerDie"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    gameObject.SetActive(false);
            }
        }

        else if (gameManager.revive)
        {
            isAttacked = true;
            isRun = false;

            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(OnRevive());
        }
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

    IEnumerator OnRevive()
    {
        rend.color = new Color(1, 1, 1, 0.5f);
        gardianEffect.SetActive(true);

        yield return new WaitForSeconds(2.5f);
        gameManager.passiveBoolVariables[4] = false;    // gameManager.revive = false
        currentHp = Mathf.Ceil(maxHp * 0.5f);
        gardianAngel.SetActive(false);

        yield return new WaitForSeconds(2f);
        rend.color = Color.white;
        isAttacked = false;
    }
}
