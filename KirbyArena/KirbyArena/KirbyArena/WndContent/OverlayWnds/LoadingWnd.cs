using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections;
using System.Collections.Generic;

namespace KirbyArena
{
    public class LoadingWnd : WndHandle
    {
        private WndComponent background;
        private CyclingLabel loading;
        private CyclingLabel hintText;

        public LoadingWnd(Rectangle displayRect, KirbyGame appRef)
            : base(WndType.Overlay, displayRect, appRef)
        {
            Rectangle loadingRect = new Rectangle(20, displayRect.Bottom - 150, 0, 0);
            List<string> loadingText = new List<string>();
            loadingText.Add("Loading");
            loadingText.Add("Loading .");
            loadingText.Add("Loading ..");
            loadingText.Add("Loading ...");
            loading = new CyclingLabel(loadingRect, loadingText, loadFont("hugeFont"), Color.White, false);
            loading.setTimer(new Timer(300));

            Rectangle hintRect = new Rectangle(30, displayRect.Bottom - 100, 0, 0);
            List<string> hintTextStrings = new List<string>();
            hintTextStrings.Add("Kirby is cute! :D");
            hintTextStrings.Add("There is so much more that could be added to this game.");
            hintTextStrings.Add("This loading time is almost completely faked.");
            hintTextStrings.Add("Whoever plays this game is awesome! ;)");
            hintTextStrings.Add("Kirby for president!");
            hintTextStrings.Add("To attack: hold down the attack button and a direction button.");
            hintTextStrings.Add("You can suck in items from 2 squares away.");
            hintTextStrings.Add("You can blow presents out to gain another item or stun the other player.");
            hintTextStrings.Add("Look in options to change the key bindings, if desired.");
            hintTextStrings.Add("The person this was made for is awesome! :P");

            hintText = new CyclingLabel(hintRect, hintTextStrings, loadFont("largeFont"), Color.White, true);
            hintText.setTimer(new Timer(2600));

            background = new WndComponent(displayRect);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (isVisible)
            {
                loading.update(gameTime);
                hintText.update(gameTime);
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            if(isVisible)
            {
                background.draw(spriteBatch);
                loading.draw(spriteBatch);
                hintText.draw(spriteBatch);
            }
        }
    }
}
