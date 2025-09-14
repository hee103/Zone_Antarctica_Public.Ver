using UnityEngine;

// 동물이 아무 행동도 하지 않고 대기(Idle)하는 상태
public class AnimalIdleState : IState
{
    [SerializeField] private AnimalController animal;         // 현재 동물 컨트롤러
    [SerializeField] private StateMachine stateMachine;       // 상태 머신
    [SerializeField] private IAnimalBehavior animalBehavior;  // 동물 행동(공격형/우호형)

    private float idleTime = 5f; // Idle 상태로 머무를 최대 시간
    private float timer;         // Idle 상태에 머문 시간 누적

    // 생성자: 필요한 컨트롤러/상태머신/행동 전략 주입
    public AnimalIdleState(AnimalController animal, StateMachine stateMachine, IAnimalBehavior animalBehavior)
    {
        this.animal = animal;
        this.stateMachine = stateMachine;
        this.animalBehavior = animalBehavior;
    }

    // Idle 상태에 진입할 때 호출
    public void Enter()
    {
        timer = 0f; // 대기 시간 초기화
        animal.animator.SetBool("isIdle", true); // Idle 애니메이션 실행
    }

    // Idle 상태일 때 매 프레임마다 실행
    void IState.Update()
    {
        timer += Time.deltaTime; // Idle 유지 시간 누적

        // 플레이어와의 거리 계산
        float distanceToTarget = Vector3.Distance(animal.transform.position, animal.target.position);

        // 플레이어가 탐지 범위 안에 들어오면 → 추격 상태로 전환
        if (distanceToTarget < animal.Data.animalDetectRange)
        {
            stateMachine.ChangeState(new AnimalChaseState(animal, stateMachine, animalBehavior));
        }
        // 탐지 대상이 없고 Idle 유지 시간이 idleTime을 초과하면 → 배회(Wander) 상태로 전환
        else if (timer > idleTime)
        {
            stateMachine.ChangeState(new AnimalWanderState(animal, stateMachine, animalBehavior));
        }
    }

    // Idle 상태에서 벗어날 때 호출
    public void Exit()
    {
        animal.animator.SetBool("isIdle", false); // Idle 애니메이션 종료
    }
}
