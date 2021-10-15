using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace KirbyArena
{
    public class NinjaLevel: Level
    {
        

        public NinjaLevel(Rectangle displayRect, GameOptions options, KirbyGame appRef)
            : base(displayRect, options, appRef)
        {
            this.levelState = LevelState.Loaded;
            hud.setFontTimerColor(Color.White);
            ArrayList ninjaPoints = new ArrayList();

            int[,] mapDef = new int[10,10] {
                {9, 9, 9, 9, 9, 9, 9, 9, 9, 11},
                {9, 10, 11, 10, 9, 9, 10, 11, 10, 9},
                {9, 9, 12, 9, 9, 9, 9, 12, 9, 9},
                {9, 10, 11, 10, 9, 9, 10, 11, 10, 9},
                {9, 9, 9, 9, 12, 12, 9, 9, 9, 9},
                {9, 9, 9, 9, 12, 12, 9, 9, 9, 9},
                {9, 10, 11, 10, 9, 9, 10, 11, 10, 9},
                {9, 9, 12, 9, 9, 9, 9, 12, 9, 9},
                {9, 10, 11, 10, 9, 9, 10, 11, 10, 9},
                {11, 9, 9, 9, 9, 9, 9, 9, 9, 9}
            };
            generateMap(mapDef);

            for (int i = 0; i < mapDef.GetLength(0); i++)
            {
                for (int j = 0; j < mapDef.GetLength(0); j++)
                {
                    if(mapDef[i,j] == 12)
                        ninjaPoints.Add(new Point(i, j));
                }
            }
            levelBackground = new NinjaForeground(loadTexture("WndContent\\LevelBackgrounds\\ninjabg"), displayRect, ninjaPoints, this);
        }

        public override double[,] getCostMap()
        {
            double[,] baseCostMap = base.getCostMap();
            double[,] ninjasCostMap = ((NinjaForeground)levelBackground).getCostMap();

            double[,] resultCostMap = new double[baseCostMap.GetLength(0), baseCostMap.GetLength(1)];

            for (int i = 0; i < resultCostMap.GetLength(0); i++)
            {
                for (int j = 0; j < resultCostMap.GetLength(0); j++)
                {
                    resultCostMap[i, j] = baseCostMap[i, j] + ninjasCostMap[i, j];
                }
            }

            return resultCostMap;
        }
    }
}
