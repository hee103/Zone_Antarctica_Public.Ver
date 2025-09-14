using UnityEngine;

// 동물이 지정 범위 내에서 무작위로 돌아다니는 상태
public class AnimalWanderState : IState
{
    [SerializeField] private AnimalController animal;        // 현재 상태의 동물 컨트롤러
    [SerializeField] private StateMachine stateMachine;      // 상태 전환을 관리하는 상태 머신
    [SerializeField] private IAnimalBehavior animalBehavior; // 동물의 행동 인터페이스

    private float wanderTime = 3f;  // 탐지 상태 유지 시간
    private float timer;            // 상태 유지 시간 측정용 타이머
    private bool movingRight = true; // 현재 이동 방향 (true = 오른쪽, false = 왼쪽)
    private Vector3 startPos;       // 배회를 시작한 기준 위치

    // 동물, 상태머신, 행동 인터페이스 연결
    public AnimalWanderState(AnimalController animal, StateMachine stateMachine, IAnimalBehavior animalBehavior)
    {
        this.animal = animal;
        this.stateMachine = stateMachine;
        this.animalBehavior = animalBehavior;
    }

    // 상태 진입 시 실행 (배회 시작 준비)
    public void Enter()
    {
        // 시작 위치 저장
        startPos = animal.transform.position;

        // 걷기 애니메이션 활성화
        animal.animator.SetBool("isWalking", true);

        // 타이머 초기화
        timer = 0f;
    }

    // 상태 업데이트 (매 프레임 실행)
    void IState.Update()
    {
        // 타이머 증가
        timer += Time.deltaTime;

        // 플레이어와의 거리 계산
        float distanceToTarget = Vector3.Distance(animal.transform.position, animal.target.position);

        // 탐지 범위 안에 플레이어가 들어오면 추격 상태로 전환
        if (distanceToTarget < animal.Data.animalDetectRange)
        {
            // 추격할 때 플레이어 방향 바라보기
            Vector3 direction = (animal.target.position - animal.transform.position).normalized;
            direction.y = 0f; // 수직 방향 제거

            if (direction != Vector3.zero)
            {
                animal.transform.rotation = Quaternion.LookRotation(direction);
            }

            stateMachine.ChangeState(new AnimalChaseState(animal, stateMachine, animalBehavior));
        }
        // 배회 시간이 끝나면 대기 상태로 전환
        else if (timer > wanderTime)
        {
            stateMachine.ChangeState(new AnimalIdleState(animal, stateMachine, animalBehavior));
        }

        // 계속 배회 동작 수행
        Wandering();
    }

    // 상태 종료 시 실행 (걷기 애니메이션 해제)
    public void Exit()
    {
        animal.animator.SetBool("isWalking", false);
    }

    // 실제 배회 이동 로직
    private void Wandering()
    {
        float walkSpeed = animal.Data.animalWalkSpeed;

        // 이동 방향 결정 (오른쪽 or 왼쪽)
        Vector3 moveDir = movingRight ? Vector3.right : Vector3.left;

        // 위치 이동
        animal.transform.position += moveDir * walkSpeed * Time.deltaTime;

        // 이동 방향 바라보게 회전
        animal.transform.LookAt(animal.transform.position + moveDir);

        // 배회 반경 초과 여부 확인
        float offset = animal.transform.position.x - startPos.x;

        // 오른쪽으로 가다가 반경 초과하면 방향 반전
        if (movingRight && offset >= animal.Data.wanderRadius)
            movingRight = false;
        // 왼쪽으로 가다가 반경 초과하면 방향 반전
        else if (!movingRight && offset <= -animal.Data.wanderRadius)
            movingRight = true;
    }
}
