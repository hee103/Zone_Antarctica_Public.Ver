using UnityEngine;

public class AnimalChaseState : IState
{
    [SerializeField] private AnimalController animal;
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private IAnimalBehavior animalBehavior;

    public AnimalChaseState(AnimalController animal, StateMachine stateMachine, IAnimalBehavior animalBehavior)
    {
        this.animal = animal;
        this.stateMachine = stateMachine;
        this.animalBehavior = animalBehavior;
    }
    public void Enter()
    {
        animalBehavior.OnEnterChase(animal);
    }

    void IState.Update()
    {
        Debug.Log("호출");
        animalBehavior.OnChaseUpdate(animal, stateMachine);

    }

    public void Exit()
    {
        animalBehavior.OnExitChase(animal);
    }



}
