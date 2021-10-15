using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class Ninja : AnimatedObject
    {
        public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3};

        protected Direction facing;
        protected Point point;
        protected Rectangle screen;
        protected Texture2D ninjaStarSprite;
        protected float rotSpeed;
        protected NinjaStar star;
        protected Level levelRef;
        protected int throwCount;
        protected bool expired;

        public Ninja(Texture2D spriteSheet, Texture2D ninjaStarSprite, Direction facing, Point p, 
                        Rectangle screen,  Rectangle dest, float rotSpeed, Level levelRef)
            : base(spriteSheet, 256, 256, dest)
        {
            this.levelRef = levelRef;
            this.rotSpeed = rotSpeed;
            this.ninjaStarSprite = ninjaStarSprite;
            this.screen = screen;
            this.point = p;
            setFacing(facing);
            star = null;
            throwCount = 0;
            expired = false;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (star != null)
            {
                star.update(gameTime);

                if (levelRef.getPlayer(1) != null && star.testCollision(levelRef.getPlayer(1)))
                {
                    levelRef.getPlayer(1).damage(0.5);
                    star = null;
                }
                else if (levelRef.getPlayer(2) != null && star.testCollision(levelRef.getPlayer(2)))
                {
                    levelRef.getPlayer(2).damage(0.5);
                    star = null;
                }
                if (star != null && star.isExpired())
                    star = null;
            }
            else
            {
                createStar();
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if(facing == Direction.Down || facing == Direction.Right)
                base.draw(spriteBatch);

            if (star != null)
                star.draw(spriteBatch);

            if (facing == Direction.Up || facing == Direction.Left)
                base.draw(spriteBatch);
        }

        public void setFacing(Direction facing)
        {
            this.facing = facing;
            setFrame((int)facing);
        }

        public Direction getFacing()
        {
            return facing;
        }

        public bool checkConflict(Point p)
        {
            return !(p.X != point.X && p.Y != point.Y);
        }

        public Point getPoint()
        {
            return point;
        }

        public bool isExpired()
        {
            return expired;
        }

        public NinjaStar getStar()
        {
            return star;
        }

        private void createStar()
        {
            if (throwCount >= 3)
            {
                expired = true;
                return;
            }

            Point center = getRect().Center;
            Rectangle newDest = new Rectangle(center.X, center.Y, getRect().Width / 2, getRect().Height / 2);
            star = new NinjaStar(ninjaStarSprite, getDirMotion(facing), 
                                            getRemovePoint(), rotSpeed, newDest );
            throwCount++;
        }

        private Vector2 getDirMotion(Direction facing)
        {
            int multiplier = 2;
            if (levelRef.getAppRef().getGameOptions().difficulty == GameOptions.Difficulty.Hard)
                multiplier = 4;
            
            if (facing == Direction.Up)
                return new Vector2(0, -getRect().Width * multiplier);
            else if (facing == Direction.Right)
                return new Vector2(getRect().Width * multiplier, 0);
            else if (facing == Direction.Down)
                return new Vector2(0, getRect().Width * multiplier);
            else
                return new Vector2(-getRect().Width * multiplier, 0);
        }

        private int getRemovePoint()
        {
            if (facing == Direction.Up)
                return screen.Top;
            else if (facing == Direction.Right)
                return screen.Right;
            else if (facing == Direction.Down)
                return screen.Bottom;
            else
                return screen.Left;
        }
    }
}
