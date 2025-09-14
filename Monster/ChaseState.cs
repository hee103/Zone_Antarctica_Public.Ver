using UnityEngine;
using UnityEngine.AI;

// 몬스터가 플레이어를 추적하는 상태를 나타내는 클래스
public class ChaseState : IState
{
    [SerializeField] private MonsterController monster; // 추적 상태인 몬스터
    [SerializeField] private StateMachine stateMachine; // 상태를 관리하는 상태 머신

    private float sfxTimer; // 추적 사운드 타이머
    private float sfxCoolDown = 15f; // 추적 사운드 재생 간격

    // 몬스터와 상태머신 초기화
    public ChaseState(MonsterController monster, StateMachine stateMachine)
    {
        this.monster = monster;
        this.stateMachine = stateMachine;
    }

    // 상태 진입 시 호출(애니메이션 실행)
    public void Enter()
    {
        monster.animator.SetBool("isChase", true);

    }

    // 매 프레임마다 호출되는 함수
    public void Update()
    {
        sfxTimer += Time.deltaTime; // 추적 사운드 타이머 증가

        // 쿨타임이 지나면 사운드 재생
        if (sfxTimer >= sfxCoolDown)
        {
            AudioManager.Instance.PlaySFX("zombie_hyperchase_1_loop", monster.transform.position);
            sfxTimer = 0f;
        }
        // 네비게이션 매쉬가 유효하고 경로가 유효한 경우
        if(monster.nav.isOnNavMesh && !monster.nav.isPathStale)
        {
            Vector3 destination = NavPoint(monster.target.position); // 타겟 위치를 네브매쉬 상의 유효한 위치로 변환
            monster.nav.SetDestination(monster.target.position); // 몬스터 이동 목적지 설정
            float distanceToTarget = Vector3.Distance(monster.transform.position, monster.target.position); // 타겟과 몬스터 사이 거리 계산
            
            // 공격 범위 내에 들어오면 공격 상태로 전환
            if (distanceToTarget <= monster.Data.monsterAttackRange)
            {
                stateMachine.ChangeState(new AttackState(monster, stateMachine));
            }
            // 감지 범위를 벗어나면 대기 상태로 전환
            else if (distanceToTarget > monster.Data.monsterDetectRange)
            {
                stateMachine.ChangeState(new IdleState(monster, stateMachine));
            }
        }

    }

    // 네브매쉬 상에서 유효한 위치를 반환하는 함수
    Vector3 NavPoint(Vector3 fromPosition)
    {
        NavMeshHit hit; 
        if (NavMesh.SamplePosition(fromPosition, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return monster.transform.position;
    }

    // 상태 종료 시 호출되는 함수
    public void Exit()
    {
        monster.nav.ResetPath(); // 이동 경로 초기화
        monster.animator.SetBool("isChase", false); // 추적 애니메이션 종료
    }

}
