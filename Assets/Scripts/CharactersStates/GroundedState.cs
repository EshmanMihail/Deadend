using UnityEngine;

namespace Assets.Scripts.CharactersStates
{
    public class GroundedState : State
    {
        protected float move;

        private bool doubleTapD;
        private bool doubleTapA;
        private float doubleTapTimer;

        private float speedNow;
        private float stepInterval;

        private int speedParam = Animator.StringToHash("Speed");

        public GroundedState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
            speedNow = character.characterNormaWalkSpeed;
            stepInterval = character.walkInterval;
            doubleTapTimer = 0f;
        }

        public override void Enter()
        {
            base.Enter();
            move = 0.0f;
            doubleTapD = false;
            doubleTapA = false;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            if (!character.isLocalPlayer) return;

            move = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (doubleTapD)
                {
                    speedNow = character.characterRunningSpeed;
                    stepInterval = character.runInterval;
                    doubleTapD = false;
                }
                else
                {
                    doubleTapD = true;
                    doubleTapTimer = character.doubleTapTimeThreshold;
                }
            }
            else if (doubleTapD && doubleTapTimer > 0)
            {
                doubleTapTimer -= Time.deltaTime;
            }
            else
            {
                doubleTapD = false;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (doubleTapA)
                {
                    speedNow = character.characterRunningSpeed;
                    stepInterval = character.runInterval;
                    doubleTapA = false;
                }
                else
                {
                    doubleTapA = true;
                    doubleTapTimer = character.doubleTapTimeThreshold;
                }
            }
            else if (doubleTapA && doubleTapTimer > 0)
            {
                doubleTapTimer -= Time.deltaTime;
            }
            else
            {
                doubleTapA = false;
            }

            // Сброс скорости при отпускании клавиши
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            {
                speedNow = character.characterNormaWalkSpeed;
                stepInterval = character.walkInterval;
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            character.TriggerMoveAnimation(speedParam, Mathf.Abs(move * speedNow));
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (character.isLocalPlayer)
            {
                character.MoveRightAndLeft(move, speedNow, stepInterval);
            }
        }
    }
}
