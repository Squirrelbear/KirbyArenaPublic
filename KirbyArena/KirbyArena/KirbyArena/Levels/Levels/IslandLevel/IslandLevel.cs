using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class IslandLevel : Level
    {
        public IslandLevel(Rectangle displayRect, GameOptions options, KirbyGame appRef)
            : base(displayRect, options, appRef)
        {
            this.levelState = LevelState.Loaded;
            levelBackground = new IslandBackground(loadTexture("WndContent\\LevelBackgrounds\\levelislandbg"), displayRect, this);

            int[,] mapDef = new int[10,10] {
                {5, 3, 3, 3, 5, 5, 3, 3, 3, 5},
                {3, 3, 5, 4, 3, 3, 4, 5, 3, 3},
                {3, 5, 4, 5, 3, 3, 5, 4, 5, 3},
                {3, 4, 5, 4, 4, 3, 4, 5, 4, 3},
                {5, 3, 3, 3, 5, 5, 4, 3, 3, 5},
                {5, 3, 3, 4, 5, 5, 3, 3, 3, 5},
                {3, 4, 5, 4, 3, 4, 4, 5, 4, 3},
                {3, 5, 4, 5, 3, 3, 5, 4, 5, 3},
                {3, 3, 5, 4, 3, 3, 4, 5, 3, 3},
                {5, 3, 3, 3, 5, 5, 3, 3, 3, 5}
            };

            generateMap(mapDef);
        }
    }
}
