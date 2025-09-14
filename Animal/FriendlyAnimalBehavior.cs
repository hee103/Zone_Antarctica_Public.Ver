using UnityEngine;

// 우호적 동물의 행동 로직 (플레이어가 접근하면 도망치거나 반응)
public class FriendlyAnimalBehavior : MonoBehaviour, IAnimalBehavior
{
    private float fleeDistance = 2f;    // 플레이어로부터 도망치기 시작할 거리
    private float sfxTimer;             // 사운드 재생 타이머
    private float sfxCoolDown = 7f;    // 사운드 재생 쿨다운

    //추격 상태에 진입할 때 호출
    public void OnEnterChase(AnimalController animal)
    {
        // 이동 애니메이션 활성화 (걷기)
        animal.animator.SetBool("isWalking", true);
    }

    // 추격 상태에서 매 프레임 실행되는 로직
    public void OnChaseUpdate(AnimalController animal, StateMachine stateMachine)
    {
        // 플레이어와 동물 간 거리 계산
        float distance = Vector3.Distance(animal.transform.position, animal.target.position);

        // NavMeshAgent를 사용해 플레이어 위치로 이동
        animal.nav.SetDestination(animal.target.position);

        // 플레이어가 fleeDistance 이내로 접근하면 도망치거나 특정 애니메이션/사운드 실행
        if (distance < fleeDistance)
        {
            // 사운드 재생 타이머 갱신
            sfxTimer += Time.deltaTime;
            if (sfxTimer >= sfxCoolDown)
            {
                // 동물 종류별 사운드 재생
                if (animal.Data.animalName == Animal.penguin)
                {
                    AudioManager.Instance.PlaySFX("SFX_Penguin", animal.transform.position);
                }
                else if (animal.Data.animalName == Animal.reindeer)
                {
                    AudioManager.Instance.PlaySFX("SFX_Eating_Grass", animal.transform.position);
                }
            }

            // 이동 중지
            animal.nav.ResetPath();

            animal.animator.SetBool("isWalking", false);
            animal.animator.SetBool("isShaking", true);
        }
        // 플레이어가 탐지 범위 내지만 fleeDistance보다 멀면 걷기 애니메이션 활성화
        else if (distance <= animal.Data.animalDetectRange)
        {
            Debug.Log(animal.Data.animalDetectRange);
            animal.animator.SetBool("isWalking", true);
            animal.animator.SetBool("isShaking", false);
        }
        // 플레이어가 탐지 범위를 벗어나면 Idle 상태로 전환
        else if (distance > animal.Data.animalDetectRange)
        {
            Debug.Log("idle");
            stateMachine.ChangeState(new AnimalIdleState(animal, stateMachine, this));
        }
    }

    // 추격 상태 종료 시 호출
    public void OnExitChase(AnimalController animal)
    {
        // 이동 및 애니메이션 초기화
        animal.nav.ResetPath();
        animal.animator.SetBool("isWalking", false);
        animal.animator.SetBool("isShaking", false);
    }
}
