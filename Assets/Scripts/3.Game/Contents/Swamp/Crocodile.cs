using UnityEngine;

[RequireComponent(typeof(Platform))]
public class Crocodile : MonoBehaviour, IMouseHover
{
    [Header("Normal State")]
    public float moveSpeed = 1.0f;            // 평상시 이동속도
    public float normalMoveDuration = 1f;     // 평상시 이동 시간
    public float standby = 1.0f;              // 평상시 대기 시간

    [Header("Hit Reaction")]
    public float speedOnHit = 2.0f;           // 피격 시 이동속도
    public float hitMoveDuration = 1.0f;      // 피격 이동 시간
    public float stun = 3.0f;                 // 피격 후 스턴 시간

    [SerializeField] Transform sprites;
    [SerializeField] Transform headHitbox;
    [SerializeField] Transform tailHitbox;

    [Header("MoveArea")]
    [SerializeField] Collider movableArea;

    Rigidbody rigid;
    bool kinematicState;
    LayerMask playerLayer;
    Platform platform;

    enum State
    {
        NormalMove,
        Standby,
        HitMove,
        Stun
    }

    State state;
    float stateTimer;

    Vector3 currentDir;
    float currentSpeed;

    Vector3 headBaseLocalPos;
    Vector3 tailBaseLocalPos;
    bool facingRight;

    public void OnHit(Crocodile_part.Part part)
    {
        Vector3 originalDir = currentDir;
        currentDir = (part == Crocodile_part.Part.head) ? -originalDir : originalDir;

        UpdateFacingByMoveDir(currentDir);

        state = State.HitMove;
        stateTimer = hitMoveDuration;
        currentSpeed = speedOnHit;
    }

    void Awake()
    {
        headBaseLocalPos = headHitbox.localPosition;
        tailBaseLocalPos = tailHitbox.localPosition;

        SetFacingRight(false); // false=왼쪽, true=오른쪽

        rigid = GetComponent<Rigidbody>();
        platform = GetComponent<Platform>();
    }

    void Start()
    {
        EnterNormalMove();
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime; 
        SetKinematic(platform.IsAboard);
        if (platform.IsAboard) return;
        switch (state)
        {
            case State.NormalMove:
                MoveFrame(dt);
                TickStateTimer(dt, EnterStandby);
                break;

            case State.Standby:
                TickStateTimer(dt, EnterNormalMove);
                break;

            case State.HitMove:
                MoveFrame(dt);
                TickStateTimer(dt, EnterStun);
                break;

            case State.Stun:
                TickStateTimer(dt, EnterNormalMove);
                break;
        }
    }


    void EnterNormalMove()
    {
        PickRandomDirection();
        state = State.NormalMove;
        stateTimer = normalMoveDuration;
        currentSpeed = moveSpeed;
    }

    void EnterStandby()
    {
        state = State.Standby;
        stateTimer = standby;
        currentSpeed = 0f;
    }

    void EnterStun()
    {
        state = State.Stun;
        stateTimer = stun;
        currentSpeed = 0f;
    }

    void MoveFrame(float dt)
    {
        if (currentSpeed <= 0f) return;

        Vector3 desiredMove = currentDir * currentSpeed;
        Vector3 nextPos = rigid.position + desiredMove * dt;

        nextPos = movableArea.ClosestPoint(nextPos);

        rigid.MovePosition(nextPos);
    }

    void TickStateTimer(float dt, System.Action onElapsed)
    {
        stateTimer -= dt;
        if (stateTimer <= 0f)
            onElapsed?.Invoke();
    }

    void PickRandomDirection()
    {
        int r = Random.Range(0, 4);
        currentDir = r switch
        {
            0 => Vector3.left,
            1 => Vector3.forward,
            2 => Vector3.right,
            3 => Vector3.back,
            _ => Vector3.forward,
        };

        UpdateFacingByMoveDir(currentDir);
    }

    void SetFacingRight(bool value)
    {
        if (facingRight == value) return;
        facingRight = value;

        float flag = facingRight ? -1f : 1f;

        headHitbox.localPosition = new Vector3(Mathf.Abs(headBaseLocalPos.x) * -flag, headBaseLocalPos.y, headBaseLocalPos.z);
        tailHitbox.localPosition = new Vector3(Mathf.Abs(tailBaseLocalPos.x) * flag, tailBaseLocalPos.y, tailBaseLocalPos.z);

        if (sprites != null)
        {
            var s = sprites.localScale;
            s.x = facingRight ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            sprites.localScale = s;
        }
    }

    void UpdateFacingByMoveDir(Vector3 dir)
    {
        if (dir.x > 0f) SetFacingRight(true);
        else if (dir.x < 0f) SetFacingRight(false);
    }

    public void OnHoverEnter()
    {
        if (GamesceneManager.Instance.isNight) return;

        SwampStone_Controller.Instance.CanThrowStone(true);

        GameSceneUI.Instance.CursorChange(CursorType.Special);
    }

    public void OnHoverStay() { }

    public void OnHoverExit()
    {

        SwampStone_Controller.Instance.CanThrowStone(false);

        if (!GamesceneManager.Instance.isNight)
            GameSceneUI.Instance.CursorChange(CursorType.Normal);
        else
            GameSceneUI.Instance.CursorChange(CursorType.Attack);
    }

    void SetKinematic(bool value)
    {
        if (kinematicState == value)
            return;

        kinematicState = value;

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.isKinematic = value;
    }
}
