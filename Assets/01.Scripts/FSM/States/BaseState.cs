
public abstract class BaseState : IState
{
    protected BaseCharacterController controller;

    protected BaseState(BaseCharacterController controller)
    {
        this.controller = controller;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
