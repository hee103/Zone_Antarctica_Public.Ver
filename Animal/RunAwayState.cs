using UnityEngine;
using UnityEngine.AI;

// 우호적 동물이 플레이어에게서 도망가는 상태
public class RunAwayState : IState
{
    [SerializeField] private AnimalController animal;       // 현재 상태의 동물 컨트롤러
    [SerializeField] private StateMachine stateMachine;     // 상태 전환 관리용 상태 머신
    [SerializeField] private IAnimalBehavior animalBehavior; // 동물 행동 인터페이스

    private float runTime = 3f; // 도망 상태 유지 시간
    private float timer;        // 상태 경과 시간 측정용 타이머

    // 동물, 상태머신, 행동 인터페이스 연결
    public RunAwayState(AnimalController animal, StateMachine stateMachine, IAnimalBehavior animalBehavior)
    {
        this.animal = animal;
        this.stateMachine = stateMachine;
        this.animalBehavior = animalBehavior;
    }

    // 상태 진입 시 실행
    public void Enter()
    {
        timer = 0f; // 타이머 초기화

        // 도망가는 애니메이션 활성화
        animal.animator.SetBool("isRun", true);

        // 플레이어로부터 도망가는 방향 계산
        Vector3 fleeDir = (animal.transform.position - animal.target.position).normalized;

        // 목표 도망 위치 계산 
        Vector3 runDestination = animal.transform.position + fleeDir * 5f;

        // NavMesh 상의 유효한 위치 확인
        NavMeshHit hit;
        if (NavMesh.SamplePosition(runDestination, out hit, 5f, NavMesh.AllAreas))
        {
            // NavMeshAgent로 도망 위치 설정
            animal.nav.SetDestination(hit.position);
        }
    }

    //상태 업데이트 (매 프레임 호출)
    public void Update()
    {
        timer += Time.deltaTime; // 시간 누적

        // 도망 시간이 끝나면 Idle 상태로 전환
        if (timer > runTime)
        {
            stateMachine.ChangeState(new AnimalIdleState(animal, stateMachine, animalBehavior));
        }
    }

    // 상태 종료 시 실행
    public void Exit()
    {
        // 이동 경로 초기화
        animal.nav.ResetPath();

        // 도망 애니메이션 종료
        animal.animator.SetBool("isRun", false);
    }
}
