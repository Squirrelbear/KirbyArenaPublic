using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class ShipEnemy
    {
        protected AnimatedObject shipBottom, shipTop;
        protected List<Cannon> cannons;

        public ShipEnemy(Rectangle displayRect, bool alignLeft, 
                        Texture2D shipBottomSprite, Texture2D shipTopSprite, 
                        Texture2D cannonSprite, Texture2D cannonBallSprite, Level levelRef)
        {
            cannons = new List<Cannon>();

            //int shipHeight = (int)(displayRect.Height * 0.8f);
            int shipWidth = levelRef.getRect(0,0).Width * 7;//(int)(shipBottomSprite.Width * 1.0 * shipHeight / shipBottomSprite.Height);
            int shipHeight = (int)(shipBottomSprite.Height * 1.0 * shipWidth / shipBottomSprite.Width);
            int xBase = (alignLeft) ? displayRect.Left : displayRect.Right;
            Rectangle shipRect = new Rectangle(xBase - shipWidth / 2,
                                                (int)(displayRect.Center.Y - shipHeight / 2),
                                                shipWidth, shipHeight);
            if (alignLeft)
            {
                shipRect.Y += levelRef.getRect(0, 0).Width / 2;
            }
            else
            {
                shipRect.Y -= levelRef.getRect(0, 0).Width / 2;
            }

            shipBottom = new AnimatedObject(shipBottomSprite, shipBottomSprite.Width, shipBottomSprite.Height, shipRect);
            shipTop = new AnimatedObject(shipTopSprite, shipTopSprite.Width, shipTopSprite.Height, shipRect);

            if (!alignLeft)
            {
                shipBottom.setSpriteEffect(SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
                shipTop.setSpriteEffect(SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
            }

            int startCell = (alignLeft) ? 2 : 3;
            Rectangle lineUpCellRect = levelRef.getRect(0, startCell);
            int shiftAlign = (alignLeft) ? -lineUpCellRect.Width : lineUpCellRect.Width;
            int multiAdd = (alignLeft) ? 0 : -2;
            int shipSide = (alignLeft) ? shipRect.Right : shipRect.Left;
            Rectangle cannonOneRect = new Rectangle(shipSide + (int)(shiftAlign * (3+multiAdd)),
                                                    lineUpCellRect.Y,
                                                    lineUpCellRect.Width*2,
                                                    lineUpCellRect.Height);
            int timeDelay = 15000;
            int fireNextTimer = 0;
            switch (levelRef.getAppRef().getGameOptions().difficulty)
            {
                case GameOptions.Difficulty.Easy:
                    fireNextTimer = 40000;
                    break;
                case GameOptions.Difficulty.Medium:
                    fireNextTimer = 30000;
                    break;
                case GameOptions.Difficulty.Hard:
                    fireNextTimer = 15000;
                    break;
            }
            int removePoint = (alignLeft) ? displayRect.Right : displayRect.Left;
            bool fireDirection = !alignLeft;
            cannons.Add(new Cannon(cannonSprite, cannonOneRect, cannonBallSprite, timeDelay, fireNextTimer, fireDirection, removePoint, levelRef));

            startCell += 2;
            lineUpCellRect = levelRef.getRect(0, startCell);
            Rectangle cannonTwoRect = new Rectangle(shipSide + (int)(shiftAlign * (2.5 + multiAdd)),
                                                    lineUpCellRect.Y,
                                                    lineUpCellRect.Width * 2,
                                                    lineUpCellRect.Height);
            timeDelay += 3000;
            cannons.Add(new Cannon(cannonSprite, cannonTwoRect, cannonBallSprite, timeDelay, fireNextTimer, fireDirection, removePoint, levelRef));

            startCell += 2;
            lineUpCellRect = levelRef.getRect(0, startCell);
            Rectangle cannonThreeRect = new Rectangle(shipSide + (int)(shiftAlign * (3 + multiAdd)),
                                                    lineUpCellRect.Y,
                                                    lineUpCellRect.Width * 2,
                                                    lineUpCellRect.Height);
            timeDelay += 3000;
            cannons.Add(new Cannon(cannonSprite, cannonThreeRect, cannonBallSprite, timeDelay, fireNextTimer, fireDirection, removePoint, levelRef));

            if (!alignLeft)
            {
                foreach (Cannon cannon in cannons)
                {
                    cannon.setSpriteEffect(SpriteEffects.FlipHorizontally);
                }
            }
        }

        public void update(GameTime gameTime)
        {
            shipBottom.update(gameTime);
            foreach (Cannon cannon in cannons)
            {
                cannon.update(gameTime);
            }
            shipTop.update(gameTime);
        }

        public void drawCannonBalls(SpriteBatch spriteBatch)
        {
            foreach (Cannon cannon in cannons)
            {
                cannon.drawCannonBalls(spriteBatch);
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            shipBottom.draw(spriteBatch);
            foreach (Cannon cannon in cannons)
            {
                cannon.draw(spriteBatch);
            }
            shipTop.draw(spriteBatch);
        }
    }
}
