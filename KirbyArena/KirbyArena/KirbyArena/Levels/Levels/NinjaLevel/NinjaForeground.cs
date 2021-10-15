using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace KirbyArena
{
    public class NinjaForeground : LevelBackground
    {
        protected Level levelRef;
        protected ArrayList ninjaSpawns;
        protected ArrayList availableSpawns;
        protected ArrayList spawnedNinjas;
        protected int maxNinjas;
        protected Texture2D ninjaSprite;
        protected Texture2D ninjaStarSprite;
        protected double[,] costMap;

        public NinjaForeground(Texture2D backgroundSprite, Rectangle displayRect, ArrayList ninjaSpawns, Level levelRef)
            : base(backgroundSprite, displayRect)
        {
            this.levelRef = levelRef;
            this.ninjaSpawns = ninjaSpawns;

            ninjaSprite = levelRef.loadTexture("Sprites\\ExtraContent\\ninjasheet");
            ninjaStarSprite = levelRef.loadTexture("Sprites\\ExtraContent\\ninjastar");

            availableSpawns = new ArrayList();
            foreach (Point p in ninjaSpawns)
                availableSpawns.Add(new Point(p.X, p.Y));

            spawnedNinjas = new ArrayList();
            maxNinjas = (int)levelRef.getAppRef().getGameOptions().difficulty + 2;

            Block[,] tiles = levelRef.getMap();
            costMap = new double[tiles.GetLength(0), tiles.GetLength(1)];
            
            /*spawnNinja();
            spawnNinja();
            spawnNinja();*/
        }

        public override void  drawForeground(SpriteBatch spriteBatch)
        {
 	        base.drawForeground(spriteBatch);

            foreach(Ninja n in spawnedNinjas)
                n.draw(spriteBatch);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            ArrayList removeList = new ArrayList();
            foreach (Ninja n in spawnedNinjas)
            {
                n.update(gameTime);
                if (n.isExpired())
                    removeList.Add(n);
            }

            foreach (Ninja n in removeList)
            {
                despawnNinja(n);
            }

            if (spawnedNinjas.Count < maxNinjas)
                spawnNinja();
        }

        public void spawnNinja()
        {
            if (spawnedNinjas.Count >= maxNinjas || availableSpawns.Count == 0)
                return;

            int spawn = -1;
            bool valid = false;
            Block b = null;
            Point p;
            while(!valid)
            {
                valid = true;
                spawn = levelRef.getSharedRandom().Next(availableSpawns.Count);
                p = (Point)availableSpawns[spawn];
                b = levelRef.getBlockAt(new Vector2(p.X, p.Y));

                foreach (Ninja n in spawnedNinjas)
                {
                    // if there is a ninja in that row
                    if (n.checkConflict(p))// || !b.canEnter(levelRef.getPlayer(0), GameObject.Direction.Down))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            Ninja.Direction facing = (Ninja.Direction)(levelRef.getSharedRandom().Next(4));

            p = (Point)availableSpawns[spawn];
            Ninja ninja = new Ninja(ninjaSprite, ninjaStarSprite, facing, p, backgroundRect, 
                                    levelRef.getRect(p.X, p.Y), 0.05f, levelRef);
            availableSpawns.RemoveAt(spawn);
            spawnedNinjas.Add(ninja);
            b.setCanEnterPlayerAll(false);
            b.setCanEnterMonsterAll(false);
            updateCostMap();
        }

        public void despawnNinja(int id)
        {
            Ninja n = (Ninja)spawnedNinjas[id];
            Point p = n.getPoint();
            Block b = levelRef.getBlockAt(new Vector2(p.X, p.Y));
            b.setCanEnterPlayerAll(true);
            b.setCanEnterMonsterAll(true);
            availableSpawns.Add(p);
            spawnedNinjas.RemoveAt(id);
        }

        public void despawnNinja(Ninja n)
        {
            for (int i = 0; i < spawnedNinjas.Count; i++)
            {
                if (spawnedNinjas[i] == n)
                {
                    despawnNinja(i);
                    updateCostMap();
                    return;
                }
            }
        }

        public double[,] getCostMap()
        {
            return costMap;
        }

        public void updateCostMap()
        {
            for (int i = 0; i < costMap.GetLength(0); i++)
                for (int j = 0; j < costMap.GetLength(1); j++)
                    costMap[i, j] = 0;

            foreach (Ninja n in spawnedNinjas)
            {
                NinjaStar star = n.getStar();
                if (star == null) continue;
                Vector2 starPos = new Vector2(star.getRect().X, star.getRect().Y);
                Vector2 starNormMove = star.getNormalMoveVector();

                Vector2 gridPos = levelRef.estimateGridPosition(starPos);
                int addCost = 100;
                for (int i = 0; i < 4; i++)
                {
                    if (levelRef.validMapCoord(gridPos))
                        costMap[(int)gridPos.X, (int)gridPos.Y] = addCost;
                    gridPos += starNormMove;
                    addCost -= 30;
                }
                addCost = 2;
                for (int i = 0; i < 3; i++)
                {
                    if (levelRef.validMapCoord(gridPos))
                        costMap[(int)gridPos.X, (int)gridPos.Y] = addCost;
                    gridPos += starNormMove;
                }
            }

        }
    }
}
