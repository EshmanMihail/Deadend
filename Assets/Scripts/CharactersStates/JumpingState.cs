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
    }
}
