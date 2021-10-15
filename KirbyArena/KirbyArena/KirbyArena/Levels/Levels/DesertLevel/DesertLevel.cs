using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace KirbyArena
{
    public class DesertLevel: Level
    {
        public DesertLevel(Rectangle displayRect, GameOptions options, KirbyGame appRef)
            : base(displayRect, options, appRef)
        {
            this.levelState = LevelState.Loaded;
            levelBackground = new DesertForeground(loadTexture("WndContent\\LevelBackgrounds\\desertbg"), displayRect, this);

            int[,] mapDef = new int[10,10] {
                {6, 6, 6, 6, 6, 6, 6, 6, 7, 7},
                {6, 8, 6, 7, 6, 6, 7, 6, 8, 7},
                {6, 6, 7, 8, 6, 6, 8, 7, 6, 6},
                {6, 7, 8, 6, 6, 6, 6, 8, 7, 6},
                {6, 6, 6, 6, 7, 7, 6, 6, 6, 6},
                {6, 6, 6, 6, 7, 7, 6, 6, 6, 6},
                {6, 7, 8, 6, 6, 6, 6, 8, 7, 6},
                {6, 6, 7, 8, 6, 6, 8, 7, 6, 6},
                {7, 8, 6, 7, 6, 6, 7, 6, 8, 6},
                {7, 7, 6, 6, 6, 6, 6, 6, 6, 6}
            };

            generateMap(mapDef);
        }
    }
}
