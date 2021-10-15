using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace KirbyArena
{
    public class WndLevelViewer : WndHandle
    {
        private Level level;
        private WndHandle overlayWnd;
        private Thread loadingThread;
        private bool hasLoaded;
        private Label lbl;

        public WndLevelViewer(Rectangle displayRect, KirbyGame appRef)
            : base(WndType.LevelViewer, displayRect, appRef)
        {
            hasLoaded = false;
            overlayWnd = new LoadingWnd(displayRect, appRef);
            loadingThread = new Thread(new ThreadStart(loadLevel));
            loadingThread.Start();

            lbl = new Label(new Rectangle(100, displayRect.Y+200, 0, 0), "Level loaded", loadFont("hugeFont"));
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (!hasLoaded)
            {
                overlayWnd.update(gameTime);
                return;
            }

            level.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            if(overlayWnd != null)
                overlayWnd.draw(spriteBatch);

            if (!hasLoaded)
                return;
            level.draw(spriteBatch);

            //if (overlayWnd != null)
            //    lbl.draw(spriteBatch);
        }

        private void loadLevel()
        {
#if !DEBUG
            Thread.Sleep(5000);
#endif
            switch(appRef.getGameOptions().levelID)
            {
                case GameOptions.LevelID.SnowLevel:
                    level = new SnowLevel(displayRect, appRef.getGameOptions(), appRef);
                    break;
                case GameOptions.LevelID.IslandLevel:
                    level = new IslandLevel(displayRect, appRef.getGameOptions(), appRef);
                    break;
                case GameOptions.LevelID.DesertLevel:
                    level = new DesertLevel(displayRect, appRef.getGameOptions(), appRef);
                    break;
                case GameOptions.LevelID.NinjaLevel:
                    level = new NinjaLevel(displayRect, appRef.getGameOptions(), appRef);
                    break;
                case GameOptions.LevelID.PacManLevel:
                    level = new PacManLevel(displayRect, appRef.getGameOptions(), appRef);
                    break;
            }
            hasLoaded = true;
            overlayWnd = null;
        }
    }
}
