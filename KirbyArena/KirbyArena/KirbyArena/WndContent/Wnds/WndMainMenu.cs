using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace KirbyArena
{
    public class WndMainMenu : WndHandle
    {
        private ButtonCollection menu;
        private WndComponent background;
        private Label verLabel;
        private SnowflakeSpawner snowflakeSpawner;

        public WndMainMenu(Rectangle displayRect, KirbyGame appRef)
            : base(WndType.MainMenu, displayRect, appRef)
        {
            background = new WndComponent(displayRect, loadTexture("WndContent\\MainMenu\\mainmenubg"));

            Rectangle menuRect = new Rectangle((int)(displayRect.Width * 0.1f), displayRect.Top + (int)(displayRect.Height * 0.3f),
                                            (int)(displayRect.Width * 0.25f), (int)(displayRect.Height * 0.5f));
            menu = new ButtonCollection(menuRect);

            LayoutManger layout = new LayoutManger(menuRect, 5, 1);
            Texture2D btnOutTexture = loadTexture("WndContent\\Components\\btnOut");
            Texture2D btnOverTexture = loadTexture("WndContent\\Components\\btnOver");
            SpriteFont font = loadFont("hugeFont");

            TextButton playBtn = new TextButton(layout.nextRect(), "Play", font, btnOverTexture, btnOutTexture, true, (int)WndType.PlayConfig);
            TextButton helpBtn = new TextButton(layout.nextRect(), "Help", font, btnOverTexture, btnOutTexture, false, (int)WndType.Help);
            TextButton optionsBtn = new TextButton(layout.nextRect(), "Options", font, btnOverTexture, btnOutTexture, false, (int)WndType.Options);
            TextButton creditsBtn = new TextButton(layout.nextRect(), "Credits", font, btnOverTexture, btnOutTexture, false, (int)WndType.Credits);
            TextButton quitBtn = new TextButton(layout.nextRect(), "Quit", font, btnOverTexture, btnOutTexture, false, (int)WndType.ExitGame);

            menu.add(playBtn);
            menu.add(helpBtn);
            menu.add(optionsBtn);
            menu.add(creditsBtn);
            menu.add(quitBtn);

            string verString = "Copyright 2012 Peter Creations. Ver: " + appRef.getAppVersion();
            SpriteFont smallFont = loadFont("smallFont");
            Vector2 verStrDim = smallFont.MeasureString(verString);
            verLabel = new Label(new Rectangle(displayRect.Right - (int)verStrDim.X - 10, displayRect.Bottom - (int)verStrDim.Y - 5, 0, 0), verString, smallFont);

            snowflakeSpawner = new SnowflakeSpawner(loadTexture("WndContent/MainMenu/snowflake"), displayRect);

            addComponent(background);
            addComponent(menu);
            addComponent(verLabel);
            addComponent(snowflakeSpawner);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isKeyPressed(Keys.Escape))
            {
                appRef.Exit();
            }
            else if (inputManager.isKeyPressed(Keys.Enter) || inputManager.isBtnPressed(Buttons.A, 1) 
                || menu.isBtnClicked())
            {
                menu.playSelectedSound();
                appRef.setWnd((WndType)menu.getSelected().getActionID());
            }
            else if (inputManager.isKeyPressed(Keys.Down) || inputManager.isBtnPressed(Buttons.DPadDown, 1))
            {
                menu.next();
            }
            else if (inputManager.isKeyPressed(Keys.Up) || inputManager.isBtnPressed(Buttons.DPadUp, 1))
            {
                menu.previous();
            }
        }
    }
}
