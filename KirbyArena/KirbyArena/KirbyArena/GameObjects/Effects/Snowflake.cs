using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class Snowflake : AnimatedObject
    {
        float fallRate;

        public Snowflake(Texture2D spriteSheet, Rectangle dest)
            : base(spriteSheet, spriteSheet.Width, spriteSheet.Height, dest)
        {
            Random gen = new Random();
            fallRate = (float)(gen.NextDouble() * 20 + 30);
        }

        public Snowflake(Texture2D spriteSheet, float fallRate, Rectangle dest)
            : base(spriteSheet, spriteSheet.Width, spriteSheet.Height, dest)
        {
            this.fallRate = fallRate;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
            moveBy(0, gameTime.ElapsedGameTime.Milliseconds / 1000.0f * fallRate);
        }

        public bool isExpired(int bottom)
        {
            return dest.Top > bottom;
        }
    }
}
