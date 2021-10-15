using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class KeyBinder : WndComponent
    {
        protected Label label;
        protected Keys binding;
        protected InputManager inputManager;
        protected int player, keyID;
        protected bool listening;
        protected GameOptions options;

        public KeyBinder(Rectangle dest, int player, int keyID, SpriteFont font, InputManager inputManager, GameOptions options)
            : base(dest)
        {
            this.player = player;
            this.keyID = keyID;
            this.inputManager = inputManager;
            this.options = options;

            binding = options.getKey(player, keyID);
            listening = false;

            label = new Label(dest, binding.ToString(), font);
            label.centreInRect();
            label.setColor(Color.Black);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);


            if (listening && inputManager.isKeyPressed(Keys.Escape))
            {
                label.setText(binding.ToString());
                label.centreInRect();
                label.setColor(Color.Black);
                listening = false;
                hasFocus = false;
            }
            else if (listening)
            {
                Keys[] keys = inputManager.getKeysDown();
                if (keys.Length > 0 && keys[0] != Keys.Escape)
                {
                    binding = keys[0];
                    options.setKey(player, keyID, binding);
                    label.setText(binding.ToString());
                    label.centreInRect();
                    label.setColor(Color.Black);
                    listening = false;
                    hasFocus = false;
                }
            }

            label.update(gameTime);
        }

        public override void mouseClicked(Point p)
        {
            base.mouseClicked(p);

            if (getFocusRect().Contains(p))
            {
                label.setColor(Color.Red);
                label.setText("Press a Key");
                label.centreInRect();
                listening = true;
                hasFocus = true;
            }
            else
            {
                label.setText(binding.ToString());
                label.centreInRect();
                label.setColor(Color.Black);
                listening = false;
                hasFocus = false;
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);

            label.draw(spriteBatch);
        }

        public bool isListening()
        {
            return listening;
        }
    }
}
