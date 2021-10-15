using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class CannonBall : AnimatedObject
    {
        protected float speed;
        protected int removePoint;

        public CannonBall(Texture2D cannonBallSprite, bool fireLeft, int removePoint, int speed, Rectangle dest)
            : base(cannonBallSprite, 256, 256, dest)
        {
            this.speed = (fireLeft) ? -speed : speed;
            this.removePoint = removePoint;
        }

        public override void update(GameTime gameTime)
        {
            //base.update(gameTime);
            moveBy(speed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, 0);
        }

        public bool isExpired()
        {
            if (speed > 0)
                return dest.X > removePoint;
            else
                return dest.Right < removePoint;
        }

        public bool testCollision(GameObject obj)
        {
            return obj.getRect().Intersects(getRect());
        }
    }
}
