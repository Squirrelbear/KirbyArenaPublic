using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KirbyArena
{
    public class StatusEffect
    {
        public enum EffectType { Link, Speed, Shield };

        protected EffectType effectType;
        protected Timer duration;
        protected Player target;
        protected bool expired;
        protected List<float> originalTimes;

        public StatusEffect(EffectType effectType, int time, Player target)
        {
            this.effectType = effectType;
            this.duration = new Timer(time);
            this.target = target;
            expired = false;
            onBegin();
        }

        public virtual void update(GameTime gameTime)
        {
            if (effectType == EffectType.Shield)
                return;

            duration.update(gameTime);
            if (duration.wasTriggered())
            {
                expired = true;
            }
        }

        public void updateTimer(int time)
        {
            duration = new Timer(time);
        }

        public virtual void onBegin()
        {
            if (effectType == EffectType.Link)
            {
                target.setFacing(GameObject.Direction.Neutral);
                target.setState(Player.KirbyStates.NeutralLink);
                /*if(target.getState() == Player.KirbyStates.LinkWalk
                    || target.getState() == Player.KirbyStates.Walk
                    || target.getState() == Player.KirbyStates.SuckReceived)
                    target.setState(Player.KirbyStates.NeutralLink);
                else if(target.getState() == Player.KirbyStates.LinkSuckWalk
                    || target.getState() == Player.KirbyStates.SuckWalk
                    || target.getState() == Player.KirbyStates.LinkSuckReceived)
                    target.setState(Player.KirbyStates.NeutralLink);*/
            }
            else if (effectType == EffectType.Speed)
            {
                target.setSpeedMulti((float)3);
                originalTimes = new List<float>();
                foreach (AnimatedObject a in target.getAnimations())
                {
                    originalTimes.Add(a.getFrameTime());
                    a.setFrameTime((int)(a.getFrameTime() / 3));
                }
            }
            else if (effectType == EffectType.Shield)
            {
                originalTimes = new List<float>();
                for (int i = 0; i < (int)duration.getInterval(); i++)
                    originalTimes.Add(0.5f);
            }
        }

        public virtual void onEnd()
        {
            if (effectType == EffectType.Link)
            {
                if (target.getState() == Player.KirbyStates.NeutralLink)
                    target.setState(Player.KirbyStates.Neutral);
                else if (target.getState() == Player.KirbyStates.LinkWalk)
                    target.setState(Player.KirbyStates.Walk);
                else if (target.getState() == Player.KirbyStates.NeutralLinkSucked)
                    target.setState(Player.KirbyStates.NeutralSucked);
                else if (target.getState() == Player.KirbyStates.LinkSuckWalk)
                    target.setState(Player.KirbyStates.SuckWalk);

            }
            else if (effectType == EffectType.Speed)
            {
                target.setSpeedMulti(1);
                List<AnimatedObject> anims = target.getAnimations();
                for(int i = 0; i < anims.Count; i++)
                {
                    anims[i].setFrameTime((int)originalTimes[i]);
                }
            }
        }

        public float mitigateShield(float damage)
        {
            if (effectType != EffectType.Shield || damage <= 0)
                return damage;

            int damageCount = (int)(damage / 0.5f);
            if (damageCount < originalTimes.Count)
            {
                originalTimes.RemoveRange(0, damageCount);
                return 0;
            }
            else
            {
                damageCount -= originalTimes.Count;
                setExpired(true);
                originalTimes.Clear();
                return damageCount * 0.5f;
            }
        }

        public EffectType getEffectType()
        {
            return effectType;
        }

        public void resetShield(int newTime)
        {
            duration.setInterval(newTime);
            originalTimes.Clear();
            for (int i = 0; i < (int)duration.getInterval(); i++)
                originalTimes.Add(0.5f);
        }

        public int getShieldCount()
        {
            return originalTimes.Count;
        }

        public int getTimeInSeconds()
        {
            return (int)duration.getTimeInSeconds();
        }

        public void setExpired(bool expired)
        {
            this.expired = expired;
        }

        public bool isExpired()
        {
            return expired;
        }
    }
}
