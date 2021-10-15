using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class NinjaStar : AnimatedObject
    {
        protected Vector2 dirMotion;
        protected float rotationSpeed;
        protected int removePoint;

        public NinjaStar(Texture2D starSprite, Vector2 dirMotion, int removePoint, float rotSpeed, Rectangle dest)
            : base(starSprite, 256, 256, dest)
        {
            this.dirMotion = dirMotion;
            this.removePoint = removePoint;
            this.rotationSpeed = rotSpeed;
            setDefaultOrigin();
            rotation = 0;
        }

        public override void update(GameTime gameTime)
        {
            //base.update(gameTime);
            moveBy(dirMotion.X * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, 
                dirMotion.Y * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            rotation += rotationSpeed;
            rotation %= (float)(2 * Math.PI);
            setRotation(rotation);
        }

        public Vector2 getMoveVector()
        {
            return dirMotion;
        }

        public Vector2 getNormalMoveVector()
        {
            int negMultiplyX = (dirMotion.X < 0) ? -1 : 1;
            int negMultiplyY = (dirMotion.Y < 0) ? -1 : 1;
            int x = (int)dirMotion.X;
            int y = (int)dirMotion.Y;
            int resultX = (x == 0) ? 0 : x/x;
            int resultY = (y == 0) ? 0 : y/y;
            return new Vector2(resultX * negMultiplyX,
                                resultY * negMultiplyY);
        }

        public bool isExpired()
        {
            if (dirMotion.X > 0)
                return dest.X > removePoint;
            else if(dirMotion.X < 0)
                return dest.Right < removePoint;
            else if (dirMotion.Y > 0)
                return dest.Y > removePoint;
            else 
                return dest.Bottom < removePoint;
        }

        public bool testCollision(GameObject obj)
        {
            return obj.getRect().Intersects(getRect());
        }
    }
}
