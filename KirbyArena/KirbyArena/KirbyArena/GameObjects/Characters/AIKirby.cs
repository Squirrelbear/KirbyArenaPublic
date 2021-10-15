using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class AIKirby : Player
    {
        protected AIState aiState;

        public AIKirby(PlayerIndex playerIndex, List<Texture2D> spriteSet, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(playerIndex, null, spriteSet, position, dimensions, dest, level)
        {
            aiState = null;
        }

        public void configureAI(PlayerIndex playerIndex)
        {
            aiState = new AIState(this, levelRef.getPlayer((playerIndex == PlayerIndex.Two) ? 1 : 2), levelRef);
        }

        public override void update(GameTime gameTime)
        {
            // update the current state variables
            // prevent the code from continuing with the "false" return if required.
            if (!updateState(gameTime))
                return;

            if (canMove(gameTime))
            {
                AIState.Action action = aiState.getNextAction(gameTime);

                switch (action.type)
                {
                    case AIState.ActionType.Move:
                        move(action.direction);
                        break;
                    case AIState.ActionType.Attack:
                        beginAttack(action.direction);
                        break;
                    case AIState.ActionType.SuckBlow:
                        beginSuckBlow(action.direction);
                        break;
                    case AIState.ActionType.UseAction:
                        activateShield();
                        break;
                    case AIState.ActionType.Nothing:
                    default:
                        returnToNeutral();
                        break;
                }
            }
        }
    }
}
