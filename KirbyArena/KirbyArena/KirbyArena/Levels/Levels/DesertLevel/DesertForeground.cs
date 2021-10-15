using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class DesertForeground : LevelBackground
    {
        protected AnimatedObject nyan;
        protected AnimatedObject nyanTrail;
        protected AnimatedObject nyanTrailEnd;
        protected bool moving;
        protected Timer cooldown;
        protected Level levelRef;
        protected float speed;
        protected int timeRange;
        protected bool p1Hit, p2Hit;
        protected Timer animationTimer;
        protected float incrementTrail, incrementEnd, maxHeight, minHeight;

        public DesertForeground(Texture2D backgroundSprite, Rectangle displayRect, Level levelRef)
            : base(backgroundSprite, displayRect)
        {
            this.levelRef = levelRef;
            Texture2D nyanSprite = levelRef.loadTexture("Sprites\\ExtraContent\\mexicannyanlarge");
            Texture2D nyanTrailSprite = levelRef.loadTexture("Sprites\\ExtraContent\\nyantrail");
            Texture2D nyanTrailEndSprite = levelRef.loadTexture("Sprites\\ExtraContent\\nyantrailend"); 

            int startrow = levelRef.getSharedRandom().Next(10);
            Rectangle rowData = levelRef.getRect(0, startrow);
            bool startLeft = (levelRef.getSharedRandom().Next(2) == 0);
            int startX = displayRect.X - rowData.Width*2 - 50;
            if(!startLeft)
            {
                startX = displayRect.Right + 50;
            }

            nyan = new AnimatedObject(nyanSprite, 512, 256, new Rectangle(startX, rowData.Y, rowData.Width * 2, rowData.Height));
            nyanTrail = new AnimatedObject(nyanTrailSprite, 256, 256, 
                new Rectangle(startX - rowData.Width, (int)(nyan.getRect().Y - 0.1 * rowData.Height), rowData.Width, rowData.Height));
            nyanTrailEnd = new AnimatedObject(nyanTrailEndSprite, 256, 256,
                new Rectangle(startX - rowData.Width * 2, (int)(nyan.getRect().Y + 0.1 * rowData.Height), rowData.Width, rowData.Height));
            cooldown = new Timer(levelRef.getSharedRandom().Next(10000) + 5000);

            maxHeight = (int)(nyan.getRect().Y + 0.1 * rowData.Height);
            minHeight = (int)(nyan.getRect().Y - 0.1 * rowData.Height);
            incrementTrail = incrementEnd = 0.05f * rowData.Height;
            animationTimer = new Timer(75);

            moving = false;
            switch (levelRef.getAppRef().getGameOptions().difficulty)
            {
                case GameOptions.Difficulty.Easy:
                    speed = rowData.Height * 6;
                    timeRange = 8000;
                    break;
                case GameOptions.Difficulty.Medium:
                    speed = rowData.Height * 7;
                    timeRange = 4000;
                    animationTimer.setInterval(75 * 7 / 6);
                    break;
                case GameOptions.Difficulty.Hard:
                    speed = rowData.Height * 8;
                    timeRange = 2000;
                    animationTimer.setInterval(75 * 8 / 6);
                    break;

            }
            
            if (!startLeft)
            {
                nyan.setSpriteEffect(SpriteEffects.FlipHorizontally);
                speed = -speed;
                nyanTrail.setLocation(startX + rowData.Width * 2, nyanTrail.getRect().Y);
                nyanTrailEnd.setLocation(startX + rowData.Width * 3, nyanTrailEnd.getRect().Y);
                nyanTrailEnd.setSpriteEffect(SpriteEffects.FlipHorizontally);
            }

            p1Hit = false;
            p2Hit = false;
        }

        public override void drawForeground(SpriteBatch spriteBatch)
        {
            base.drawForeground(spriteBatch);

            nyanTrail.draw(spriteBatch);
            nyanTrailEnd.draw(spriteBatch);
            nyan.draw(spriteBatch);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (moving)
            {
                animationTimer.update(gameTime);

                nyan.moveBy(speed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, 0);

                bool swap = false;
                if (speed < 0)
                {
                    if (nyan.getRect().X < -nyan.getRect().Width - 5 * nyanTrail.getRect().Width)
                    {
                        swap = true;
                    }

                    nyanTrail.setLocation(nyan.getRect().Right - nyanTrail.getRect().Width / 2, nyanTrail.getRect().Y);
                    nyanTrailEnd.setLocation(nyanTrail.getRect().X + nyanTrailEnd.getRect().Width, nyanTrailEnd.getRect().Y);
                }
                else
                {
                    if (nyan.getRect().X > backgroundRect.Right + 5 * nyanTrail.getRect().Width)
                    {
                        swap = true;
                    }

                    nyanTrail.setLocation(nyan.getRect().Left - nyanTrail.getRect().Width / 2, nyanTrail.getRect().Y);
                    nyanTrailEnd.setLocation(nyanTrail.getRect().X - nyanTrailEnd.getRect().Width, nyanTrailEnd.getRect().Y);
                }

                if (swap)
                {
                    moving = false;
                    speed = -speed;
                    int row = levelRef.getSharedRandom().Next(10);
                    Rectangle rowData = levelRef.getRect(0, row);
                    int startX = backgroundRect.X - rowData.Width*2 - 50;                    
                    if (speed < 0)
                    {
                        startX = backgroundRect.Right + 50;
                        nyan.setLocation(startX, rowData.Y);
                        nyan.setSpriteEffect(SpriteEffects.FlipHorizontally);
                        nyanTrailEnd.setSpriteEffect(SpriteEffects.FlipHorizontally);
                        nyanTrail.setLocation(nyan.getRect().X + nyanTrail.getRect().Width * 2, (int)(nyan.getRect().Y - 0.1 * rowData.Height));
                        nyanTrailEnd.setLocation(nyan.getRect().X + nyanTrailEnd.getRect().Width * 3, (int)(nyan.getRect().Y + 0.1 * rowData.Height));
                    }
                    else
                    {
                        nyan.setLocation(startX, rowData.Y);
                        nyan.setSpriteEffect(SpriteEffects.None);
                        nyanTrailEnd.setSpriteEffect(SpriteEffects.None);
                        nyanTrail.setLocation(nyan.getRect().X - nyanTrail.getRect().Width, (int)(nyan.getRect().Y - 0.1 * rowData.Height));
                        nyanTrailEnd.setLocation(nyan.getRect().X - nyanTrailEnd.getRect().Width * 2, (int)(nyan.getRect().Y + 0.1 * rowData.Height));
                    }

                    maxHeight = (int)(nyan.getRect().Y + 0.1 * rowData.Height);
                    minHeight = (int)(nyan.getRect().Y - 0.1 * rowData.Height);
                    p1Hit = false;
                    p2Hit = false;

                    incrementTrail = incrementEnd = 0.05f * rowData.Height;
                }
                else
                {
                    Player p1 = levelRef.getPlayer(1);
                    Player p2 = levelRef.getPlayer(2);

                    if (!p1Hit && p1 != null && nyan.getRect().Intersects(p1.getRect()))
                    {
                        p1.damage(0.5);
                        p1.stun();
                        p1Hit = true;
                    }
                    else if (!p2Hit && p2 != null && nyan.getRect().Intersects(p2.getRect()))
                    {
                        p2.damage(0.5);
                        p2.stun();
                        p2Hit = true;
                    }

                    if (animationTimer.wasTriggered())
                    {
                        if((incrementTrail > 0 && nyanTrail.getRect().Y + incrementTrail > maxHeight)
                            || (incrementTrail < 0 && nyanTrail.getRect().Y - incrementTrail < minHeight))
                            incrementTrail = -incrementTrail;
                        int newTrailY = (int)(nyanTrail.getRect().Y + incrementTrail);
                        nyanTrail.setLocation(nyanTrail.getRect().X, newTrailY);

                        if ((incrementEnd > 0 && nyanTrailEnd.getRect().Y + incrementEnd > maxHeight)
                            || (incrementEnd < 0 && nyanTrailEnd.getRect().Y - incrementEnd < minHeight))
                            incrementEnd = -incrementEnd;
                        int newTrailEndY = (int)(nyanTrailEnd.getRect().Y + incrementEnd);
                        nyanTrailEnd.setLocation(nyanTrailEnd.getRect().X, newTrailEndY);
                    }
                }
            }
            else
            {
                cooldown.update(gameTime);
                if (cooldown.wasTriggered())
                {
                    cooldown.setInterval(levelRef.getSharedRandom().Next(8000) + 2000);
                    moving = true;  
                }
            }
        }
    }
}
