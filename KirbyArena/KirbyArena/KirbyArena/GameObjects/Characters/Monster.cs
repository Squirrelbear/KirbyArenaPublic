using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class Monster : GameObject
    {
        public Monster(Texture2D spriteSheet, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(spriteSheet, position, dimensions, dest, level)
        {
            type = Type.Monster;
        }
    }
}
