using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class HealthBar : WndComponent
    {
        public const double MAXHEARTS = 10;

        protected Texture2D heart, halfheart;
        protected double heartCount;
        protected Label textLabel;
        protected bool trackMouse;
        protected bool ingameMode;
        protected Rectangle destText;

        public HealthBar(Rectangle dest, Texture2D heart, Texture2D halfheart, SpriteFont font)
            : base(dest)
        {
            this.heart = heart;
            this.halfheart = halfheart;
            heartCount = MAXHEARTS;
            destText = dest;
            textLabel = new Label(dest, "Unlimited Lives", font);
            textLabel.centreInRect(destText);
            trackMouse = false;
            ingameMode = false;
        }

        public HealthBar(Rectangle dest, Rectangle destText, Texture2D heart, Texture2D halfheart, SpriteFont font)
            : this(dest, heart, halfheart, font)
        {
            this.destText = destText;
            textLabel.centreInRect(destText);
            ingameMode = true;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public void setTrackMouse(bool trackMouse)
        {
            this.trackMouse = trackMouse;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            if (heartCount <= 0)
            {
                textLabel.draw(spriteBatch);
            }
            else if (!ingameMode)
            {
                int fullCount = (int)heartCount;
                Rectangle targetDest = new Rectangle(dest.X, dest.Y, dest.Height, dest.Height);
                for (int i = 0; i < fullCount; i++)
                {
                    spriteBatch.Draw(heart, targetDest, Color.White);
                    targetDest.X += dest.Height;
                }

                if (heartCount - fullCount > 0)
                {
                    spriteBatch.Draw(halfheart, targetDest, Color.White);
                }
            }
            else
            {
                int fullCount = (int)heartCount;
                Rectangle targetDest = new Rectangle(dest.X, dest.Bottom - dest.Width, dest.Width, dest.Width);
                for (int i = 0; i < fullCount; i++)
                {
                    spriteBatch.Draw(heart, targetDest, Color.White);
                    targetDest.Y -= dest.Width;
                }

                if (heartCount - fullCount > 0)
                {
                    spriteBatch.Draw(halfheart, targetDest, Color.White);
                }
            }
        }

        public void setHearts(double value)
        {
            if (heartCount == -0.5 || heartCount == 0)
                return;
            this.heartCount = value;
            if (heartCount == 0)
            {
                textLabel.setText("No Lives Left");
                textLabel.centreInRect(destText);
            }
        }

        public double getHearts()
        {
            return heartCount;
        }

        public override void mouseMoved(Point oldP, Point newP)
        {
            if (!trackMouse) return;

            base.mouseMoved(oldP, newP);

            Rectangle trackRect = new Rectangle(dest.X, dest.Y, dest.Width + dest.Height / 2, dest.Height);

            if (trackRect.Contains(newP) && trackRect.Contains(oldP))
            {
                double value = (newP.X - dest.X) * 1.0 / dest.Height;
                value = Math.Ceiling(value * 2) / 2;
                if (value == MAXHEARTS + 0.5)
                    value = -0.5;
                heartCount = value;
            }
        }

        public override void next()
        {
            if (heartCount == MAXHEARTS)
                heartCount = -0.5;
            else if (heartCount == -0.5)
                heartCount = 0.5;
            else
                heartCount += 0.5;
        }

        public override void previous()
        {
            if (heartCount == -0.5)
                heartCount = MAXHEARTS;
            else if (heartCount == 0.5)
                heartCount = -0.5;
            else
                heartCount -= 0.5;
        }
    }
}
