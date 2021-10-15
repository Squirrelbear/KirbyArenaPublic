using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KirbyArena
{
    public class Player : GameObject
    {
        public enum KirbyStates
        {
            Neutral, NeutralSucked,
            Walk, Stun, Suck, SuckReceived, SuckWalk, Blow,
            NeutralLink, NeutralLinkSucked,
            LinkBlow, LinkSuck, LinkSuckReceived, LinkSuckWalk, LinkWalk, LinkAttack
        }
        
        private PlayerIndex playerIndex;
        private InputManager inputManager;
        private bool inputEnabled = true;
        private List<AnimatedObject> animations;
        private AnimatedObject curAnim;
        private KirbyStates curState;
        private int atkDimMod;
        private bool waitForAnim;
        private double lives, maxLives;
        private Timer actionTimer;
        private Collectible targetPickup;
        private List<StatusEffect> statusEffects;
        private int score;
        private InputAssociation kbInput;
        private bool damageApplied;
        private double threatRating;

        public Player(PlayerIndex playerIndex, InputAssociation keyboardInput, List<Texture2D> spriteSet, Vector2 position, Vector2 dimensions, Rectangle dest, Level level)
            : base(spriteSet, position, dimensions, dest, level)
        {
            inputManager = levelRef.getAppRef().getInputManager();
            type = Type.Player;
            this.playerIndex = playerIndex;
            this.kbInput = keyboardInput;
            score = 0;
            threatRating = 0;
            statusEffects = new List<StatusEffect>();

            animations = new List<AnimatedObject>();

            AnimatedObject walk = new AnimatedObject(spriteSet[0], 256, 256, dest);
            walk.setFrameTime(250 / 7);
            AnimatedObject stun = new AnimatedObject(spriteSet[1], 256, 256, dest);
            stun.setFrameTime(250 / 7);
            AnimatedObject suck = new AnimatedObject(spriteSet[2], 256, 256, dest);
            AnimatedObject suckWalk = new AnimatedObject(spriteSet[3], 256, 256, dest);
            suckWalk.setFrameTime(250 / 7);
            AnimatedObject blow = new AnimatedObject(spriteSet[4], 256, 256, dest);
            blow.setFrameTime(1000 / 5);
            AnimatedObject linkBlow = new AnimatedObject(spriteSet[5], 256, 256, dest);
            linkBlow.setFrameTime(1000 / 5);
            AnimatedObject linkSuck = new AnimatedObject(spriteSet[6], 256, 256, dest);
            AnimatedObject linkSuckWalk = new AnimatedObject(spriteSet[7], 256, 256, dest);
            linkSuckWalk.setFrameTime(250 / 7);
            AnimatedObject linkWalk = new AnimatedObject(spriteSet[8], 256, 256, dest);
            linkWalk.setFrameTime(250 / 7);
            AnimatedObject linkAttack = new AnimatedObject(spriteSet[9], 300, 300, dest);
            linkAttack.setFrameTime(250 / 2);

            animations.Add(walk);
            animations.Add(stun);
            animations.Add(suck);
            animations.Add(suckWalk);
            animations.Add(blow);
            animations.Add(linkBlow);
            animations.Add(linkSuck);
            animations.Add(linkSuckWalk);
            animations.Add(linkWalk);
            animations.Add(linkAttack);

            // (300-256) / 2 = 22
            atkDimMod = (int)(dimensions.X / 256.0 * 22);

            // disable the default rendering method
            waitForAnim = false;
            setVisible(false);
            setState(KirbyStates.Neutral);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            if (targetPickup != null && (curState == KirbyStates.LinkSuck || curState == KirbyStates.Suck
                                           || curState == KirbyStates.Blow || curState == KirbyStates.LinkBlow))
                targetPickup.draw(spriteBatch);

            base.draw(spriteBatch, pos);
            if (curState == KirbyStates.LinkAttack)
            {
                setSize(dimensions.X + atkDimMod * 2, dimensions.Y + atkDimMod * 2);
                if (facing != Direction.Right)
                {
                    dest.X -= atkDimMod;
                }

                if (facing != Direction.Down && facing != Direction.Neutral)
                {
                    dest.Y -= atkDimMod;
                }
            }
            curAnim.setDest(dest);
            curAnim.draw(spriteBatch);

            if (hasEffect(StatusEffect.EffectType.Shield))
            {
                Texture2D shieldTexture = levelRef.getCollectibleSprites()[(int)Collectible.CollectibleType.Shield];
                spriteBatch.Draw(shieldTexture, dest, 
                        Color.White * (0.4f + getEffect(StatusEffect.EffectType.Shield).getShieldCount() * 0.1f));
            }
        }

        public override void update(GameTime gameTime)
        {
            // update the current state variables
            // prevent the code from continuing with the "false" return if required.
            if (!updateState(gameTime))
                return;

            if (canMove(gameTime))
            {
                if (curState == KirbyStates.NeutralLink && testPlayerActionAttack())
                {
                    beginAttack(getPlayerDirection());
                }
                else if (testPlayerActionSuckBlow())
                {
                    beginSuckBlow(getPlayerDirection());
                }
                else if (getPlayerDirection() != Direction.Neutral && 
                           (curState == KirbyStates.Neutral || curState == KirbyStates.NeutralLink
                            || curState == KirbyStates.NeutralLinkSucked || curState == KirbyStates.NeutralSucked
                            || curState == KirbyStates.SuckWalk || curState == KirbyStates.LinkSuckWalk
                            || curState == KirbyStates.Walk || curState == KirbyStates.LinkWalk))
                {
                    move(getPlayerDirection());
                }
                else if ((curState == KirbyStates.NeutralLinkSucked || curState == KirbyStates.NeutralSucked)
                         && targetPickup != null && targetPickup.getCollectibleType() == Collectible.CollectibleType.Shield
                         && testPlayerActionUseAction())
                {
                    activateShield();
                }
                else
                {
                    returnToNeutral();
                }
            }
        }

        public bool updateState(GameTime gameTime)
        {
            base.update(gameTime);

            foreach (StatusEffect effect in statusEffects)
            {
                effect.update(gameTime);
            }

            curAnim.update(gameTime);
            if (targetPickup != null && (curState == KirbyStates.LinkBlow || curState == KirbyStates.LinkSuck
                                        || curState == KirbyStates.Blow || curState == KirbyStates.Suck))
                targetPickup.update(gameTime);

            updateThreat();

            if (curState == KirbyStates.Suck || curState == KirbyStates.LinkSuck)
            {
                actionTimer.update(gameTime);
                if (actionTimer.wasTriggered())
                {
                    if (curState == KirbyStates.Suck)
                        setState(KirbyStates.SuckReceived);
                    else
                        setState(KirbyStates.LinkSuckReceived);

                    collect(targetPickup, false);
                }
                return false;
            }
            else if (curState == KirbyStates.Blow || curState == KirbyStates.LinkBlow)
            {
                actionTimer.update(gameTime);
                if (!actionTimer.wasTriggered() && targetPickup != null)
                    return false;
                else
                {
                    //targetPickup = null;
                    if (curState == KirbyStates.Blow)
                        setState(KirbyStates.Neutral);
                    else
                        setState(KirbyStates.NeutralLink);
                }
            }
            else if (curState == KirbyStates.Stun)
            {
                actionTimer.update(gameTime);
                if (!actionTimer.wasTriggered())
                    return false;
                else if (targetPickup == null)
                    setState(KirbyStates.Neutral);
                else // TODO: Need to check if this will actually fix the issue
                    setState(KirbyStates.NeutralSucked);
            }
            else if (curState == KirbyStates.LinkAttack)
            {
                if (!damageApplied && curAnim.getFrame() > frameOffset(curAnim, facing) + 4)
                {
                    Block targetBlock = levelRef.getBlockAt(levelRef.getMoveFromDirection(this, facing));
                    if (targetBlock != null)
                    {
                        GameObject controller = targetBlock.getControllingObj();
                        if (controller != null && controller.getType() == Type.Player)
                        {
                            Player player = (Player)controller;
                            player.damage(0.5);
                        }
                    }

                    damageApplied = true;
                }
            }
            else if (targetPickup == null && (curState == KirbyStates.NeutralLinkSucked
                || curState == KirbyStates.NeutralSucked))
            {
                if (hasEffect(StatusEffect.EffectType.Link))
                    setState(KirbyStates.NeutralLink);
                else
                    setState(KirbyStates.Neutral);
            }

            if (inputEnabled)
            {
                List<StatusEffect> removeList = new List<StatusEffect>();
                foreach (StatusEffect effect in statusEffects)
                {
                    if (effect.isExpired())
                    {
                        effect.onEnd();
                        removeList.Add(effect);
                    }
                }
                foreach (StatusEffect effect in removeList)
                {
                    statusEffects.Remove(effect);
                }
            }

            return true;
        }

        public bool canMove(GameTime gameTime)
        {
            if (inputEnabled)
            {
                moveDelay += gameTime.ElapsedGameTime.Milliseconds;
                if (moveDelay > moveDelayTime)
                {
                    moveDelay = moveDelayTime;
                    return true;
                }
            }
            else
            {
                if (!isMoving() && (!waitForAnim || (waitForAnim && !curAnim.isAnimating())))
                {
                    inputEnabled = true;
                }
            }

            return false;
        }

        public bool beginAttack(Direction dir)
        {
            if (dir == Direction.Neutral)
                return false;

            setFacing(dir);

            moveDelay = 0;
            inputEnabled = false;
            setState(KirbyStates.LinkAttack);

            damageApplied = false;

            return true;
        }

        public bool beginSuckBlow(Direction dir)
        {
            if (dir == Direction.Neutral)
                return false;

            setFacing(dir);

            if (curState == KirbyStates.Neutral || curState == KirbyStates.NeutralLink)
            {
                Block nextBlock = levelRef.getBlockAt(levelRef.getMoveFromDirection(this, facing));
                if (nextBlock == null)
                    return false;

                Collectible targetGrab = null;
                if (nextBlock.getChild() != null && nextBlock.getChild().getType() == Type.Collectible
                    && levelRef.canEnter(this, nextBlock.getPosition(), facing))
                {
                    targetGrab = (Collectible)nextBlock.getChild();
                }
                else if (nextBlock.getChild() == null)
                {
                    nextBlock = levelRef.getBlockAt(levelRef.getMoveFromDirection(nextBlock, facing));
                    if (nextBlock != null && nextBlock.getChild() != null && nextBlock.getChild().getType() == Type.Collectible
                        && levelRef.canEnter(this, nextBlock.getPosition(), facing))
                    {
                        targetGrab = (Collectible)nextBlock.getChild();
                    }
                }

                if (targetGrab == null)
                    return false;

                if (!canCollect(targetGrab, true))
                    return false;

                collect(targetGrab, true);

                moveDelay = 0;
                inputEnabled = false;
                if (curState == KirbyStates.Neutral)
                    setState(KirbyStates.Suck);
                else if (curState == KirbyStates.NeutralLink)
                    setState(KirbyStates.LinkSuck);
            }
            else if (targetPickup != null && (curState == KirbyStates.NeutralLinkSucked || curState == KirbyStates.NeutralSucked))
            {
                Block nextBlock = levelRef.getBlockAt(levelRef.getMoveFromDirection(this, facing));
                if (nextBlock == null)
                    return false;

                if (levelRef.canEnter(targetPickup, nextBlock.getPosition(), facing))
                {
                    Block blockAfter = levelRef.getBlockAt(levelRef.getMoveFromDirection(nextBlock, facing));
                    if (blockAfter != null && levelRef.canEnter(targetPickup, blockAfter.getPosition(), facing))
                    {
                        nextBlock = blockAfter;
                    }
                }
                else
                {
                    return false;
                }

                moveDelay = 0;
                inputEnabled = false;
                if (curState == KirbyStates.NeutralSucked)
                    setState(KirbyStates.Blow);
                else if (curState == KirbyStates.NeutralLinkSucked)
                    setState(KirbyStates.LinkBlow);

                targetPickup.setTarget(new Vector2(nextBlock.getRect().X, nextBlock.getRect().Y),
                                       new Vector2(dest.Center.X, dest.Center.Y), 1000, false);
                targetPickup.setParent(this);
                targetPickup.setNewTargetPosition(nextBlock.getPosition());
            }

            return true;
        }

        public void activateShield()
        {
            if (targetPickup == null || targetPickup.getCollectibleType() != Collectible.CollectibleType.Shield)
                return;

            addEffect(StatusEffect.EffectType.Shield, 4);
            clearCollectible();
            if (curState != KirbyStates.NeutralLinkSucked)
                setState(KirbyStates.Neutral);
            else
                setState(KirbyStates.NeutralLink);
        }

        public void returnToNeutral()
        {
            setFacing(Direction.Neutral);
            if (curState == KirbyStates.SuckReceived
                || curState == KirbyStates.SuckWalk
                || curState == KirbyStates.NeutralSucked)
                setState(KirbyStates.NeutralSucked);
            else if (curState == KirbyStates.LinkSuckReceived
                || curState == KirbyStates.NeutralLinkSucked
                || curState == KirbyStates.LinkSuckWalk)
                setState(KirbyStates.NeutralLinkSucked);
            else if (curState == KirbyStates.LinkAttack
                || curState == KirbyStates.LinkBlow
                || curState == KirbyStates.LinkWalk
                || curState == KirbyStates.NeutralLink)
                setState(KirbyStates.NeutralLink);
            else if (curState == KirbyStates.Stun
                || curState == KirbyStates.Walk
                || curState == KirbyStates.Blow
                || curState == KirbyStates.Neutral)
                setState(KirbyStates.Neutral);
        }

        public Direction getPlayerDirection()
        {
            if ((inputManager.isKeyDown(kbInput.KeyMoveUp)) || inputManager.getThumbStickStateLeftY((int)playerIndex + 1) > 0.5)
            {
                return Direction.Up;
            }
            else if ((inputManager.isKeyDown(kbInput.KeyMoveDown)) || inputManager.getThumbStickStateLeftY((int)playerIndex + 1) < -0.5)
            {
                return Direction.Down;
            }
            else if ((inputManager.isKeyDown(kbInput.KeyMoveLeft)) || inputManager.getThumbStickStateLeftX((int)playerIndex + 1) < -0.5)
            {
                return Direction.Left;
            }
            else if ((inputManager.isKeyDown(kbInput.KeyMoveRight)) || inputManager.getThumbStickStateLeftX((int)playerIndex + 1) > 0.5)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Neutral;
            }
        }

        public bool testPlayerActionAttack()
        {
            return (inputManager.isKeyDown(kbInput.KeyAttack) || inputManager.isBtnDown(Buttons.X, (int)playerIndex + 1));
        }

        public bool testPlayerActionSuckBlow()
        {
            return inputManager.isKeyDown(kbInput.KeySuckBlow) || inputManager.isBtnDown(Buttons.B, (int)playerIndex + 1);
        }

        public bool testPlayerActionUseAction()
        {
            return inputManager.isKeyDown(kbInput.KeyAction) || inputManager.isBtnDown(Buttons.Y, (int)playerIndex + 1);
        }

        public KirbyStates getState()
        {
            return curState;
        }

        public void setState(KirbyStates state)
        {
            this.curState = state;
            if(state == KirbyStates.LinkAttack)
                setSize(dimensions.X + atkDimMod * 2, dimensions.Y + atkDimMod * 2);
            else
                setSize(dimensions.X, dimensions.Y);

            waitForAnim = false;
            int offset;
            switch (state)
            {
                case KirbyStates.Neutral:
                    curAnim = animations[0];
                    curAnim.beginAnimation(0, 0);
                    break;
                case KirbyStates.NeutralSucked:
                    curAnim = animations[3];
                    curAnim.beginAnimation(0, 0);
                    break;
                case KirbyStates.Walk:
                    curAnim = animations[0];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1);
                    break;
                case KirbyStates.Stun:
                    curAnim = animations[1];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1);
                    actionTimer = new Timer(1500);
                    break;
                case KirbyStates.Suck:
                    curAnim = animations[2];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + 4, 4);
                    curAnim.setStopFrame(offset + 4);
                    actionTimer = new Timer(2000);
                    break;
                case KirbyStates.SuckReceived:
                    curAnim = animations[2];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset + 5, offset + curAnim.getFramesPerRow() - 1, 2);
                    curAnim.setStopFrame(offset + curAnim.getFramesPerRow() - 1);
                    break;
                case KirbyStates.SuckWalk:
                    curAnim = animations[3];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1);
                    break;
                case KirbyStates.Blow:
                    curAnim = animations[4];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1, 5);
                    actionTimer = new Timer(1000);
                    break;
                case KirbyStates.NeutralLink:
                    curAnim = animations[8];
                    curAnim.beginAnimation(0, 0);
                    break;
                case KirbyStates.NeutralLinkSucked:
                    curAnim = animations[7];
                    curAnim.beginAnimation(0, 0);
                    break;
                case KirbyStates.LinkBlow:
                    curAnim = animations[5];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1, 5);
                    actionTimer = new Timer(1000);
                    break;
                case KirbyStates.LinkSuck:
                    curAnim = animations[6];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + 4, 4);
                    curAnim.setStopFrame(offset + 4);
                    actionTimer = new Timer(2000);
                    break;
                case KirbyStates.LinkSuckReceived:
                    curAnim = animations[6];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset + 5, offset + curAnim.getFramesPerRow() - 1, 2);
                    curAnim.setStopFrame(offset + curAnim.getFramesPerRow() - 1);
                    break;
                case KirbyStates.LinkSuckWalk:
                    curAnim = animations[7];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1);
                    break;
                case KirbyStates.LinkWalk:
                    curAnim = animations[8];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1);
                    break;
                case KirbyStates.LinkAttack:
                    curAnim = animations[9];
                    offset = frameOffset(curAnim, facing);
                    curAnim.beginAnimation(offset, offset + curAnim.getFramesPerRow() - 1, curAnim.getFramesPerRow());
                    waitForAnim = true;
                    break;
            }
        }

        protected override void move(GameObject.Direction dir)
        {
            setFacing(dir);
            base.move(dir);
            if (isMoving())
            {
                moveDelay = 0;
                inputEnabled = false;
                if (curState == KirbyStates.NeutralLink || curState == KirbyStates.LinkWalk)
                    setState(KirbyStates.LinkWalk);
                else if (curState == KirbyStates.Neutral || curState == KirbyStates.Walk)
                    setState(KirbyStates.Walk);
                else if (curState == KirbyStates.NeutralLinkSucked || curState == KirbyStates.LinkSuckWalk)
                    setState(KirbyStates.LinkSuckWalk);
                else if (curState == KirbyStates.NeutralSucked || curState == KirbyStates.SuckWalk)
                    setState(KirbyStates.SuckWalk);
            }
        }

        public bool canCollect(Collectible desiredObj, bool isSuck)
        {
            if (targetPickup == null)
                return true;

            // if there is already something held and they are sucking
            if (isSuck)
                return false;

            // not sucking and have something held
            Collectible.CollectibleType type = desiredObj.getCollectibleType();
            if (type == Collectible.CollectibleType.Present)
            {
                return false;
            }

            return true;
        }

        public void collect(Collectible obj, bool isSuck)
        {
            if (isSuck)
            {
                targetPickup = obj;
                Block nextBlock = levelRef.getBlockAt(obj.getPosition());
                if(nextBlock != null)
                    nextBlock.setChild(null);
                targetPickup.setTarget(new Vector2(dest.Center.X, dest.Center.Y), new Vector2(-1,-1), 2000, true);
            }
            else
            {
                if (obj.getCollectibleType() == Collectible.CollectibleType.LinkGear)
                {
                    addEffect(StatusEffect.EffectType.Link, 30000);
                }
                else if (obj.getCollectibleType() == Collectible.CollectibleType.Speed)
                {
                    addEffect(StatusEffect.EffectType.Speed, 10000);
                }
                else if (obj.getCollectibleType() == Collectible.CollectibleType.Health)
                {
                    damage(-1);
                }
                else if (obj.getCollectibleType() == Collectible.CollectibleType.Coin)
                {
                    score += 1;
                }
                else if (obj.getCollectibleType() == Collectible.CollectibleType.Star)
                {
                    score += 10;
                }
                else if (obj.getCollectibleType() == Collectible.CollectibleType.Present
                    || obj.getCollectibleType() == Collectible.CollectibleType.Shield)
                {
                    targetPickup = obj;
                    Block block = levelRef.getBlockAt(obj.getPosition());
                    if (block != null && block.getChild() == obj)
                        block.setChild(null);

                    setFacing(GameObject.Direction.Neutral);
                    if (curState == KirbyStates.NeutralLink
                        || curState == KirbyStates.NeutralLinkSucked
                        || curState == KirbyStates.LinkWalk
                        || curState == KirbyStates.LinkSuckWalk
                        || curState == KirbyStates.LinkSuckReceived)
                        setState(Player.KirbyStates.NeutralLinkSucked);
                    else if (curState == KirbyStates.Neutral
                        || curState == KirbyStates.NeutralSucked
                        || curState == KirbyStates.Walk
                        || curState == KirbyStates.SuckWalk
                        || curState == KirbyStates.SuckReceived)
                        setState(KirbyStates.NeutralSucked);
                    return;
                }

                if(targetPickup == obj)
                    targetPickup = null;

                setFacing(GameObject.Direction.Neutral);
                if (hasEffect(StatusEffect.EffectType.Link))
                {
                    if (targetPickup != null)
                        setState(KirbyStates.NeutralLinkSucked);
                    else
                        setState(KirbyStates.NeutralLink);
                }
                else
                {
                    if (targetPickup != null)
                        setState(KirbyStates.NeutralSucked);
                    else
                        setState(KirbyStates.Neutral);
                }
                Block nextBlock = levelRef.getBlockAt(obj.getPosition());
                if (nextBlock != null && nextBlock.getChild() == obj)
                    nextBlock.setChild(null);
            }
        }

        public void clearCollectible()
        {
            targetPickup = null;
        }

        public void stun()
        {
            if (hasEffect(StatusEffect.EffectType.Shield))
            {
                StatusEffect e = getEffect(StatusEffect.EffectType.Shield);
                if (!e.isExpired())
                {
                    e.resetShield(0);
                    e.setExpired(true);
                    return;
                }
            }

            moveDelay = 0;
            inputEnabled = false;
            setState(KirbyStates.Stun);

            if (hasEffect(StatusEffect.EffectType.Link))
            {
                StatusEffect e = getEffect(StatusEffect.EffectType.Link);
                e.setExpired(true);
            }
        }

        public void addEffect(StatusEffect.EffectType effectType, int time)
        {
            if (effectType == StatusEffect.EffectType.Shield)
            {
                foreach (StatusEffect e in statusEffects)
                {
                    if (e.getEffectType() == effectType)
                    {
                        e.resetShield(time);
                        return;
                    }
                }
            }
            else
            {
                foreach (StatusEffect e in statusEffects)
                {
                    if (e.getEffectType() == effectType)
                    {
                        e.updateTimer(time);
                        return;
                    }
                }
            }
            StatusEffect effect = new StatusEffect(effectType, time, this);
            statusEffects.Add(effect);
        }

        public StatusEffect getEffect(StatusEffect.EffectType effectType)
        {
            foreach (StatusEffect e in statusEffects)
            {
                if (e.getEffectType() == effectType)
                {
                    return e;
                }
            }
            return null;
        }

        public bool hasEffect(StatusEffect.EffectType effectType)
        {
            foreach (StatusEffect e in statusEffects)
            {
                if (e.getEffectType() == effectType)
                {
                    return true;
                }
            }

            return false;
        }

        private void updateThreat()
        {
            //threatRating = 0;

            double lifeScore = (lives != -0.5) ? lives : 0;
            StatusEffect linkGear = getEffect(StatusEffect.EffectType.Link);
            StatusEffect shield = getEffect(StatusEffect.EffectType.Shield);
            StatusEffect speed = getEffect(StatusEffect.EffectType.Speed);

            double linkTime = ((linkGear != null) ? linkGear.getTimeInSeconds() : 0) / 45.0; // link time is 30
            double speedTime = ((speed != null) ? speed.getTimeInSeconds() : 0) / 5.0; // speed time is 10 
            double shieldScore = ((shield != null) ? shield.getShieldCount() : 0) * 10; // shield is 2

            double weaponRating = 0;
            if (getCurItem() != null)
            {
                if (getCurItem().getCollectibleType() == Collectible.CollectibleType.Shield)
                    weaponRating = 2;
                else if (getCurItem().getCollectibleType() == Collectible.CollectibleType.Present)
                    weaponRating = 3.5;
            }

            if (linkGear != null)
                weaponRating += (0.2 + linkTime) * (1 + speedTime/3);

            double totalMultiplier = (getState() == KirbyStates.Stun) ? 0.2 : 1;

            threatRating = (weaponRating * 5 + shieldScore * 3.5 + speedTime + lifeScore) * totalMultiplier;
        }

        public double getThreat()
        {
            return threatRating;
        }

        public List<StatusEffect> getAllStatusEffects()
        {
            return statusEffects;
        }

        public Collectible getCurItem()
        {
            return targetPickup;
        }

        public void setHealth(double amount)
        {
            this.lives = amount;
        }

        public void setMaxHealth(double maxLives)
        {
            this.maxLives = maxLives;
        }

        public bool needHealth()
        {
            return (lives != -0.5 && lives < maxLives);
        }

        public void damage(double amount)
        {
            if (lives == -0.5)
                return;

            if (hasEffect(StatusEffect.EffectType.Shield) && amount > 0)
            {
                StatusEffect e = getEffect(StatusEffect.EffectType.Shield);
                amount = e.mitigateShield((float)amount);
            }

            lives -= amount;
            if (lives < 0)
                lives = 0;

            if (lives > maxLives)
                lives = maxLives;

            if (lives == 0)
                levelRef.levelOver("Player Killed");
        }

        public double getLives()
        {
            return lives;
        }

        public int getScore()
        {
            return score;
        }

        public List<AnimatedObject> getAnimations()
        {
            return animations;
        }

        private int frameOffset(AnimatedObject obj, Direction dir)
        {
            return obj.getFramesPerRow() * frameDirectionMultiplier(dir);
        }

        private int frameDirectionMultiplier(Direction dir)
        {
            switch (dir)
            {
                case Direction.Neutral:
                case Direction.Down:
                    return 0;
                case Direction.Left:
                    return 1;
                case Direction.Right:
                    return 2;
                case Direction.Up:
                    return 3;
            }
            return 0;
        }
    }
}
