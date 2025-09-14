public interface IState
{
    public void Enter(); // 상태 진입 시 호출
    public void Update(); // 상태가 유지되는 동안 매 프레임 호출
    public void Exit(); // 상태 종료 시 호출
    
}

public class StateMachine
{
    protected IState currentState; // 현재 활성화된 상태

    public void ChangeState(IState state)
    {
        currentState?.Exit(); // 기존 상태의 Exit() 호출
        currentState = state; 
        currentState?.Enter(); // 새 상태로 교체 후 Enter() 호출
    }

    // 현재 상태의 Update() 실행
    public void Update()
    {
        currentState?.Update();
    }

}
