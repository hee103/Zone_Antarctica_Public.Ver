using UnityEngine;

// 동물이 피해를 입었을 때를 담당하는 상태
public class AnimalTakeDamageState : IState
{
    [SerializeField] private AnimalController animal;       // 현재 상태에 속한 동물
    [SerializeField] private StateMachine stateMachine;     // 상태 전환을 관리하는 상태머신
    [SerializeField] private IAnimalBehavior animalBehavior; // 동물 행동 인터페이스
    private float damage;                                   // 이번에 입을 피해량

    //상태가 생성될 때 컨트롤러, 상태머신, 행동을 받아 초기화
    public AnimalTakeDamageState(AnimalController animal, StateMachine stateMachine, IAnimalBehavior animalBehavior)
    {
        this.animal = animal;
        this.stateMachine = stateMachine;
        this.animalBehavior = animalBehavior;
    }

    // 외부에서 이번에 입힐 데미지를 설정하는 메서드
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    // 상태 진입 시 실행되는 로직
    public void Enter()
    {
        // 체력 감소
        animal.Data.animalHP -= damage;

        // 체력 UI 갱신
        animal.RefreshHealthUI();

        // 체력이 0 이하라면 사망 처리
        if (animal.Data.animalHP <= 0f)
        {
            animal.animator.SetTrigger("isDie");
        }
        else
        {
            // 동물 타입에 따라 다음 행동 결정
            if (animal.Data.animalID == AnimalType.friendly)
            {
                // 우호적인 동물 → 맞으면 도망감
                stateMachine.ChangeState(new RunAwayState(animal, stateMachine, animalBehavior));
            }
            else if (animal.Data.animalID == AnimalType.aggression)
            {
                // 공격적인 동물 → 맞으면 추격 상태로 전환
                Debug.Log("데미지를 받고 추격 상태로 전환");
                animal.animator.SetBool("isRun", true);
                stateMachine.ChangeState(new AnimalChaseState(animal, stateMachine, animalBehavior));
            }
        }
    }

    public void Update()
    {
        // TakeDamage 상태에서는 매 프레임 별도 로직 없음
    }

    public void Exit()
    {
        // 상태 종료 시 특별한 처리 없음
    }

    //동물 사망 시 호출되는 메서드
    public void Die()
    {
        // 아이템 드롭 위치 (동물 위치 + 약간 위)
        Vector3 dropPos = animal.transform.position + Vector3.up * 0.5f;

        // 아이템 드롭 실행
        DropItemManager.Instance.DropItem(dropPos);

        // 동물 오브젝트를 풀로 반환 (비활성화 후 재사용 가능)
        AnimalPoolManager.Instance.ReturnAnimal(animal.gameObject);
    }
}
