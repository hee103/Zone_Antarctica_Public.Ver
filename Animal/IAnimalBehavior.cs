public interface IAnimalBehavior
{
    void OnEnterChase(AnimalController animal);
    void OnChaseUpdate(AnimalController animal, StateMachine stateMachine);
    void OnExitChase(AnimalController animal);

}
