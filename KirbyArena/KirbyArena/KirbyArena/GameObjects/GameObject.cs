using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class GameObject : AnimatedObject
    {
        #region Enum definitions
        /// <summary>
        /// A direction can be up, down, left, right, or neutral. 
        /// The direction property is mainly referring to the stance of 
        /// the monsters and player. But it is also used as a state variable
        /// for up to 5 states that different blocks can have.
        /// </summary>
        public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3, Neutral = 4 };

        /// <summary>
        /// The four types of game object are Block that are static, monsters that may move by themselves,
        /// and the player that is controlled by the players actions. The last type is Cursor that is for the 
        /// editor mode.
        /// </summary>
        public enum Type { Block = 0, Monster, Player, Collectible, Cursor };
        #endregion

        #region Instance Variables
        /// <summary>
        /// Where on the map of tiles this object is located.
        /// </summary>
        protected Vector2 position;

        /// <summary>
        /// The state of the object. For tiles it may be a rotation, 
        /// but for monsters and players in particular it will be a 
        /// specification of the image to be displayed.
        /// </summary>
        protected Direction facing;

        /// <summary>
        /// The dimensions of the object.
        /// </summary>
        protected Vector2 dimensions;

        /// <summary>
        /// The type of object. (Block/Monster/Player)
        /// </summary>
        protected Type type;

        /// <summary>
        /// Defines whether a general monster is allowed to enter from a particular direction.
        /// </summary>
        protected bool[] canEnterMonster;

        /// <summary>
        /// Defines whether a player is allowed to enter from a particular direction.
        /// </summary>
        protected bool[] canEnterPlayer;

        protected GameObject childObj;
        protected GameObject controllingObj;
        protected Level levelRef;
        protected List<Texture2D> spriteSheets;

        /// <summary>
        /// The progress of movement to another cell
        /// </summary>
        protected float transition;

        /// <summary>
        /// A multiplier value that will make the object move faster or slower.
        /// </summary>
        protected float speedMultiplier;

        /// <summary>
        /// The cell that is being transitioned to.
        /// </summary>
        protected Vector2 targetCell;

        /// <summary>
        /// Used for an automatic chained movement so allow everything to fluidly update.
        /// </summary>
        //protected Vector2 chainTargetCell;

        /// <summary>
        /// A counter to manage the time taken
        /// </summary>
        protected float moveDelay;

        /// <summary>
        /// The delay time that must be waited between moves.
        /// </summary>
        protected float moveDelayTime;
        #endregion



        public GameObject(List<Texture2D> spriteSheets, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : this(spriteSheets[0], position, dimensions, dest, level)
        {
            this.spriteSheets = spriteSheets;
        }

        public GameObject(Texture2D spriteSheet, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(spriteSheet, (int)dimensions.X, (int)dimensions.Y, dest)
        {
            this.position = position;
            this.dimensions = dimensions;
            this.facing = 0;
            this.levelRef = level;
            //setSprite(facing);
            canEnterMonster = new bool[4];
            canEnterPlayer = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                canEnterPlayer[i] = true;
                canEnterMonster[i] = true;
            }

            transition = 0.0f;
            speedMultiplier = 1.0f;
            targetCell = new Vector2(-1, -1);
            //chainTargetCell = new Vector2(-1, -1);
            moveDelay = 0;
            moveDelayTime = 100;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
            if(childObj != null)
                childObj.update(gameTime);

            if (isMoving())
            {
                transition += gameTime.ElapsedGameTime.Milliseconds / 250.0f * speedMultiplier;

                if (transition >= 1.0f)
                {
                    completeMoveTo();
                }
            }
        }

        public virtual void draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            int posx = (int)pos.X;
            int posy = (int)pos.Y;

            if (transition > 0.0f)
            {
                if (facing == Direction.Left || facing == Direction.Right)
                {
                    posx = (int)(pos.X + transition * dimensions.X * levelRef.getDirectionMultiplier(facing));
                }
                if (facing == Direction.Up || facing == Direction.Down)
                {
                    posy = (int)(pos.Y + transition * dimensions.Y * levelRef.getDirectionMultiplier(facing));
                }
            }

            dest = new Rectangle(posx, posy, (int)levelRef.getCellDimensions().X, (int)levelRef.getCellDimensions().Y);

            base.draw(spriteBatch);

            if(childObj != null)
                childObj.draw(spriteBatch, pos);
        }
        
        #region Set sprite methods
        /// <summary>
        /// Set the sprite to the specified sprite position.
        /// </summary>
        /// <param name="spriteID">The array id to get the sprite coordinates.</param>
        [Obsolete("Note that this is not used for this application it is a deprecated method from Chad's Challenge.")]
        protected void setSprite(int spriteID)
        {
            // Note that this is not used for this application it is a deprecated method from Chad's Challenge
        }

        /// <summary>
        /// Set the sprite using a Direction variable instead of the int.
        /// </summary>
        /// <param name="dir">A direction for the facing to be assumed as.</param>
        [Obsolete("Note that this is not used for this application it is a deprecated method from Chad's Challenge.")]
        protected void setSprite(Direction dir)
        {
            setSprite((int)dir);

        }
        #endregion
        
        #region Movement Modifiers
        /// <summary>
        /// Attempt to initiate movement in the specified direction.
        /// This may fail, it there is something that is blocking the path.
        /// </summary>
        /// <param name="dir">The direction to move the player.</param>
        protected virtual void move(GameObject.Direction dir)
        {
            setFacing(dir);
            levelRef.moveObject(this, dir);
        }
        
        /// <summary>
        /// Initiate a successful move call by setting the target cell and
        /// disabling player input. The player will now move by itself until 
        /// set otherwise.
        /// </summary>
        /// <param name="to">The cell that is being moved to.</param>
        public void beginMoveTo(Vector2 to)
        {
            targetCell = to;
            /*
            if (targetCell.X == -1)
            {
                if (!checkValidMove(getPosition(), to)) return;
                targetCell = to;
            }
            else
            {
                if (!checkValidMove(targetCell, to)) return;
                chainTargetCell = to;
            }*/
        }

        public bool beginCheckMoveTo(Vector2 to)
        {

            /*if (targetCell.X == -1)
            {
                if (!checkValidMove(getPosition(), to)) return false;
            }
            else
            {
                if (to.Equals(chainTargetCell) || !checkValidMove(targetCell, to)) return false;
            }*/
            return true;
        }

        /// <summary>
        /// This method is a "fix" XD
        /// </summary>
        /// <param name="from">Cell 1</param>
        /// <param name="to">Cell 2</param>
        /// <returns>Movement valid.</returns>
        private bool checkValidMove(Vector2 from, Vector2 to)
        {
            return true;
        }
        
        /// <summary>
        /// Complete the process of moving to a particular cell.
        /// This includes triggering of the relevant events.
        /// </summary>
        protected void completeMoveTo()
        {
            // trigger exited
            levelRef.triggerOnExited(position, this);

            position.X = targetCell.X;
            position.Y = targetCell.Y;
            transition = 0.0f;

            // trigger entered
            levelRef.triggerOnEntered(this);

            /*if (chainTargetCell.X == -1)
            {*/
                targetCell = new Vector2(-1, -1);
            /*}
            else
            {
                targetCell = chainTargetCell;
                chainTargetCell = new Vector2(-1, -1);
            }*/
        }

        /// <summary>
        /// Get the current target cell
        /// </summary>
        /// <returns>Target cell</returns>
        public Vector2 getTargetCell()
        {
            return targetCell;
        }


        /// <summary>
        /// Get the chained target cell
        /// </summary>
        /// <returns>Chained target cell</returns>
       /* public Vector2 getChainedTargetCell()
        {
            return chainTargetCell;
        }*/

        /// <summary>
        /// Set the speed multiplier for this object to a higher or lower value.
        /// </summary>
        /// <param name="multiplier">The value to make the speed.</param>
        public void setSpeedMulti(float multiplier)
        {
            speedMultiplier = multiplier;
        }

        /// <summary>
        /// Reset the speed multiplier to the default value.
        /// </summary>
        public void resetSpeedMultiplier()
        {
            speedMultiplier = 1.0f;
        }

        /// <summary>
        /// Is the object moving?
        /// </summary>
        /// <returns>Whether the object is moving</returns>
        public bool isMoving()
        {
            return targetCell.X != -1 && targetCell.Y != -1;
        }
        #endregion

        #region Set Methods
        /// <summary>
        /// Set the facing of this particular game object and update the sprite.
        /// </summary>
        /// <param name="facing">The new direction of face.</param>
        public virtual void setFacing(Direction facing)
        {
            this.facing = facing;
            setSprite(facing);
        }

        /// <summary>
        /// This method will set which directions the player can enter this square from.
        /// </summary>
        /// <param name="canEnter">The boolean checks for if a direction is allowed.</param>
        public void setCanEnterPlayer(bool[] canEnter)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterPlayer[i] = canEnter[i];
            }
        }

        /// <summary>
        /// This method will set which directions a monster can enter this square from.
        /// </summary>
        /// <param name="canEnter">The boolean checks for if a direction is allowed.</param>
        public void setCanEnterMonster(bool[] canEnter)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterMonster[i] = canEnter[i];
            }
        }

        /// <summary>
        /// Set it so that no player can enter this object.
        /// </summary>
        /// <param name="setting">The boolean checks for if a direction is allowed.</param>
        public void setCanEnterPlayerAll(bool setting)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterPlayer[i] = setting;
            }
        }

        /// <summary>
        /// Set it so that no monster can enter this object.
        /// </summary>
        /// <param name="setting">The boolean checks for if a direction is allowed.</param>
        public void setCanEnterMonsterAll(bool setting)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterMonster[i] = setting;
            }
        }

        /// <summary>
        /// Set the position of the object to a new coordinate.
        /// </summary>
        /// <param name="pos">The position to place the object at.</param>
        public void setPosition(Vector2 pos)
        {
            this.position = pos;
        }

        public void setChild(GameObject childObj)
        {
            if (getChild() != null)
                onChildRemoved();

            this.childObj = childObj;

            if (getChild() != null)
                onChildAdded();
        }

        public void setControllingObj(GameObject controller)
        {
            this.controllingObj = controller;
        }
        #endregion

        #region Get State Variables
        /// <summary>
        /// Gets the current direction of facing or state of the object.
        /// </summary>
        /// <returns>The current direction or state.</returns>
        public Direction getFacing()
        {
            return facing;
        }

        /// <summary>
        /// Gets the type of the object (block/monster/player)
        /// </summary>
        /// <returns>The object type.</returns>
        public Type getType()
        {
            return type;
        }

        /// <summary>
        /// Gets the position of the object.
        /// </summary>
        /// <returns>The objects position.</returns>
        public Vector2 getPosition()
        {
            return this.position;
        }

        public GameObject getChild()
        {
            return childObj;
        }

        public GameObject getControllingObj()
        {
            return controllingObj;
        }
        #endregion

        #region Virtual Events that may be overridden to handle interactions
        /// <summary>
        /// This event is triggered when another object is moving into 
        /// this object.
        /// </summary>
        /// <param name="obj">The object that is moving.</param>
        /// <param name="dir">The direction they are entering from.</param>
        public virtual void onEntering(GameObject obj, Direction dir)
        {
            if(obj == this) return;

            if (controllingObj != null)
            {
                controllingObj.onEntering(obj, dir);
            }
            else if (obj.getType() == Type.Monster || obj.getType() == Type.Player)
            {
                setControllingObj(obj);
            }

            if (childObj != null)
                childObj.onEntering(obj, dir);
        }

        /// <summary>
        /// This event is triggered once entry into the object is completed.
        /// </summary>
        /// <param name="obj">The object that has now entered this object's area.</param>
        public virtual void onEntered(GameObject obj)
        {
            if (obj == this) return;

            if (controllingObj != null)
            {
                controllingObj.onEntered(obj);
            }
            else if (obj.getType() == Type.Monster || obj.getType() == Type.Player)
            {
                setControllingObj(obj);
            }

            if (childObj != null)
                childObj.onEntered(obj);
        }

        /// <summary>
        /// This event is triggered as an object is exiting the object.
        /// </summary>
        /// <param name="obj">The object exiting.</param>
        /// <param name="dir">The direction that they are exiting to.</param>
        public virtual void onExiting(GameObject obj, Direction dir)
        {
            if (obj == this) return;

            if (controllingObj != null)
            {
                controllingObj.onExiting(obj, dir);
            }

            if (childObj != null)
                childObj.onExiting(obj, dir);
        }

        /// <summary>
        /// This event is triggered after an object has left this object.
        /// </summary>
        /// <param name="obj">The object that has exited.</param>
        public virtual void onExited(GameObject obj)
        {
            if (obj == this) return;

            if (controllingObj == obj)
            {
                setControllingObj(null);
            }
            else if (controllingObj != null)
            {
                controllingObj.onExited(obj);
            }

            if (childObj != null)
                childObj.onExited(obj);
        }

        /// <summary>
        /// Check if the object obj can enter this element from direction dir.
        /// Depth allows for composite advanced checks. A non-zero value will 
        /// allow recursive calls that test multiple squares in direction dir.
        /// Each successive call decreases depth by 1. The call is terminated if 
        /// a square is found to have false entry.
        /// This is not implemented by this virtual function, but it is the purpose
        /// that is will contain within the relevant classes inheriting.
        /// </summary>
        /// <param name="obj">The object wanting to test for entry.</param>
        /// <param name="dir">The direction they wish to enter from.</param>
        /// <param name="depth">The maximum depth check for a composite entry test.</param>
        /// <returns>Can the object enter the cell.</returns>
        public virtual bool canEnter(GameObject obj, Direction dir, int depth)
        {
            if (obj.getType() == GameObject.Type.Player)
            {
                if (this.getType() == GameObject.Type.Player && obj != this)
                    return false;

                return canEnterPlayer[(int)dir] && (childObj == null || childObj.canEnter(obj, dir)) 
                    && (controllingObj == null || controllingObj.canEnter(obj, dir));
            }
            else
            {
                return canEnterMonster[(int)dir] && (childObj == null || childObj.canEnter(obj, dir))
                    && (controllingObj == null || controllingObj.canEnter(obj, dir));
            }
        }

        /// <summary>
        /// Checks if the object obj can enter this element from direction dir.
        /// </summary>
        /// <param name="obj">The object wanting to test for entry.</param>
        /// <param name="dir">The direction they wish to enter from.</param>
        /// <returns>Can the object enter the cell.</returns>
        public virtual bool canEnter(GameObject obj, Direction dir)
        {
            return canEnter(obj, dir, 0);
        }

        /// <summary>
        /// Used to check if the object currently on the square can exit the square.
        /// </summary>
        /// <param name="obj">The object on this object.</param>
        /// <param name="dir">The direction to leave.</param>
        /// <returns></returns>
        public virtual bool canExit(GameObject obj, Direction dir)
        {
            return canEnter(obj, dir);
        }

        public virtual void onChildAdded()
        {

        }

        public virtual void onChildRemoved()
        {

        }

        public virtual void onLevelLoaded()
        {

        }
        #endregion
    }
}
