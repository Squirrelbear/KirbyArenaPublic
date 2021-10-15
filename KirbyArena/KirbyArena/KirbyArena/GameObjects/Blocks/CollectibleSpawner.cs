using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class CollectibleSpawner : Block
    {
        protected List<Texture2D> collectibleSprites;
        protected Timer spawnTimer;
        protected Random gen;

        public CollectibleSpawner(Texture2D sprite, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(sprite, position, dimensions, dest, level)
        {
            this.blockType = BlockType.CollectibleSpawner;
            this.collectibleSprites = level.getCollectibleSprites();
            gen = level.getSharedRandom();
            resetSpawnTimer();
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if(getChild() == null)
            {
                spawnTimer.update(gameTime);
                if(spawnTimer.wasTriggered())
                {
                    setChild(levelRef.createCollectible(getPosition(), true));
                }
            }
        }

        public override void onChildRemoved()
        {
            base.onChildRemoved();

            resetSpawnTimer();
        }

        public void resetSpawnTimer()
        {
            int nextTime = gen.Next(5000, 30000);
            spawnTimer = new Timer(nextTime);
        }
    }
}
