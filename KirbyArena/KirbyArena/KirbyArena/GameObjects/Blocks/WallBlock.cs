using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class WallBlock : Block
    {
        public WallBlock(Texture2D spriteSheet, Texture2D childSprite, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(spriteSheet, position, dimensions, dest, level)
        {
            blockType = BlockType.Wall;

            if (childSprite != null)
            {
                Block child = new Block(childSprite, position, dimensions, dest, level);
                setChild(child);
            }

            setCanEnterPlayerAll(false);
            setCanEnterMonsterAll(false);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            base.draw(spriteBatch, pos);
        }

    }
}
