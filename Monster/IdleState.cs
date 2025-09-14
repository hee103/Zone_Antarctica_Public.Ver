using UnityEngine;

// 몬스터가 아무 행동도 하지 않고 대기하는 상태를 나타내는 클래스
public class IdleState : IState
{
    [SerializeField] private MonsterController monster; // 대기 상태인 몬스터
    [SerializeField] private StateMachine stateMachine; // 상태를 관리하는 상태머신

    private float idleTime = 10f; // 일정 시간 대기 후 이동 상태 전환
    private float timer; // 대기 시간 계산용 타이머
    private float sfxTimer; // 대기 사운드 타이머
    private float sfxCoolDown = 2f; // 대기 사운드 재생 간격

    // 몬스터와 상태머신 초기화
    public IdleState(MonsterController monster, StateMachine stateMachine)
    {
        this.monster = monster;
        this.stateMachine = stateMachine;
    }

    // 상태 진입 시 호출(애니메이션 재생, 타이머 초기화)
    public void Enter()
    {
        monster.animator.SetBool("isIdle", true);
        timer = 0f;
    }  
    
    // 매프레임마다 호출되는 업데이트
    public void Update()
    {
        sfxTimer += Time.deltaTime;// 대기 사운드 타이머 증가

        // 쿨타임이 지나면 대기 사운드 재생
        if (sfxTimer >= sfxCoolDown)
        {
            AudioManager.Instance.PlaySFX("zombie_growl_023", monster.transform.position);
            sfxTimer = 0f;
        }
        timer += Time.deltaTime; // 대기 시간 계산

        if(monster != null)
        {
            float distanceToTarget = Vector3.Distance(monster.transform.position, monster.target.position); // 몬스터와 타겟 사이 거리 계산
            AnimatorStateInfo animState = monster.animator.GetCurrentAnimatorStateInfo(0); // 현재 애니메이션 상태 가져오기

            // 공격 범위 내면 공격 상태로 전환
            if (distanceToTarget < monster.Data.monsterAttackRange)
            {
                stateMachine.ChangeState(new AttackState(monster, stateMachine));
            }
            // 감지 범위 내이면 준비 또는 추적 상태로 전환
            else if (distanceToTarget <= monster.Data.monsterDetectRange)
            {
                Vector3 direction = (monster.target.position - monster.transform.position).normalized; // 타겟을 바라보도록 회전
                direction.y = 0f;

                if (direction != Vector3.zero)
                {
                    monster.transform.rotation = Quaternion.LookRotation(direction);
                }

                // 아직 소리 지르지 않았으면 소리 지르기
                if (!monster.hasScreamed)
                {
                    monster.animator.SetBool("isScream", true);
                    monster.isScreaming = true;
                }
                else
                {
                    stateMachine.ChangeState(new ChaseState(monster, stateMachine)); // 이미 소리 지른 후면 추적 상태로 전환
                }

            }
            // 일정 시간 동안 대기 후 특정 몬스터일 경우 wander 상태로 전환
            else if (timer > idleTime && monster.Data.monsterID == MonsterType.Monster1_Base)
            {
                stateMachine.ChangeState(new WanderState(monster, stateMachine));
            }
        }
        
    }

    // 상태 종료 시 호출(대기 애니메이션 종료)
    public void Exit()
    {
        monster.animator.SetBool("isIdle", false);
    }

}


