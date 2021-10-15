using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class PacmanWall : WallBlock
    {
        public PacmanWall(Texture2D spriteSheet, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(spriteSheet, null, position, dimensions, dest, level) 
        {
        }

        public override void onLevelLoaded()
        {
            base.onLevelLoaded();

            Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
            string textureName = "Sprites/Blocks/pacman";
            for(int i = 0; i < 4; i++) 
            {
                Vector2 pos = getPosition() - sides[i];

                if (pos.X >= levelRef.getMap().GetLength(0) || pos.X < 0
                    || pos.Y >= levelRef.getMap().GetLength(1) || pos.Y < 0)
                {
                    textureName += 0;
                    continue;
                }

                Block block = levelRef.getBlockAt(pos);
                textureName += (block.getBlockType() == BlockType.Wall) ? 1 : 0;
            }

            setSpriteSheet(levelRef.loadTexture(textureName));
        }
    }
}
