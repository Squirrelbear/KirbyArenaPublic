using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class Block : GameObject
    {
        public enum BlockType { Basic = 0, Wall = 1, CollectibleSpawner = 2, // Snow
                                BasicSand = 3, WallPalm = 4, SpawnerSand = 5, // Island
                                BasicDesert = 6, WallCactus = 7, SpawnerDesert = 8, // Mexico
                                BasicNinja = 9, WallNinja = 10, SpawnerNinja = 11, RandomNinja = 12, // Ninja
                                BasicPacMan=13, WallPacMan=14, SpawnerPacMan= 15 // PacMan
                              };
        protected BlockType blockType;

        public Block(Texture2D spriteSheet, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(spriteSheet, position, new Vector2(spriteSheet.Width, spriteSheet.Height), dest, level)
        {
            type = Type.Block;
            blockType = BlockType.Basic;
        }

        public BlockType getBlockType()
        {
            return blockType;
        }
    }
}
