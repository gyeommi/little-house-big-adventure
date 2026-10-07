
public abstract class BaseState : IState
{
    protected BaseCharacterController controller;
    protected StateMachine stateMachine;

    protected BaseState(BaseCharacterController controller, StateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();
}