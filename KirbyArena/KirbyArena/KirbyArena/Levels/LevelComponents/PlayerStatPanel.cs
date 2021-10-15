using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class PlayerStatPanel : Panel
    {
        protected Label playerLabel;
        protected SpriteFont font;
        protected Player player;
        protected HealthBar healthBar;
        protected bool alignLeft;
        protected string baseText;

        public PlayerStatPanel(Rectangle dest, Player player, string labelText, bool alignLeft, 
                                Texture2D background, Texture2D heart, Texture2D halfHeart, SpriteFont font,
                                Rectangle displayRect, SpriteFont healthFont, Level levelRef)
            : base(dest, background)
        {
            this.player = player;
            int hpHeight = (int)(dest.Width / HealthBar.MAXHEARTS);

            baseText = labelText;
            Vector2 textDims = font.MeasureString(baseText.Replace("%s", ""+player.getScore()));
            Vector2 textSmallDims = healthFont.MeasureString("Sample Text");
            Rectangle labelRect;
            this.alignLeft = alignLeft;
            this.font = font;
            Rectangle textRect;
            if(alignLeft)
            {
                textRect = new Rectangle(dest.X + 20, dest.Bottom - (int)(textSmallDims.Y + 5), dest.Width - 40, (int)(textSmallDims.Y + 10));
                int cellHeight = (int)((displayRect.Height - dest.Height - 10) / HealthBar.MAXHEARTS);
                Rectangle healthRect = new Rectangle(dest.Left + 5, displayRect.Top + 5, cellHeight,
                                                    (int)(cellHeight * HealthBar.MAXHEARTS));
                healthBar = new HealthBar(healthRect, textRect, heart, halfHeart, healthFont);
                labelRect = new Rectangle(dest.X + 20, dest.Y + 5, (int)textDims.X, (int)textDims.Y);
            }
            else
            {
                textRect = new Rectangle(dest.X + 20, dest.Bottom - (int)(textSmallDims.Y + 5), dest.Width - 40, (int)(textSmallDims.Y + 10));
                int cellHeight = (int)((displayRect.Height - dest.Height - 10) / HealthBar.MAXHEARTS);
                Rectangle healthRect = new Rectangle(dest.Right - cellHeight - 5, displayRect.Top + 5, cellHeight,
                                                    (int)(cellHeight * HealthBar.MAXHEARTS));
                healthBar = new HealthBar(healthRect, textRect, heart, halfHeart, healthFont);
                labelRect = new Rectangle(dest.Right - (int)textDims.X - 20, dest.Y + 5, (int)textDims.X, (int)textDims.Y);
            }

            Rectangle effectBarRect = new Rectangle(dest.X + 10, labelRect.Bottom - 8, dest.Width - 20, textRect.Top - labelRect.Bottom + 15);
            EffectBar effectBar = new EffectBar(effectBarRect, player, levelRef);

            playerLabel = new Label(labelRect, labelText, font);
            playerLabel.setColor(Color.Maroon);

            addComponent(playerLabel);
            addComponent(effectBar);
            addComponent(healthBar);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            healthBar.setHearts(player.getLives());

            string nextText = baseText.Replace("%s", ""+player.getScore());
            playerLabel.setText(nextText);

            if (!alignLeft)
            {
                Vector2 textDims = font.MeasureString(nextText);
                Rectangle newRect = playerLabel.getRect();
                newRect.X = dest.Right - (int)textDims.X - 20;
                newRect.Width = (int)textDims.X;
                playerLabel.setRect(newRect);
            }
        }
    }
}
