using UnityEngine;

// 몬스터가 지정 범위 내에서 무작위로 돌아다니는 상태
public class WanderState : IState
{
    [SerializeField] private MonsterController monster; // Wander 상태인 몬스터
    [SerializeField] private StateMachine stateMachine; // 상태를 관리하는 상태머신

    private float wanderTime = 10f; // Wander 상태 유지 시간
    private float timer; // 시간 계산용 타이머

    private bool movingRight = true; // 이동 방향 플래그
    private Vector3 startPos; // Wander 시작 위치

    // 몬스터와 상태머신 초기화
    public WanderState(MonsterController monster, StateMachine stateMachine)
    {
        this.monster = monster;
        this.stateMachine = stateMachine;
    }
   
    // 상태 진입 시 호출(시작 위치 저장, 애니메이션 재생, 타이머 초기화)
    public void Enter()
    {
        startPos = monster.transform.position;
        monster.animator.SetBool("isWander", true);
        timer = 0f;
    }

    // 매 프레임마다 타겟과 몬스터 사이 거리 계산 후 공격 범위 내이면 공격 상태로 전환, 감지 범위 내이면 애니메이션 재생
    public void Update()
    {
        timer += Time.deltaTime;
        float distanceToTarget = Vector3.Distance(monster.transform.position, monster.target.position);

        if (distanceToTarget < monster.Data.monsterAttackRange)
        {
            stateMachine.ChangeState(new AttackState(monster, stateMachine));
        }
        else if (distanceToTarget < monster.Data.monsterDetectRange)
        {
            Vector3 direction = (monster.target.position - monster.transform.position).normalized;
            direction.y = 0f; 

            if (direction != Vector3.zero)
            {
                monster.transform.rotation = Quaternion.LookRotation(direction);
            }
            AudioManager.Instance.PlaySFX("zombie_agressive_039",monster.transform.position);
            monster.animator.SetBool("isScream", true);
        }
        else if (timer > wanderTime)
        {
            stateMachine.ChangeState(new IdleState(monster, stateMachine));
        }

        Wandering();
        
    }

    public void Exit()
    {
        monster.animator.SetBool("isWander", false);
    }

    // 몬스터가 좌우로 왕복 이동하는 함수
    private void Wandering()
    {
        AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
        bool isScreaming = stateInfo.IsName("Zombie Scream");

        if (isScreaming)
        {
            return;
        }

        float walkSpeed = monster.Data.monsterWalkSpeed;
        Vector3 moveDir = movingRight ? Vector3.right : Vector3.left;

        // 좌우 이동
        monster.transform.position += moveDir * walkSpeed * Time.deltaTime;
        monster.transform.LookAt(monster.transform.position + moveDir); // 이동방향 바라보기

        // Wander 반경 체크 후 방향 전환
        float offset = monster.transform.position.x - startPos.x;
        if (movingRight && offset >= monster.Data.wanderRadius)
            movingRight = false;
        else if (!movingRight && offset <= -monster.Data.wanderRadius)
            movingRight = true;
    }


}
