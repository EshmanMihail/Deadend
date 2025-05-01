
using UnityEngine;

namespace Assets.Scripts.CharactersStates
{
    public class StandingState : GroundedState
    {
        private bool jump;
        private int speedyParam = Animator.StringToHash("SpeedY");

        public StandingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            jump = false;
        }

        public override void HandleInput()
        {
            base.HandleInput();            
            character.Jump(speedyParam);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
