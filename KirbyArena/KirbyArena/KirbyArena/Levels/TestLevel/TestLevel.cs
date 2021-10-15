using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class TestLevel : Level
    {
        public TestLevel(Rectangle displayRect, GameOptions options, KirbyGame appRef)
            : base(displayRect, options, appRef)
        {
            this.levelState = LevelState.Loaded;
            levelBackground = new LevelBackground(loadTexture("WndContent\\LevelBackgrounds\\levelsnowbg"), displayRect);

            int[,] mapDef = new int[10,10] {
                {2, 0, 0, 0, 2, 2, 0, 0, 0, 2},
                {0, 1, 1, 2, 1, 1, 2, 1, 1, 0},
                {0, 1, 2, 0, 0, 0, 0, 2, 1, 0},
                {0, 2, 0, 0, 0, 0, 0, 0, 2, 0},
                {2, 1, 0, 1, 2, 2, 1, 0, 1, 2},
                {2, 1, 0, 1, 2, 2, 1, 0, 1, 2},
                {0, 2, 0, 0, 0, 0, 0, 0, 2, 0},
                {0, 1, 2, 0, 0, 0, 0, 2, 1, 0},
                {0, 1, 1, 2, 1, 1, 2, 1, 1, 0},
                {2, 0, 0, 0, 2, 2, 0, 0, 0, 2}
            };

            generateMap(mapDef);
        }
    }
}
