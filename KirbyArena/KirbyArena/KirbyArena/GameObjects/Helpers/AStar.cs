using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Diagnostics;

namespace KirbyArena
{
    public class AStar
    {
        private GameObject obj;
        private Level lvl;
        private Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
        private Tile source, destination;
        private Tile current, neighbor;
        private double timeElapsed;

        public double GetEstimate(Vector2 source, Vector2 destination)
        {
            double scale = 1.0 / 1000;
            return ((Math.Abs(destination.X - source.X) + Math.Abs(destination.Y - source.Y)) * (1 + scale));
        }

        private class Tile
        {
            public Vector2 position = Vector2.Zero;
            public GameObject.Direction facing = GameObject.Direction.Neutral;
            public bool walkable;
            public double G = 0;
            public double F = 0;
            public Tile parent;
        }

        private Tile setTile(GameObject obj)
        {
            Tile tile = new Tile();

            tile.position = obj.getPosition();
            tile.walkable = true;

            return tile;
        }

        private Tile[] getNeighbors(Tile tile)
        {
            Tile[] neighbors = new Tile[4];

            for (int i = 0; i < 4; i++)
            {
                Tile newTile = new Tile();
                Tile tempTile = new Tile();
                newTile.position = tile.position - sides[i];

                if (newTile.position.X >= lvl.getMap().GetLength(0) || newTile.position.X < 0
                    || newTile.position.Y >= lvl.getMap().GetLength(1) || newTile.position.Y < 0)
                {
                    neighbors[i] = null;
                    continue;
                }

                Block block = lvl.getBlockAt(newTile.position);
                newTile.walkable = lvl.canEnter(obj, newTile.position, (GameObject.Direction)i);

                if (newTile.walkable)
                    tempTile = new Tile();

                if (!Vector2.Equals(tempTile.position, Vector2.Zero))
                    newTile = tempTile;

                newTile.facing = (GameObject.Direction)i;
                neighbors[i] = newTile;
            }

            return neighbors;
        }

        private List<GameObject.Direction> doAStar(GameObject obj, GameObject tar, Level lvl)
        {
            double[,] costMap = lvl.getCostMap();

            this.obj = obj;
            this.lvl = lvl;

            source = setTile(obj);
            destination = setTile(tar);
            //neighbor = new Tile();

            //source.F = GetEstimate(neighbor.position, destination.position);

            ArrayList open = new ArrayList();
            open.Add(source);

            ArrayList closed = new ArrayList();

            IEnumerator openEnum = open.GetEnumerator();
            openEnum.MoveNext();
            current = (Tile)openEnum.Current;

            timeElapsed = 0;

            while (!Vector2.Equals(current.position, destination.position) && timeElapsed < 100)
            {
                timeElapsed += 1;
                closed.Add(current);
                open.Remove(current);
                Tile[] neighbors = getNeighbors(current);

                for (int i = 0; i < 4; i++)
                {
                    neighbor = neighbors[i];
                    if (neighbor == null)
                        continue;

                    if (neighbor.walkable == false)
                    {
                        if (Vector2.Equals(neighbor.position, destination.position))
                            return makePath();
                        else
                        {
                            closed.Add(neighbor);
                            continue;
                        }
                    }

                    if (findTile(closed, neighbor)) continue;

                    double cost = current.G + 1;

                    //Double cost to deter monster from bomb
                    //if (lvl.getBlockAt(neighbor.position).getBlockType() == Block.BlockType.Bomb)
                    //    cost += cost;

                    // apply additional costs (eg, for special level components that are not in blocks)
                    cost += cost * costMap[(int)neighbor.position.X, (int)neighbor.position.Y];

                    neighbor.G += cost;
                    neighbor.F = neighbor.G + GetEstimate(neighbor.position, destination.position);
                    neighbor.parent = current;

                    if (findTile(open, neighbor))
                    {
                        Tile tile = getTile(open, neighbor);
                        if (neighbor.G < tile.G)
                        {
                            open.Remove(tile);
                            tile.G += cost;
                            tile.F = neighbor.G + GetEstimate(tile.position, destination.position);
                            tile.parent = current;

                            sortOpen(ref open);
                        }
                    }

                    if (!findTile(open, neighbor))
                        sortOpen(ref open);
                }

                if (open.Count == 0)
                    return makePath();

                openEnum = open.GetEnumerator();
                openEnum.MoveNext();
                current = (Tile)openEnum.Current;
            }

            return makePath();
        }

        private Tile getTile(ArrayList list, Tile tile)
        {
            foreach (Tile listTile in list)
            {
                if (Vector2.Equals(tile.position, listTile.position))
                    return listTile;
            }

            return new Tile();
        }

        private List<GameObject.Direction> makePath()
        {
            List<GameObject.Direction> path = new List<GameObject.Direction>();

            while (!Vector2.Equals(current.position, source.position))
            {
                if (Vector2.Equals(current.position, destination.position))
                    path.Add(GameObject.Direction.Neutral);

                path.Add(current.facing);
                current = current.parent;
            }

            path.Reverse();

            return path;
        }

        private bool findTile(ArrayList list, Tile tile)
        {
            foreach (Tile listTile in list)
            {
                if (Vector2.Equals(tile.position, listTile.position))
                    return true;
            }

            return false;
        }

        private void sortOpen(ref ArrayList list)
        {
            int index = 0;

            foreach (Tile tile in list)
            {
                if (neighbor.F > tile.F)
                    index++;
                else
                    break;
            }

            list.Insert(index, neighbor);
        }

        /// <summary>
        /// Implements A* algorithm to create path from obj to player.
        /// </summary>
        /// <param name="obj">GameObject needing the pathing</param>
        /// <param name="lvl">Instance of level</param>
        /// <returns></returns>
        public List<GameObject.Direction> getPath(GameObject obj, int playerid, Level lvl)
        {
            return doAStar(obj, lvl.getPlayer(playerid), lvl);
        }

        /// <summary>
        /// Implements A* algorithm to create path from one object to another.
        /// </summary>
        /// <param name="obj">GameObject needing the pathing</param>
        /// <param name="tar">Target GameObject for algorithm to search for</param>
        /// <param name="lvl">Instance of level</param>
        /// <returns></returns>
        public List<GameObject.Direction> getPath(GameObject obj, GameObject tar, Level lvl)
        {
            return doAStar(obj, tar, lvl);
        }
    }
}