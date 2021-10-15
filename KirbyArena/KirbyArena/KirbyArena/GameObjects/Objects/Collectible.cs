using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class Collectible : GameObject
    {
        public enum CollectibleType { LinkGear = 0, Speed = 1, Health = 2, Coin = 3, Star = 4, Shield = 5, Present = 6  };

        protected CollectibleType collectibleType;
        protected Vector2 targetPoint, startPoint, startDim;
        protected Timer trackingToPoint;
        protected bool shrink;
        protected Vector2 newPosition;
        protected Player parent;

        public Collectible(CollectibleType type, Texture2D spriteSheet, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(spriteSheet, position, dimensions, dest, level)
        {
            this.collectibleType = type;
            this.type = Type.Collectible;
            targetPoint = new Vector2(-1, -1);
            shrink = false;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (targetPoint.X != -1 && trackingToPoint != null)
            {
                trackingToPoint.update(gameTime);
                if (!trackingToPoint.wasTriggered())
                {
                    float distanceX = targetPoint.X - startPoint.X;
                    float distanceY = targetPoint.Y - startPoint.Y;
                    dest.X = (int)(startPoint.X + distanceX * trackingToPoint.getTimePercent());
                    dest.Y = (int)(startPoint.Y + distanceY * trackingToPoint.getTimePercent());

                    if (shrink)
                    {
                        dest.Width = (int)(startDim.X - startDim.X * trackingToPoint.getTimePercent());
                        dest.Height = (int)(startDim.Y - startDim.Y * trackingToPoint.getTimePercent());
                    }
                    else
                    {
                        Player otherPlayer = (levelRef.getPlayer(1) == parent) ? levelRef.getPlayer(2) : levelRef.getPlayer(1);
                        if (otherPlayer != null && otherPlayer.getRect().Intersects(getRect()))
                        {
                            if (collectibleType == CollectibleType.Present)
                            {
                                otherPlayer.damage(0.5);
                                otherPlayer.stun();
                            }
                            else if (collectibleType == CollectibleType.Shield)
                            {
                                otherPlayer.damage(0.5);
                            }
                            parent.clearCollectible();
                        }

                        dest.Width = (int)(startDim.X - startDim.X * (1 - trackingToPoint.getTimePercent()));
                        dest.Height = (int)(startDim.Y - startDim.Y * (1 - trackingToPoint.getTimePercent()));
                    }
                }
                else
                {
                    trackingToPoint = null;
                    if (!shrink)
                    {
                        setPosition(newPosition);
                        setDest(levelRef.getRect((int)newPosition.X, (int)newPosition.Y));
                        Block block = levelRef.getBlockAt(getPosition());

                        if (collectibleType == CollectibleType.Present)
                        {
                            block.setChild(levelRef.createCollectible(block.getPosition(), false));
                        }
                        else if (collectibleType == CollectibleType.Shield)
                        {
                            // do nothing to the target block
                        }
                        else
                        {
                            block.setChild(this);
                        }

                        parent.clearCollectible();
                    }
                }
            }
            else
            {
                Player p = levelRef.getPlayer(1);
                if (p.getRect().Contains(getRect().Center) && !p.isMoving() && !p.isAnimating())
                {
                    if(p.canCollect(this, false))
                        p.collect(this, false);
                }

                Player p2 = levelRef.getPlayer(2);
                if (p2 != null && p2.getRect().Contains(getRect().Center) && !p2.isMoving() && !p2.isAnimating())
                {
                    if (p2.canCollect(this, false))
                        p2.collect(this, false);
                }
            }
        }

        public override void  draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            if (isCollectibleMoving())
                base.draw(spriteBatch);
            else
 	            base.draw(spriteBatch, pos);
        }

        /*public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Player)
            {
                ((Player)obj).collect(this, false);
            }
        }*/

        public void setNewTargetPosition(Vector2 position)
        {
            this.newPosition = position;
        }

        public Vector2 getTarget()
        {
            return targetPoint;
        }

        public void setParent(Player parent)
        {
            this.parent = parent;
        }

        public bool isCollectibleMoving()
        {
            return targetPoint.X != -1;
        }

        public void setTarget(Vector2 targetPoint, Vector2 startPoint, int time, bool shrink)
        {
            this.shrink = shrink;
            if (!shrink)
            {
                dest.Width = 5;
                dest.Height = 5;
                Rectangle targetRect = levelRef.getRect(0, 0);
                this.startDim = new Vector2(targetRect.Width, targetRect.Height);
            }
            else
            {
                this.startDim = new Vector2(dest.Width, dest.Height);
            }

            //this.startDim = new Vector2(dest.Width, dest.Height);

            if (startPoint.X == -1)
            {
                this.startPoint = new Vector2(dest.X, dest.Y);
            }
            else
            {
                dest.X = (int)startPoint.X;
                dest.Y = (int)startPoint.Y;
                this.startPoint = startPoint;
            }
            this.targetPoint = targetPoint;
            trackingToPoint = new Timer(time);
        }

        public CollectibleType getCollectibleType()
        {
            return collectibleType;
        }
    }
}
