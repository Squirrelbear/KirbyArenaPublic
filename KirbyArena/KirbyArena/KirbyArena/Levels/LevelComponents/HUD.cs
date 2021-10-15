using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class HUD
    {
        protected PlayerStatPanel statPanel1, statPanel2;
        protected Label timerLabel;
        protected Rectangle labelRect;
        protected Timer levelTimer;

        public HUD(Rectangle displayRect, Timer levelTimer, Player playerOne, Player playerTwo, 
                        Texture2D background, Texture2D heart, Texture2D heartHalf, SpriteFont font, SpriteFont smallFont, Level levelRef)
        {
            this.levelTimer = levelTimer;
            if(levelTimer != null)
            {
                Vector2 timerDims = font.MeasureString(levelTimer.getFormatedTime());
                labelRect = new Rectangle(displayRect.X, (int)(displayRect.Bottom - timerDims.Y - 10), 
                                                    displayRect.Width, (int)timerDims.Y);
                timerLabel = new Label(labelRect, levelTimer.getFormatedTime(), font);
                timerLabel.setColor(Color.Maroon);
                timerLabel.centreInRect(labelRect);
            }

            int height = (int)(displayRect.Height * 0.125);
            int width = (int)(displayRect.Width * 0.2);
            Rectangle s1Rect = new Rectangle(displayRect.X, displayRect.Bottom - height, width, height);
            statPanel1 = new PlayerStatPanel(s1Rect, playerOne, "Player 1: %s", true, background, heart, heartHalf, 
                                                font, displayRect, smallFont, levelRef);

            if (playerTwo != null)
            {
                Rectangle s2Rect = new Rectangle(displayRect.Right - width, displayRect.Bottom - height, width, height);
                statPanel2 = new PlayerStatPanel(s2Rect, playerTwo, "%s :Player 2", false, background, heart, heartHalf, 
                                                font, displayRect, smallFont, levelRef);
            }
        }

        public void update(GameTime gameTime)
        {
            statPanel1.update(gameTime);

            if (statPanel2 != null)
            {
                statPanel2.update(gameTime);
            }

            if (timerLabel != null)
            {
                timerLabel.setText(levelTimer.getFormatedTime());
                timerLabel.centreInRect(labelRect);
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            statPanel1.draw(spriteBatch);

            if (statPanel2 != null)
                statPanel2.draw(spriteBatch);

            if (timerLabel != null)
                timerLabel.draw(spriteBatch);
        }

        public void setFontTimerColor(Color fontColor)
        {
            timerLabel.setColor(fontColor);
        }
    }
}
