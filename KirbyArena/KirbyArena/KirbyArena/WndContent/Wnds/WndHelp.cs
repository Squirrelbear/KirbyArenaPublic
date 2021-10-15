using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KirbyArena
{
    public class WndHelp : WndHandle
    {
        private WndComponent background;
        private WndComponent helpImage;
        private ToggleOption menuList;
        private List<Texture2D> helpImages;

        public WndHelp(Rectangle displayRect, KirbyGame appRef)
            : base(WndType.Help, displayRect, appRef)
        {
            Texture2D arrowLeftTexture = loadTexture("WndContent\\Components\\leftarrow");
            Texture2D arrowRightTexture = loadTexture("WndContent\\Components\\rightarrow");
            SpriteFont font = loadFont("hugeFont");
            SpriteFont fontSmall = loadFont("mediumFont");

            background = new WndComponent(displayRect, loadTexture("WndContent\\MainMenu\\othermenubg"));
            helpImages = new List<Texture2D>();
            helpImages.Add(loadTexture("WndContent\\MainMenu\\HelpImages_howplay"));
            helpImages.Add(loadTexture("WndContent\\MainMenu\\HelpImages_keyboard"));
            helpImages.Add(loadTexture("WndContent\\MainMenu\\HelpImages_gamepad"));

            Vector2 backMessageDim = fontSmall.MeasureString("Press Escape or Back on controller to return to Main Menu.");
            Rectangle backMessageDest = new Rectangle(displayRect.X, displayRect.Bottom - (int)backMessageDim.Y - 10, displayRect.Width, (int)backMessageDim.Y);
            Label backMessage = new Label(backMessageDest, "Press Escape or Back on controller to return to Main Menu.", fontSmall);
            backMessage.centreInRect();
            //backMessage.setColor(Color.Maroon);

            List<string> menuItems = new List<string>();
            menuItems.Add("How to Play");
            menuItems.Add("Keyboard Controls");
            menuItems.Add("Controller Controls");

            Rectangle toggleMenuDest = new Rectangle(displayRect.Width/ 2 - 500 / 2, displayRect.Top + 20, 500, 75);
            menuList = new ToggleOption(toggleMenuDest, menuItems, arrowLeftTexture, arrowRightTexture, font);
            menuList.setLoop(true);

            Rectangle helpImageDest = new Rectangle((int)(displayRect.Center.X - displayRect.Height * 0.4), (int)(displayRect.Center.Y - displayRect.Height * 0.35),
                                                    (int)(displayRect.Height * 0.8), (int)(displayRect.Height * 0.8));

            helpImage = new WndComponent(helpImageDest, helpImages[0]);

            addComponent(background);
            addComponent(menuList);
            addComponent(helpImage);
            addComponent(backMessage);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isBtnPressed(Buttons.B, 1)  || inputManager.isKeyPressed(Keys.Escape))
            {
                appRef.setWnd(WndType.MainMenu);
            }
            else if (inputManager.isKeyPressed(Keys.Back) || inputManager.isBtnPressed(Buttons.B, 1))
            {
                appRef.setWnd(WndType.MainMenu);
            }
            else if (inputManager.isKeyPressed(Keys.Right) || inputManager.isBtnPressed(Buttons.DPadRight, 1)
                || (menuList.getIsChanged() && !menuList.isShiftLeft()))
            {
                // if not already changed (indicating a click event)
                if (!menuList.getIsChanged())
                    menuList.next();
                helpImage.setTexture(helpImages[menuList.getSelectedID()]);
            }
            else if (inputManager.isKeyPressed(Keys.Left) || inputManager.isBtnPressed(Buttons.DPadLeft, 1)
                || (menuList.getIsChanged() && menuList.isShiftLeft()))
            {
                // if not already changed (indicating a click event)
                if(!menuList.getIsChanged())
                    menuList.previous();
                helpImage.setTexture(helpImages[menuList.getSelectedID()]);
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
        }
    }
}
