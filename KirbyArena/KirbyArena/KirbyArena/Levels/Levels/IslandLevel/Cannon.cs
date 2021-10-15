using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class Cannon : AnimatedObject
    {
        protected bool fireLeft;
        protected int removePoint;
        protected List<CannonBall> cannonBalls;
        protected Texture2D cannonBallSprite;
        protected Timer fireTimer;
        protected int dimension;
        protected Level levelRef;
        protected int ballSpeed;

        public Cannon(Texture2D cannonSprite, Rectangle dest, Texture2D cannonBallSprite, int delayTimer, int nextFireTimer, bool fireLeft, int removePoint, Level level)
            : base(cannonSprite, 256, 128, dest)
        {
            this.levelRef = level;
            cannonBalls = new List<CannonBall>();
            this.cannonBallSprite = cannonBallSprite;
            fireTimer = new Timer(delayTimer, nextFireTimer);
            this.removePoint = removePoint;
            this.fireLeft = fireLeft;
            dimension = dest.Height - 8;
            ballSpeed = dest.Height * 3;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            Player p1 = levelRef.getPlayer(1);
            Player p2 = levelRef.getPlayer(2);

            List<CannonBall> removeList = new List<CannonBall>();
            foreach (CannonBall cannonBall in cannonBalls)
            {
                cannonBall.update(gameTime);

                if (p1 != null && cannonBall.testCollision(p1))
                {
                    p1.damage(1);
                    p1.stun();
                    removeList.Add(cannonBall);
                }
                else if (p2 != null && cannonBall.testCollision(p2))
                {
                    p2.damage(1);
                    p2.stun();
                    removeList.Add(cannonBall);
                }
                else if (cannonBall.isExpired())
                {
                    removeList.Add(cannonBall);
                }
            }

            foreach (CannonBall cannonBall in removeList)
            {
                cannonBalls.Remove(cannonBall);
            }

            fireTimer.update(gameTime);
            if (fireTimer.wasTriggered())
            {
                fire();
            }
        }

        public void fire()
        {
            Rectangle spawnRect =  new Rectangle(dest.Center.X, dest.Y + 4, dimension, dimension);
            CannonBall ball = new CannonBall(cannonBallSprite, fireLeft, removePoint, ballSpeed, spawnRect);
            cannonBalls.Add(ball);
        }

        public void drawCannonBalls(SpriteBatch spriteBatch)
        {
            foreach (CannonBall cannonBall in cannonBalls)
            {
                cannonBall.draw(spriteBatch);
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
        }
    }
}
