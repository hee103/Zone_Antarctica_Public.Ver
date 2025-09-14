using UnityEngine;

// 몬스터가 데미지 받는 상태를 나타내는 클래스
public class TakeDamageState : IState
{
    [SerializeField] private MonsterController monster; // 해당 상태를 가진 몬스터
    [SerializeField] private StateMachine stateMachine; // 상태를 관리하는 상태머신
    private float damage; // 공격으로 입은 피해량

    // 몬스터와 상태머신 초기화
    public TakeDamageState(MonsterController monster, StateMachine stateMachine)
    {
        this.monster = monster;
        this.stateMachine = stateMachine;
    }
    
    // 외부에서 피해량 설정(Player)
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    // 상태 진입 시 호출(사운드 재생, 애니메이션 실행, 체력 감소 및 UI 갱신)
    public void Enter()
    {
        AudioManager.Instance.PlaySFX("zombie_grunt_006", monster.transform.position);
        monster.animator.SetBool("isDamage", true);
        monster.Data.monsterHP -= damage;
        monster.RefreshHealthUI();
        if (monster.Data.monsterHP <= 0f)
        {
            AudioManager.Instance.PlaySFX("zombie_death_004", monster.transform.position);
            monster.animator.SetTrigger("isDie");
        }

    }

    public void Update() 
    {
    }
    
    public void Exit() 
    {
    }

    // 몬스터 사망을 처리하는 함수
    public void Die()
    {
        Vector3 dropPos = monster.transform.position + Vector3.up * 0.5f; // 아이템 드롭 위치 계산
        DropItemManager.Instance.DropItem(dropPos); // 아이템 드롭
        MonsterPoolManager.Instance.ReturnMonster(monster.gameObject); // 몬스터 객체 반환
    }

}
