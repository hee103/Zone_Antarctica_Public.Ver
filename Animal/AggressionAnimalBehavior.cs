using UnityEngine;

// 공격적인 동물의 추격 행동을 담당하는 클래스
public class AggressionAnimalBehavior : MonoBehaviour, IAnimalBehavior
{
    private float chasePersistenceTime = 3f; // 추격을 유지하는 시간
    private float chaseTimer; // 실제 추격이 진행된 시간

    // 추격 상태에 진입했을 때 호출(애니메이션 실행 및 추격 시간 초기화)
    public void OnEnterChase(AnimalController animal)
    {
        animal.animator.SetBool("isRun", true);
        chaseTimer = 0f;
    }

    // 추격 상태일 때 매 프레임 실행되는 함수
    public void OnChaseUpdate(AnimalController animal, StateMachine stateMachine)
    {
        float distanceToTarget = Vector3.Distance(animal.transform.position, animal.target.position); // 목표와의 거리 계산
        
        animal.nav.SetDestination(animal.target.position);  // 목표 위치를 목적지로 설정

        chaseTimer += Time.deltaTime; // 추격 시간 누적

        // 공격 범위 안에 들어왔을 경우 공격 상태로 전환
        if (distanceToTarget < animal.Data.animalAttackRange)
        {
            stateMachine.ChangeState(new AnimalAttackState(animal, stateMachine, this));
        }
        // 탐지 범위를 벗어나고 추격 유지 시간도 초과했을 경우 대기 상태로 전환
        else if (distanceToTarget > animal.Data.animalDetectRange && chaseTimer > chasePersistenceTime)
        {
            stateMachine.ChangeState(new AnimalIdleState(animal, stateMachine, this));
        }

    }

    // 추격 상태에서 벗어날 때 호출
    public void OnExitChase(AnimalController animal)
    {
        animal.nav.ResetPath(); // 경로 초기화
        animal.animator.SetBool("isRun",false); 
    }

}
