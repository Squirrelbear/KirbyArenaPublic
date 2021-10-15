using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KirbyArena
{
    public class PauseWnd : WndHandle
    {
        private ButtonCollection menu;
        private Level level;

        public PauseWnd(Rectangle displayRect, KirbyGame appRef, Level level)
            : base(WndType.Overlay, displayRect, appRef)
        {
            this.level = level;
            WndComponent background = new WndComponent(displayRect, loadTexture("WndContent\\HUD\\DialogBackground"));

            Rectangle menuRect = new Rectangle((int)(displayRect.X + displayRect.Width * 0.1f), displayRect.Top + (int)(displayRect.Height * 0.3f),
                                            (int)(displayRect.Width * 0.8f), (int)(displayRect.Height * 0.6f));
            menu = new ButtonCollection(menuRect);

            LayoutManger layout = new LayoutManger(menuRect, 3, 1);
            Texture2D btnOutTexture = loadTexture("WndContent\\Components\\btnOut");
            Texture2D btnOverTexture = loadTexture("WndContent\\Components\\btnOver");
            SpriteFont font = loadFont("largeFont");

            TextButton resumeBtn = new TextButton(layout.nextRect(), "Resume", font, btnOverTexture, btnOutTexture, true, 0);
            resumeBtn.setFontColor(Color.Maroon);
            resumeBtn.setSelectedFontColor(Color.Black);
            TextButton restartBtn = new TextButton(layout.nextRect(), "Restart", font, btnOverTexture, btnOutTexture, false, 2);
            restartBtn.setFontColor(Color.Maroon);
            restartBtn.setSelectedFontColor(Color.Black);
            TextButton mainmenuBtn = new TextButton(layout.nextRect(), "Main Menu", font, btnOverTexture, btnOutTexture, false, 1);
            mainmenuBtn.setFontColor(Color.Maroon);
            mainmenuBtn.setSelectedFontColor(Color.Black);

            menu.add(resumeBtn);
            menu.add(restartBtn);
            menu.add(mainmenuBtn);

            Rectangle titleRect = new Rectangle(displayRect.Left, displayRect.Top, displayRect.Width, (int)(displayRect.Height * 0.3));
            Label titleLabel = new Label(titleRect, "Paused", font);
            titleLabel.setColor(Color.Maroon);
            titleLabel.centreInRect(titleRect);

            addComponent(background);
            addComponent(menu);
            addComponent(titleLabel);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isKeyPressed(Keys.Escape))
            {
                level.setOverlayWnd(null);
            }
            else if (inputManager.isKeyPressed(Keys.Enter) || inputManager.isBtnPressed(Buttons.A, 1)
                    || menu.isBtnClicked())
            {
                menu.playSelectedSound();
                if(menu.getSelected().getActionID() == 0)
                {
                    level.setOverlayWnd(null);
                }
                else if (menu.getSelected().getActionID() == 2)
                {
                    appRef.setWnd(WndType.LevelViewer);
                }
                else
                {
                    appRef.setWnd(WndType.MainMenu);
                }
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
