using MonsterTarget;
using UnityEngine;
using UnityEngine.AI;

// 동물의 전체 행동, 상태, 체력 관리 등을 담담하는 컨트롤러
public class AnimalController : MonoBehaviour, IDamageable
{
    [Header("Data")]
    [SerializeField] private AnimalData originalData; // 원본 데이터
    public AnimalData Data; // 런타임에서 사용하는 실제 데이터

    // 체력 프로퍼티
    public float CurrentHP => Data.animalHP;
    public float MaxHP => Data.maxHP;

    [Header("Components")]
    public NavMeshAgent nav; 
    private Rigidbody rigid;
    public Animator animator;
    public HPUI hpUI;

    private StateMachine stateMachine;
    private AnimalTakeDamageState animalTakeDamageState;
    private IAnimalBehavior animalBehavior;

    // 두 가지 행동 패턴(우호적/ 공격적)
    [SerializeField]private FriendlyAnimalBehavior friendlyAnimalBehavior;
    [SerializeField]private AggressionAnimalBehavior aggressionAnimalBehavior;

    public Transform target { get; private set; }

    private void OnEnable()
    {
        Data = Instantiate(originalData);
        Data.animalHP = Data.maxHP;

        // 체력 UI 갱신
        RefreshHealthUI();

        // 동물 성향 결정
        if (Data.animalID == AnimalType.friendly)
        {
            animalBehavior = friendlyAnimalBehavior;
        }
        else
        {
            animalBehavior = aggressionAnimalBehavior;
        }

        // 상태 머신 초기화
        if (stateMachine == null)
        {
            stateMachine = new StateMachine();
            animalTakeDamageState = new AnimalTakeDamageState(this, stateMachine, animalBehavior);
        }

        // 초기 상태를 Idle로 설정
        stateMachine.ChangeState(new AnimalIdleState(this, stateMachine, animalBehavior));
    }

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        hpUI = GetComponentInChildren<HPUI>();

        target = PlayerObject.playerObject.transform;

        // 속도 설정
        if (Data != null)
        {
            nav.speed = Data.animalRunSpeed;
        }
    }

    // 랜덤으로 초기 상태 설정(Idle 또는 Wander)
    public void SetRandomStartState()
    {
        float randomValue = Random.value;

        if (randomValue < 0.5f)
        {
            stateMachine.ChangeState(new AnimalIdleState(this, stateMachine, animalBehavior));
        }
        else
        {
            stateMachine.ChangeState(new AnimalWanderState(this, stateMachine, animalBehavior));
        }
    }

    // 매 프레임 상태 머신 갱신 
    private void Update()
    {
        stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    // 물리 속도 고정
    private void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    // 데미지를 입었을 때 호출되는 함수
    public void ApplyDamage(float damage)
    {
        AnimalTakeDamageState newDamageState = new AnimalTakeDamageState(this, stateMachine, animalBehavior);
        newDamageState.SetDamage(damage);
        stateMachine.ChangeState(newDamageState);

    }

    // 사망 애니메이션 종료 후 실제 죽음 처리
    public void OnDeathEnd()
    {
        animalTakeDamageState.Die();
    }

    public void RefreshHealthUI()
    {
        if (hpUI != null)
        {
            hpUI.UpdateHP();
        }
    }

}
