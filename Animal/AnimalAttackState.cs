using UnityEngine;

// 동물이 공격 상태일 때 동작을 정의하는 클래스
public class AnimalAttackState : IState
{
    [SerializeField] private AnimalController animal; // 현재 동물
    [SerializeField] private StateMachine stateMachine; // 상태 머신
    [SerializeField] private PlayerCondition player; // 공격 대상 플레이어 상태
    [SerializeField] private IAnimalBehavior animalBehavior; // 동물의 행동 전략

    private float attackTimer; // 공격 쿨타임
    private float sfxTimer; // 효과음 재생 간격 타이머
    private float sfxCoolDown = 7f; // 효과음을 너무 자주 재생하지 않도록 하는 쿨타임

    //공격 상태가 시작될 때 필요한 값 할당
    public AnimalAttackState(AnimalController animal, StateMachine stateMachine, IAnimalBehavior animalBehavior)
    {
        this.animal = animal;
        this.stateMachine = stateMachine;
        this.animalBehavior = animalBehavior;
    }

    // 공격 상태 진입 시 호출(대상 상태 컴포넌트 가져오기, 공격 타이머 초기화)
    public void Enter()
    {
        player = animal.target.GetComponent<PlayerCondition>(); 
        attackTimer = 0f;
    }

    // 공격 상태에서 매 프레임마다 실행되는 로직
    public void Update()
    {
        float distanceToTarget = Vector3.Distance(animal.transform.position, animal.target.position); // 목표와의 거리 계산

        // 목표가 공격 범위를 벗어니면 추격 상태로 전환
        if (distanceToTarget > animal.Data.animalAttackRange)
        {
            animal.nav.isStopped = false; 

            stateMachine.ChangeState(new AnimalChaseState(animal, stateMachine, animalBehavior));
            return;
        }
        // 공격 범위 안에 있으면 공격 준비
        else if (distanceToTarget <= animal.Data.animalAttackRange)
        {
            animal.animator.SetBool("isAttack", true);
            animal.nav.isStopped = true;
        }

        // 시간 누적
        attackTimer += Time.deltaTime; 
        sfxTimer += Time.deltaTime;

        // 공격 쿨타임이 지났다면 공격 실행
        if (attackTimer >= animal.Data.animalAttackCoolDown)
        {
            Attack();
            attackTimer = 0f; // 다시 쿨타임 초기화
        }
    }

    // 실제 공격 동작 수행
    void Attack()
    {
        // 일정 시간마다 효과음 재생
        if (sfxTimer >= sfxCoolDown)
        {
            if (animal.Data.animalName == Animal.bear)
            {
                AudioManager.Instance.PlaySFX("SFX_Bear_Growl_2", animal.transform.position);
            }
            else if (animal.Data.animalName == Animal.bear)
            {
                AudioManager.Instance.PlaySFX("SFX_Bear_Calm", animal.transform.position);
            }
        }

        animal.animator.SetTrigger("Attack"); // 공격 애니메이션 트리거 실행

        // 플레이어가 존재하고 체력이 남아 있다면 피해 익힘
        if (player != null && player.health >= 0)
        {
            player.health -= animal.Data.animalPower;// 체력 감소

            player.infection += 5f; // 감염 수치 증가
        }
    }

    // 공격 상태 종료 시 호출
    public void Exit()
    {
        animal.animator.SetBool("isAttack", false);
    }



}
