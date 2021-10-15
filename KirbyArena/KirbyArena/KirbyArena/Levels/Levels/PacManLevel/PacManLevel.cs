using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    class PacManLevel : Level
    {
        public PacManLevel(Rectangle displayRect, GameOptions options, KirbyGame appRef)
            : base(displayRect, options, appRef)
        {
            this.levelState = LevelState.Loaded;
            hud.setFontTimerColor(Color.White);
            Texture2D blank = new Texture2D(appRef.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.Black });
            levelBackground = new LevelBackground(blank, displayRect);

            int[,] mapDef = new int[10, 10] {
                {15, 13, 13, 13, 15, 15, 13, 13, 13, 15},
                {13, 14, 14, 15, 14, 14, 15, 14, 14, 13},
                {13, 14, 15, 13, 13, 13, 13, 15, 14, 13},
                {13, 15, 13, 13, 13, 13, 13, 13, 15, 13},
                {15, 14, 13, 14, 15, 15, 14, 13, 14, 15},
                {15, 14, 13, 14, 15, 15, 14, 13, 14, 15},
                {13, 15, 13, 13, 13, 13, 13, 13, 15, 13},
                {13, 14, 15, 13, 13, 13, 13, 15, 14, 13},
                {13, 14, 14, 15, 14, 14, 15, 14, 14, 13},
                {15, 13, 13, 13, 15, 15, 13, 13, 13, 15}
            };
            generateMap(mapDef);
        }
    }
}
