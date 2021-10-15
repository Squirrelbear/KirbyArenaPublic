using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class LevelBackground
    {
        protected Texture2D background;
        protected Rectangle backgroundRect;
        protected Texture2D foreground;
        protected Rectangle foregroundRect;

        public LevelBackground(Texture2D background, Rectangle backgroundRect)
            : this(background, backgroundRect, null, new Rectangle(-1,-1,0,0))
        {
        }

        public LevelBackground(Texture2D background, Rectangle backgroundRect, Texture2D foreground, Rectangle foregroundRect)
        {
            this.background = background;
            this.backgroundRect = backgroundRect;
            this.foreground = foreground;
            this.foregroundRect = foregroundRect;
        }

        public virtual void update(GameTime gameTime)
        {

        }
        
        public virtual void drawBackground(SpriteBatch spriteBatch)
        {
            if (background != null)
                spriteBatch.Draw(background, backgroundRect, Color.White);
        }

        public virtual void drawForeground(SpriteBatch spriteBatch)
        {
            if (foreground != null)
                spriteBatch.Draw(foreground, foregroundRect, Color.White);
        }
    }
}
