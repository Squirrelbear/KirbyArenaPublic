using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class WndCredits : WndHandle
    {
        public WndCredits(Rectangle displayRect, KirbyGame appRef)
            : base(WndType.Credits, displayRect, appRef)
        {
            WndComponent background = new WndComponent(displayRect, loadTexture("WndContent\\MainMenu\\othermenubg"));

            Rectangle divArea = new Rectangle(displayRect.X + (int)(displayRect.Height * 0.05), displayRect.Y, displayRect.Width, (int)(displayRect.Height * 0.9));
            LayoutManger layout = new LayoutManger(divArea, 4, 1);
            SpriteFont font = loadFont("hugeFont");
            SpriteFont fontSmall = loadFont("mediumFont");

            Label title = new Label(layout.nextRect(), "Credits", font);
            title.centreInRect();
            //title.setColor(Color.Maroon);

            Label msg = new Label(layout.nextRect(), "This game was developed for Christina Georgas.", font);
            //msg.setColor(Color.Maroon);
            Label peter = new Label(layout.nextRect(), "Lead Designer/Developer/Programmer/Artist/Procrastinator: Peter Mitchell", font);
           // peter.setColor(Color.Maroon);
            Label andy = new Label(layout.nextRect(), "Animated Kirby: Andrew Krix", font);
            //andy.setColor(Color.Maroon);

            Vector2 backMessageDim = fontSmall.MeasureString("Press Escape or Back on controller to return to Main Menu.");
            Rectangle backMessageDest = new Rectangle(displayRect.X, displayRect.Bottom - (int)backMessageDim.Y - 10, displayRect.Width, (int)backMessageDim.Y);
            Label backMessage = new Label(backMessageDest, "Press Escape or Back on controller to return to Main Menu.", fontSmall);
            backMessage.centreInRect();
            //backMessage.setColor(Color.Maroon);

            addComponent(background);
            addComponent(title);
            addComponent(msg);
            addComponent(peter);
            addComponent(andy);
            addComponent(backMessage);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isBtnPressed(Buttons.B, 1) || inputManager.isKeyPressed(Keys.Escape))
            {
                appRef.setWnd(WndType.MainMenu);
            }
        }
    }
}
