using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class AIState
    {
        public enum ActionType { Nothing = 0, Move = 1, Attack = 2, SuckBlow = 3, UseAction = 4 };
        public enum ObjectiveType { UseShield = 0, HighScore = 1, Survival = 2, KillPlayerClose = 3, 
                                    GetItemClose = 4, ScoreShort = 5, KillPlayer = 6,
                                    Weapon = 7, ScoreFar = 8, GetItem = 9, SuperLowPriority = 10, IdleTask = 11,
                                    Nothing = 12 };


        public struct Objective
        {
            public ObjectiveType type;
            public Vector2 target;
            public List<GameObject.Direction> path;

            public Objective(ObjectiveType type, Vector2 target, List<GameObject.Direction> path)
            {
                this.type = type;
                this.target = target;
                this.path = path;
            }
        }

        public struct Action
        {
            public ActionType type;
            public GameObject.Direction direction;

            public Action(ActionType type, GameObject.Direction direction)
            {
                this.type = type;
                this.direction = direction;
            }
        }

        protected AIKirby actor;
        protected Player otherPlayer;
        protected Level levelRef;

        protected List<Vector2> collectSpawnPoints;
        protected AStar astar;
        protected Action lastAction;
        //protected Vector2 goalPos;
        protected List<Action> previousActions;
        protected List<int> previousCells;
        protected Objective lastObjective;
        protected int objectiveCount;

        protected Timer pauseTimer;
        protected Objective infiniteLoopObjective;

        protected Vector2 longTermGoal;

        private int MAX_ACTION_HISTORY = 50;
        private int HISTORY_CLEAR_INTERVAL = 20;

        public AIState(AIKirby actor, Player otherPlayer, Level levelRef)
        {
            this.actor = actor;
            this.otherPlayer = otherPlayer;
            this.levelRef = levelRef;

            collectSpawnPoints = null;
            astar = new AStar();
            lastAction = new Action(ActionType.Nothing, GameObject.Direction.Neutral);
            //goalPos = actor.getPosition();

            previousActions = new List<Action>();
            previousCells = new List<int>();
            lastObjective = new Objective(ObjectiveType.Nothing, new Vector2(0,0), new List<GameObject.Direction>());
            objectiveCount = 0;
        }

        private Action setLastAction(Action newAction, Objective newObjective)
        {
            this.lastAction = newAction;
            previousActions.Add(newAction);
            if(pauseTimer == null)
                lastObjective = newObjective;
            Vector2 targetCell = levelRef.getMoveFromDirection(actor, newAction.direction);
            int cellValue = (int)(levelRef.getMap().GetLength(0) * targetCell.Y + targetCell.X);
            previousCells.Add(cellValue);
            if (previousActions.Count > MAX_ACTION_HISTORY)
            {
                List<Action> newTempActionList = new List<Action>();
                newTempActionList.AddRange(previousActions.GetRange(previousActions.Count - MAX_ACTION_HISTORY + HISTORY_CLEAR_INTERVAL, MAX_ACTION_HISTORY - HISTORY_CLEAR_INTERVAL));
                previousActions = newTempActionList;
                List<int> newTempCellList = new List<int>();
                newTempCellList.AddRange(previousCells.GetRange(previousCells.Count - MAX_ACTION_HISTORY + HISTORY_CLEAR_INTERVAL, MAX_ACTION_HISTORY - HISTORY_CLEAR_INTERVAL));
                previousCells = newTempCellList;
            }
            return lastAction;
        }

        public Action getNextAction(GameTime gameTime)
        {
            if (collectSpawnPoints == null)
                firstTimeSetup();

            Objective nextObjective = getNextObjective();

            nextObjective = validateObjective(nextObjective, gameTime);

            if (nextObjective.type == ObjectiveType.Nothing)
            {
                return setLastAction(new Action(ActionType.Nothing, GameObject.Direction.Neutral), nextObjective);
            }
            else if (nextObjective.type == ObjectiveType.UseShield)
            {
                return setLastAction(new Action(ActionType.UseAction, GameObject.Direction.Neutral), nextObjective);
            }
            else if (nextObjective.type == ObjectiveType.KillPlayer || nextObjective.type == ObjectiveType.KillPlayerClose)
            {
                if (actor.getCurItem() != null &&
                    (actor.getCurItem().getCollectibleType() == Collectible.CollectibleType.Present
                    || actor.getCurItem().getCollectibleType() == Collectible.CollectibleType.Shield))
                {
                    if (nextObjective.path.Count == 0)
                    {
                        Vector2 dirTarget = otherPlayer.getPosition() - actor.getPosition();
                        GameObject.Direction attackDir = GameObject.Direction.Up;
                        if (dirTarget.Y == -1)
                            attackDir = GameObject.Direction.Up;
                        else if (dirTarget.Y == 1)
                            attackDir = GameObject.Direction.Down;
                        else if (dirTarget.X == -1)
                            attackDir = GameObject.Direction.Left;
                        else
                            attackDir = GameObject.Direction.Right;

                        if (lastAction.type != ActionType.Nothing)
                        {
                            return setLastAction(new Action(ActionType.Nothing, GameObject.Direction.Neutral), nextObjective);
                        }

                        return setLastAction(new Action(ActionType.SuckBlow, attackDir), nextObjective);
                    }
                    else
                    {
                        if (lastAction.type != ActionType.Nothing && lastAction.type != ActionType.Move)
                        {
                            return setLastAction(new Action(ActionType.Nothing, GameObject.Direction.Neutral), nextObjective);
                        }

                        return setLastAction(new Action(ActionType.Move, nextObjective.path[0]), nextObjective);
                    }
                } 
                else if (actor.hasEffect(StatusEffect.EffectType.Link))
                {
                    if (nextObjective.path.Count == 0)
                    {
                        Vector2 dirTarget = otherPlayer.getPosition() - actor.getPosition();
                        GameObject.Direction attackDir = GameObject.Direction.Up;
                        if (dirTarget.Y == -1)
                            attackDir = GameObject.Direction.Up;
                        else if (dirTarget.Y == 1)
                            attackDir = GameObject.Direction.Down;
                        else if (dirTarget.X == -1)
                            attackDir = GameObject.Direction.Left;
                        else
                            attackDir = GameObject.Direction.Right;

                        if (lastAction.type != ActionType.Nothing)
                        {
                            return setLastAction(new Action(ActionType.Nothing, GameObject.Direction.Neutral), nextObjective);
                        }

                        return setLastAction(new Action(ActionType.Attack, attackDir), nextObjective);
                    }
                    else
                    {
                        if (lastAction.type != ActionType.Nothing && lastAction.type != ActionType.Move)
                        {
                            return setLastAction(new Action(ActionType.Nothing, GameObject.Direction.Neutral), nextObjective);
                        }

                        return setLastAction(new Action(ActionType.Move, nextObjective.path[0]), nextObjective);
                    }

                }
            }
            else
            {
                if (lastAction.type != ActionType.Nothing && lastAction.type != ActionType.Move)
                {
                    return setLastAction(new Action(ActionType.Nothing, GameObject.Direction.Neutral), nextObjective);
                }

                if(nextObjective.path.Count > 0)
                    return setLastAction(new Action(ActionType.Move, nextObjective.path[0]), nextObjective);
            }

            return setLastAction(new Action(ActionType.Nothing, GameObject.Direction.Neutral), nextObjective);

            /*if (actor.getPosition().Equals(goalPos))
            {
                if (goalPos.Equals(new Vector2(1, 0)))
                {
                    goalPos = new Vector2(9, 9);
                }
                else
                {
                    goalPos = new Vector2(1, 0);
                }
            }

            List<GameObject.Direction> directions = astar.getPath(actor, levelRef.getBlockAt(goalPos), levelRef);

            if (directions.Count == 0 || directions[0] == GameObject.Direction.Neutral)
                return new Action(ActionType.Nothing, GameObject.Direction.Neutral);

            return new Action(ActionType.Move, directions[0]);//getNextDirection());*/
        }

        private void firstTimeSetup()
        {
            collectSpawnPoints = new List<Vector2>();

            Block[,] map = levelRef.getMap();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j].getBlockType() == Block.BlockType.CollectibleSpawner
                        || map[i, j].getBlockType() == Block.BlockType.SpawnerDesert
                        || map[i, j].getBlockType() == Block.BlockType.SpawnerNinja
                        || map[i, j].getBlockType() == Block.BlockType.SpawnerSand)
                        collectSpawnPoints.Add(map[i, j].getPosition());
                }
            }
        }

        // this method validates the current objective to test for infinite loops
        public Objective validateObjective(Objective nextObjective, GameTime gameTime)
        {
            // ignore new objectives
            if (lastObjective.type != nextObjective.type)
            {
                objectiveCount = 1;
                pauseTimer = null;
                return nextObjective;
            }


            // infinite loop possible do something about it
            if (objectiveCount >= 8 && nextObjective.type != ObjectiveType.Nothing && 
                nextObjective.path.Count > 0 && nextObjective.path[0] != GameObject.Direction.Neutral)
            {
                if (pauseTimer != null)
                {
                    pauseTimer.update(gameTime);
                    if (pauseTimer.wasTriggered())
                    {
                        pauseTimer = null;
                    }
                    else
                    {
                        List<GameObject.Direction> path = new List<GameObject.Direction>();
                        path.Add(GameObject.Direction.Neutral);
                        return new Objective(ObjectiveType.Nothing, new Vector2(0, 0), path);
                    }
                }

                Vector2 targetCell = levelRef.getMoveFromDirection(actor, nextObjective.path[0]);
                int cellValue = (int)(levelRef.getMap().GetLength(0) * targetCell.Y + targetCell.X);

                for(int i = previousCells.Count - 1; i >= 0 && i >= previousCells.Count - objectiveCount; i--)
                {
                    if (previousCells[i] == cellValue)
                    {
                        //for (int j = i-1; i >= 0 && j >= previousCells.Count - objectiveCount; j--)
                        //{
                            // TODO need to test if this code will actually work properly
                        int targetCheck = i - (previousCells.Count - i);
                        if (targetCheck >= 0 && previousCells[targetCheck] == cellValue)
                        {
                            GameObject.Direction nextDir = getNextRandomDirection(nextObjective.path[0]);
                            Vector2 nextPoint = levelRef.getMoveFromDirection(actor, nextDir);
                            if(nextPoint == targetCell)
                                break;

                            if (levelRef.getSharedRandom().NextDouble() > 0.6)
                            {
                                pauseTimer = new Timer(250);
                                List<GameObject.Direction> path = new List<GameObject.Direction>();
                                path.Add(GameObject.Direction.Neutral);
                                return new Objective(ObjectiveType.Nothing, new Vector2(0, 0), path);

                            }
                            else
                            {
                                List<GameObject.Direction> path = new List<GameObject.Direction>();
                                path.Add(nextDir);
                                Objective modifiedObjective = new Objective(ObjectiveType.IdleTask, nextPoint, path);
                                objectiveCount = 1;
                                return modifiedObjective;
                            }
                        }
                        //}

                        break;
                    }
                }
            }

            // okay so it wasn't really an infinite loop if we get to here
            pauseTimer = null;
            objectiveCount++;
            return nextObjective;
        }

        private Objective getNextObjective()
        {
            Objective bestObjective = new Objective(ObjectiveType.Nothing, new Vector2(-1,-1), new List<GameObject.Direction>());

            foreach(Vector2 spawn in collectSpawnPoints)
            {
                Block b = levelRef.getBlockAt(spawn);
                if(b.getChild() != null && b.getChild().getType() == GameObject.Type.Collectible)
                {
                    ObjectiveType objectiveType = ObjectiveType.Nothing;
                    List<GameObject.Direction> path = astar.getPath(actor, b, levelRef);

                    switch(((Collectible)b.getChild()).getCollectibleType())
                    {
                        case Collectible.CollectibleType.Coin:
                            if(path.Count < 3)
                                objectiveType = ObjectiveType.ScoreShort;
                            else
                                objectiveType = ObjectiveType.ScoreFar;
                            break;
                        case Collectible.CollectibleType.Health:
                            if(actor.needHealth())
                                objectiveType = ObjectiveType.Survival;
                            else
                                objectiveType = ObjectiveType.GetItem;
                            break;
                        case Collectible.CollectibleType.Shield:
                            if (actor.getCurItem() == null)
                                objectiveType = ObjectiveType.Survival;
                            break;
                        case Collectible.CollectibleType.LinkGear: 
                            StatusEffect link = actor.getEffect(StatusEffect.EffectType.Link);
                            if(link == null || link.getTimeInSeconds() < 10)
                                objectiveType = ObjectiveType.Weapon;
                            else
                                objectiveType = ObjectiveType.SuperLowPriority;
                            break;
                        case Collectible.CollectibleType.Present:
                            if(actor.getCurItem() == null)
                                objectiveType = ObjectiveType.Weapon;
                            break;
                        case Collectible.CollectibleType.Speed:
                            if(path.Count < 3)
                                objectiveType = ObjectiveType.GetItemClose;
                            else
                                objectiveType = ObjectiveType.GetItem;
                            break;
                        case Collectible.CollectibleType.Star:
                            objectiveType = ObjectiveType.HighScore;
                            break;
                    }

                    if((int)objectiveType < (int)bestObjective.type ||
                        ((int)objectiveType == (int)bestObjective.type 
                            && path.Count < bestObjective.path.Count))
                    {
                        bestObjective = new Objective(objectiveType, spawn, path);
                    }
                }
            }

            double modThreat = actor.getThreat();
            if (actor.getCurItem() != null && actor.getCurItem().getCollectibleType() == Collectible.CollectibleType.Present)
            {
                modThreat += 10 * ((int)levelRef.getAppRef().getGameOptions().difficulty);
            }

            // if the actor can attack the other player
            if((actor.hasEffect(StatusEffect.EffectType.Link) || 
                (actor.getCurItem() != null &&
                    (actor.getCurItem().getCollectibleType() == Collectible.CollectibleType.Present
                    || (actor.getCurItem().getCollectibleType() == Collectible.CollectibleType.Shield
                          && actor.hasEffect(StatusEffect.EffectType.Shield) 
                          && actor.getEffect(StatusEffect.EffectType.Shield).getShieldCount() >= 3))))
                && (otherPlayer.getThreat() < actor.getThreat() || otherPlayer.getThreat() - actor.getThreat() < 8
                    || actor.getThreat()/otherPlayer.getThreat() > 0.6))
            {
                List<GameObject.Direction> pathPlayer = astar.getPath(actor, otherPlayer, levelRef);

                ObjectiveType objectiveType = ObjectiveType.KillPlayer;
                if(pathPlayer.Count < 3)
                    objectiveType = ObjectiveType.KillPlayerClose;

                if((int)objectiveType < (int)bestObjective.type)
                {
                    bestObjective = new Objective(objectiveType, otherPlayer.getPosition(), pathPlayer);
                }
            }
            
            if (actor.getCurItem() != null &&
                    actor.getCurItem().getCollectibleType() == Collectible.CollectibleType.Shield
                    && (!actor.hasEffect(StatusEffect.EffectType.Shield)
                          || actor.getEffect(StatusEffect.EffectType.Shield).getShieldCount() < 3))
            {
                ObjectiveType objectiveType = ObjectiveType.UseShield;

                if ((int)objectiveType < (int)bestObjective.type)
                {
                    bestObjective = new Objective(objectiveType, otherPlayer.getPosition(), null);
                }
            }

            if (bestObjective.type == ObjectiveType.Nothing)
            {
                // TODO: Need to track for existing random objective
                //if(previousActions.Count >= 1 && previousActions[0].type == ActionType.Nothing

                bool allNothing = true;
                for (int i = previousActions.Count - 1; i >= 0 && i >= previousActions.Count - 5; i--)
                {
                    if (previousActions[i].type != ActionType.Nothing)
                    {
                        allNothing = false;
                        break;
                    }
                }

                if (allNothing)
                {
                    Vector2 point = collectSpawnPoints[levelRef.getSharedRandom().Next(collectSpawnPoints.Count)];
                    bestObjective = new Objective(ObjectiveType.IdleTask, point, astar.getPath(actor, levelRef.getBlockAt(point), levelRef));
                }
            }

            return bestObjective;
        }

        public GameObject.Direction getNextRandomDirection()
        {
            int tries = 15;
            while (tries > 0)
            {
                GameObject.Direction newDir = (GameObject.Direction)levelRef.getSharedRandom().Next(4);
                if (levelRef.canEnter(actor, actor.getPosition(), newDir))
                {
                    return newDir;
                }
                tries--;
            }
            return GameObject.Direction.Neutral;
        }

        public GameObject.Direction getNextRandomDirection(GameObject.Direction notThis)
        {
            int tries = 15;
            while (tries > 0)
            {
                GameObject.Direction newDir = (GameObject.Direction)levelRef.getSharedRandom().Next(4);
                if (newDir != notThis && levelRef.canEnter(actor, actor.getPosition(), newDir))
                {
                    return newDir;
                }
                tries--;
            }
            return GameObject.Direction.Neutral;
        }

    }
}
