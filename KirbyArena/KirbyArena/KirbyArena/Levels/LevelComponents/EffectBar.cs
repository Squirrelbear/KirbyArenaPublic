using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class EffectBar : Panel
    {
        protected Level levelRef;
        protected Player player;
        protected SpriteFont font;

        public EffectBar(Rectangle dest, Player player, Level levelRef)
            : base(dest)
        {
            this.levelRef = levelRef;
            font = levelRef.loadFont("largeFont");
            this.player = player;
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            LayoutManger lm = new LayoutManger(dest, 1, 5);

            if (player.getCurItem() != null)
            {
                spriteBatch.Draw(player.getCurItem().getTexture(), lm.getRect(0, 0), Color.White);
            }

            int cellX = 2;
            foreach(StatusEffect effect in player.getAllStatusEffects())
            {
                int time = effect.getTimeInSeconds();
                string text = "";
                if (effect.getEffectType() == StatusEffect.EffectType.Shield)
                    text += effect.getShieldCount() * 0.5;
                else
                    text += time;
                Vector2 stringDims = font.MeasureString(text);
                Point centre = lm.getRect(0, cellX).Center;
                int posX = centre.X - (int)stringDims.X / 2;
                int posY = centre.Y - (int)stringDims.Y / 2;
                Vector2 textPos = new Vector2(posX, posY);
                Color c = Color.Black;
                if(time < 5)
                    c = Color.Red;

                switch (effect.getEffectType())
                {
                    case StatusEffect.EffectType.Link:
                        spriteBatch.Draw(levelRef.getCollectibleSprites()[(int)Collectible.CollectibleType.LinkGear], lm.getRect(0, cellX), Color.White);
                        spriteBatch.DrawString(font, text, new Vector2(textPos.X - 2, textPos.Y + 2), Color.White); 
                        spriteBatch.DrawString(font, text, textPos, c); 
                        break;
                    case StatusEffect.EffectType.Shield:
                        spriteBatch.Draw(levelRef.getCollectibleSprites()[(int)Collectible.CollectibleType.Shield], lm.getRect(0, cellX), Color.White);
                        spriteBatch.DrawString(font, text, new Vector2(textPos.X - 2, textPos.Y + 2), Color.White);
                        spriteBatch.DrawString(font, text, textPos, c); 
                        break;
                    case StatusEffect.EffectType.Speed:
                        spriteBatch.Draw(levelRef.getCollectibleSprites()[(int)Collectible.CollectibleType.Speed], lm.getRect(0, cellX), Color.White);
                        spriteBatch.DrawString(font, text, new Vector2(textPos.X - 2, textPos.Y + 2), Color.White);
                        spriteBatch.DrawString(font, text, textPos, c); 
                        break;
                }

                cellX++;
            }
        }
    }
}
