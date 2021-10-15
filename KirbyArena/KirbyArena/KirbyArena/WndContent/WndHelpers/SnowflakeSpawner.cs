using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class SnowflakeSpawner : WndComponent
    {
        private List<Snowflake> snowflakes;
        private List<Snowflake> removeList;
        private float spawnTimer;
        private Random gen;
        private Texture2D snowflakeSprite;

        public SnowflakeSpawner(Texture2D snowflakeSprite, Rectangle displayRect)
            : base(displayRect)
        {
            this.snowflakeSprite = snowflakeSprite;
            this.gen = new Random();
            this.snowflakes = new List<Snowflake>();
            this.removeList = new List<Snowflake>();
            this.spawnTimer = 0;
        }

        public override void update(GameTime gameTime)
        {
            if (spawnTimer <= 0)
            {
                int x = gen.Next(0, this.dest.Right - 1);
                int width = gen.Next(10, 30);
                int height = (int)(gen.NextDouble() * 0.6 - 1) * width + width;
                Snowflake snowflake = new Snowflake(snowflakeSprite, new Rectangle(x, this.dest.Top-height, width, height));
                snowflakes.Add(snowflake);
                spawnTimer = gen.Next(150, 250);
            }
            else
            {
                spawnTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }

            foreach(Snowflake snowflake in snowflakes)
            {
                snowflake.update(gameTime);

                if (snowflake.isExpired(this.dest.Bottom))
                {
                    removeList.Add(snowflake);
                }
            }

            foreach (Snowflake snowflake in removeList)
            {
                snowflakes.Remove(snowflake);
            }
            removeList.Clear();
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            foreach (Snowflake snowflake in snowflakes)
            {
                snowflake.draw(spriteBatch);
            }
        }
    }
}
