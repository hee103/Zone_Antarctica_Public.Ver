using MonsterTarget;
using UnityEngine;
using UnityEngine.AI;

// 몬스터의 전체 동작을 관리하는 컨트롤러 IDamageable 인터페이스를 구현하여 피해를 받을 수 있음
public class MonsterController : MonoBehaviour, IDamageable
{
    [Header("Data")]
    [SerializeField] private MonsterData originalData; // 원본 몬스터 데이터
    public MonsterData Data; // 인스턴스로 사용할 몬스터 데이터

    public float CurrentHP => Data.monsterHP; // 현재 체력
    public float MaxHP => Data.maxHP; // 최대 체력

    [Header("Components")]
    public NavMeshAgent nav; 
    private Rigidbody rigid;
    public Animator animator;
    public HPUI hpUI;

    private StateMachine stateMachine; // 상태 관리
    private TakeDamageState takeDamageState; // 피해 상태
    public bool hasScreamed = false; // 한 번 소리 지름 여부
    public bool isScreaming; // 현재 소리 지르는 애니메이션이 재생 중인지
    public Transform target { get; private set; } 

    // 오브젝트 활성화 시 초기화
    private void OnEnable()
    {
        Data = Instantiate(originalData);  
        Data.monsterHP = Data.maxHP; // 체력 초기화

        RefreshHealthUI(); // 체력 UI 갱신

        // 상태머신과 피해 상태 초기화
        if (stateMachine == null)
        {
            stateMachine = new StateMachine();
            takeDamageState = new TakeDamageState(this, stateMachine);
        }

        // 초가 상태를 Idle 상태로 설정
        stateMachine.ChangeState(new IdleState(this, stateMachine));
    }

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hpUI = GetComponentInChildren<HPUI>();

        target = PlayerObject.playerObject.transform; // 플레이어 타겟 설정

        if (Data != null)
        {
            nav.speed = Data.monsterRunSpeed;
        }
    }

    // 랜덤으로 초기 상태 설정(Idle 또는 Wander)
    public void SetRandomStartState()
    {
        float randomValue = Random.value;

        if (randomValue < 0.5f && Data.monsterID == MonsterType.Monster1_Base)
        {
            stateMachine.ChangeState(new IdleState(this, stateMachine));
        }
        else if(randomValue > 0.5f && Data.monsterID == MonsterType.Monster1_Base)
        {
            stateMachine.ChangeState(new WanderState(this, stateMachine));
        }
    }

    // 매 프레임마다 상태 업데이트
    private void Update()
    {
        stateMachine?.Update();
    }

    // 물리 연산마다 호출
    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    // 물리적 움직임을 고정
    private void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    // 외부에서 피해를 입을 때 호출
    public void ApplyDamage(float damage)
    {
        takeDamageState.SetDamage(damage); // 피해량 설정
        stateMachine.ChangeState(takeDamageState); // 피해 상태로 전환
    }

    // 체력 UI 갱신
    public void RefreshHealthUI()
    {
        if (hpUI != null)
        {
            hpUI.UpdateHP();
        }
    }

    // 소리 지르기 애니메이션 종료 시 호출
    public void OnScreamEnd()
    {
        isScreaming = false;
        animator.SetBool("isScream", false);
        hasScreamed = true;

        // 소리 지른 후 추적 상태로 전환
        stateMachine.ChangeState(new ChaseState(this, stateMachine));
    }

    // 사망 애니메이션 종료 시 호출
    public void OnDeathEnd()
    {
        takeDamageState.Die(); // 몬스터 제거 및 아이템 드롭
    }

    // 피해 애니메이션 종료 시 호출
    public void OnDamageEnd()
    {
        animator.SetBool("isDamage", false);
        stateMachine.ChangeState(new IdleState(this, stateMachine));
    }
}
