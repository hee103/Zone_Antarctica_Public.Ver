using UnityEngine;

// 몬스터의 공격을 나타내는 클래스
public class AttackState : IState
{
    [SerializeField] private MonsterController monster; // 해당 상태를 가진 몬스터
    [SerializeField] private StateMachine stateMachine;// 상태를 관리하는 상태머신
    [SerializeField] private PlayerCondition player; // 공격 대상 플레이어
    private float attackTimer; // 공격 쿨타임 계산용 타이머


    // 몬스터와 상태머신을 받아 초기화
    public AttackState(MonsterController monster, StateMachine stateMachine)
    {
        this.monster = monster;
        this.stateMachine = stateMachine;
    }

    // 공격 상태로 집입 했을 때 호출(사운드 재생, 타켓 플레이어 컴포넌트, 공격 타이머 초기화)
    public void Enter()
    {
        AudioManager.Instance.PlaySFX("zombie_agressive_044", monster.transform.position);
        player = monster.target.GetComponent<PlayerCondition>();
        attackTimer = 0f;
    }

    // 매 프레임마다 호출되는 함수
    public void Update()
    {
        // 몬스터와 타겟 사이의 거리 계산
        float distanceToTarget = Vector3.Distance(monster.transform.position, monster.target.position);

        // 공격 범위를 벗어나면 추적 상태로 전환
        if (distanceToTarget > monster.Data.monsterAttackRange)
        {
            monster.nav.isStopped = false; // 이동 가능 상태로 변경
            stateMachine.ChangeState(new ChaseState(monster, stateMachine)); // 추적 상태로 변경
            return;
        }
        // 공격 범위 내에 있으면 공격 애니메이션 실행
        else if (distanceToTarget <= monster.Data.monsterAttackRange)
        {
            monster.animator.SetBool("isAttack", true); // 공격 애니메이션 시작
            monster.nav.isStopped = true; // 이동 정지

        }

        // 공격 쿨타임 계산
        attackTimer += Time.deltaTime;

        // 쿨타임이 지나면 공격 실행
        if (attackTimer >= monster.Data.attackCoolDown)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    // 실제 공격이 수행되는 함수
    void Attack()
    {
        monster.animator.SetTrigger("Attack"); // 공격 애니메이션 트리거

        // 플레이어에게 공격 데미지 적용
        if (player != null && player.health >= 0)
        {
            player.health -= monster.Data.monsterPower;

            player.infection += 5f;
        }
    }

    // 상태 종료 시 호출
    public void Exit()
    {
        monster.animator.SetBool("isAttack", false);
    }
}
