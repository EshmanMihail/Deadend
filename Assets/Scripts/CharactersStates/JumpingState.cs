using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.CharactersStates
{
    public class JumpingState : State
    {
        private bool grounded;
        private int speedyParam = Animator.StringToHash("SpeedY");

        public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            character.Jump(speedyParam);
            grounded = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (grounded)
            {
                //SoundManager.Instance.PlaySound(SoundManager.Instance.jumpHit);
                stateMachine.ChangeState(character.standing);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            Vector2 v2GroundedBoxCheckPosition = (Vector2)character.transform.position + new Vector2(0, -0.01f);
            Vector2 v2GroundedBoxCheckScale = (Vector2)character.transform.localScale + new Vector2(-0.04f, 0);
            bool wallGround = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, character.lmWalls);
            bool platformGround = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, character.lmPlatform);

            if (wallGround || platformGround) grounded = true;
        }
    }
}
