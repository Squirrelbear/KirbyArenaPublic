using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class WndOptions : WndHandle
    {
        protected ToggleOption volumeToggle;
        protected ToggleOption muteToggle;

        public WndOptions(Rectangle displayRect, KirbyGame appRef)
            : base(WndType.Options, displayRect, appRef)
        {
            WndComponent background = new WndComponent(displayRect, loadTexture("WndContent\\MainMenu\\othermenubg"));
            addComponent(background);

            Texture2D btnOutTexture = loadTexture("WndContent\\Components\\btnOut");
            Texture2D btnOverTexture = loadTexture("WndContent\\Components\\btnOver");
            Texture2D leftArrowTexture = loadTexture("WndContent\\Components\\leftarrow");
            Texture2D rightArrowTexture = loadTexture("WndContent\\Components\\rightarrow");
            SpriteFont font = loadFont("hugeFont");
            SpriteFont fontSmall = loadFont("mediumFont");

            Rectangle titleRect = new Rectangle(displayRect.Left, displayRect.Top, displayRect.Width, (int)(displayRect.Height * 0.1));
            Label title = new Label(titleRect, "Options", font);
            title.centreInRect();
            //title.setColor(Color.Maroon);

            /*Label msg = new Label(displayRect, "There are not currently any options available.", font);
            msg.centreInRect();
            msg.setColor(Color.Maroon);*/

            int widthColLeft = (int)(displayRect.Width * 0.3);
            int widthColRight = (int)(displayRect.Width * 0.7);
            int basicHeight = (int)(displayRect.Height * 0.1);

            Rectangle volumeToggleRect = new Rectangle(displayRect.Left + widthColLeft,
                                                     displayRect.Top + basicHeight,
                                                     widthColRight, basicHeight);
            List<string> volumeOptions = new List<string>();
            for (int i = 0; i <= 100; i += 5)
                volumeOptions.Add("" + i);
            volumeToggle = new ToggleOption(volumeToggleRect, volumeOptions, leftArrowTexture, rightArrowTexture, font);
            volumeToggle.setSelection((int)(getAppRef().getAudioManager().getVolume() / 5));

            Rectangle volumeLabelRect = new Rectangle(displayRect.Left + 5, displayRect.Top + basicHeight,
                                                     (int)font.MeasureString("Volume:").X, basicHeight);
            Label volumeLabel = new Label(volumeLabelRect, "Volume:", font);
            volumeLabel.centreInRect(volumeLabelRect);

            Rectangle muteToggleRect = new Rectangle(displayRect.Left + widthColLeft,
                                                     displayRect.Top + basicHeight*2,
                                                     widthColRight, basicHeight);
            List<string> muteOptions = new List<string>();
            muteOptions.Add("Not Muted");
            muteOptions.Add("Muted");
            muteToggle = new ToggleOption(muteToggleRect, muteOptions, leftArrowTexture, rightArrowTexture, font);
            muteToggle.setSelection(appRef.getAudioManager().getMuted() ? 1 : 0);

            Rectangle muteLabelRect = new Rectangle(displayRect.Left + 5, displayRect.Top + basicHeight * 2,
                                                     (int)font.MeasureString("Mute:").X, basicHeight);
            Label muteLabel = new Label(volumeLabelRect, "Mute:", font);
            muteLabel.centreInRect(muteLabelRect);

            LayoutManger lm = new LayoutManger(new Rectangle(20, displayRect.Y + (int)(displayRect.Height * 0.35), 
                                                displayRect.Width - 40, (int)(displayRect.Height * 0.5)), 8, 3);

            Label kbTitle = new Label(lm.nextRect(), "Key Bindings", font);
            kbTitle.centreInRect(Label.CentreMode.CentreVertical);
            Label bindPlayerOne = new Label(lm.nextRect(), "Player One", font);
            bindPlayerOne.centreInRect(Label.CentreMode.CentreBoth);
            Label bindPlayerTwo = new Label(lm.nextRect(), "Player Two", font);
            bindPlayerTwo.centreInRect(Label.CentreMode.CentreBoth);

            Label bindTitleMoveUp = new Label(lm.nextRect(), "Move Up", font);
            bindTitleMoveUp.centreInRect(Label.CentreMode.CentreVertical);
            KeyBinder bindMoveUpOne = new KeyBinder(lm.nextRect(), 1, 0, font, inputManager, getAppRef().getGameOptions());
            KeyBinder bindMoveUpTwo = new KeyBinder(lm.nextRect(), 2, 0, font, inputManager, getAppRef().getGameOptions());

            Label bindTitleMoveDown = new Label(lm.nextRect(), "Move Down", font);
            bindTitleMoveDown.centreInRect(Label.CentreMode.CentreVertical);
            KeyBinder bindMoveDownOne = new KeyBinder(lm.nextRect(), 1, 1, font, inputManager, getAppRef().getGameOptions());
            KeyBinder bindMoveDownTwo = new KeyBinder(lm.nextRect(), 2, 1, font, inputManager, getAppRef().getGameOptions());

            Label bindTitleMoveLeft = new Label(lm.nextRect(), "Move Left", font);
            bindTitleMoveLeft.centreInRect(Label.CentreMode.CentreVertical);
            KeyBinder bindMoveLeftOne = new KeyBinder(lm.nextRect(), 1, 2, font, inputManager, getAppRef().getGameOptions());
            KeyBinder bindMoveLeftTwo = new KeyBinder(lm.nextRect(), 2, 2, font, inputManager, getAppRef().getGameOptions());

            Label bindTitleMoveRight = new Label(lm.nextRect(), "Move Right", font);
            bindTitleMoveRight.centreInRect(Label.CentreMode.CentreVertical);
            KeyBinder bindMoveRightOne = new KeyBinder(lm.nextRect(), 1, 3, font, inputManager, getAppRef().getGameOptions());
            KeyBinder bindMoveRightTwo = new KeyBinder(lm.nextRect(), 2, 3, font, inputManager, getAppRef().getGameOptions());

            Label bindTitleAtk = new Label(lm.nextRect(), "Attack", font);
            bindTitleAtk.centreInRect(Label.CentreMode.CentreVertical);
            KeyBinder bindAtkOne = new KeyBinder(lm.nextRect(), 1, 4, font, inputManager, getAppRef().getGameOptions());
            KeyBinder bindAtkTwo = new KeyBinder(lm.nextRect(), 2, 4, font, inputManager, getAppRef().getGameOptions());

            Label bindTitleSuck = new Label(lm.nextRect(), "Suck/Blow", font);
            bindTitleSuck.centreInRect(Label.CentreMode.CentreVertical);
            KeyBinder bindSuckOne = new KeyBinder(lm.nextRect(), 1, 5, font, inputManager, getAppRef().getGameOptions());
            KeyBinder bindSuckTwo = new KeyBinder(lm.nextRect(), 2, 5, font, inputManager, getAppRef().getGameOptions());

            Label bindTitleUse = new Label(lm.nextRect(), "Use Action", font);
            bindTitleSuck.centreInRect(Label.CentreMode.CentreVertical);
            KeyBinder bindUseActionOne = new KeyBinder(lm.nextRect(), 1, 6, font, inputManager, getAppRef().getGameOptions());
            KeyBinder bindUseActionTwo = new KeyBinder(lm.nextRect(), 2, 6, font, inputManager, getAppRef().getGameOptions());

            Vector2 backMessageDim = fontSmall.MeasureString("Press Escape or Back on controller to return to Main Menu.");
            Rectangle backMessageDest = new Rectangle(displayRect.X, displayRect.Bottom - (int)backMessageDim.Y - 10, displayRect.Width, (int)backMessageDim.Y);
            Label backMessage = new Label(backMessageDest, "Press Escape or Back on controller to return to Main Menu.", fontSmall);
            backMessage.centreInRect();
            //backMessage.setColor(Color.Maroon);

            addComponent(background);
            addComponent(title);

            addComponent(volumeLabel);
            addComponent(volumeToggle);
            addComponent(muteLabel);
            addComponent(muteToggle);

            addComponent(kbTitle);
            addComponent(bindPlayerOne);
            addComponent(bindPlayerTwo);

            addComponent(bindTitleMoveUp);
            addComponent(bindMoveUpOne);
            addComponent(bindMoveUpTwo);

            addComponent(bindTitleMoveDown);
            addComponent(bindMoveDownOne);
            addComponent(bindMoveDownTwo);

            addComponent(bindTitleMoveLeft);
            addComponent(bindMoveLeftOne);
            addComponent(bindMoveLeftTwo);

            addComponent(bindTitleMoveRight);
            addComponent(bindMoveRightOne);
            addComponent(bindMoveRightTwo);

            addComponent(bindTitleAtk);
            addComponent(bindAtkOne);
            addComponent(bindAtkTwo);

            addComponent(bindTitleSuck);
            addComponent(bindSuckOne);
            addComponent(bindSuckTwo);

            addComponent(bindTitleUse);
            addComponent(bindUseActionOne);
            addComponent(bindUseActionTwo);

            addComponent(backMessage);

            //addComponent(testKey);
        }

        public override void update(GameTime gameTime)
        {
            bool canExit = true;
            if (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isBtnPressed(Buttons.B, 1) || inputManager.isKeyPressed(Keys.Escape))
            {
                foreach (WndComponent component in components)
                {
                    if (component.getHasFocus())
                    {
                        canExit = false;
                        break;
                    }
                }
            }

            base.update(gameTime);

            if (volumeToggle.getIsChanged())
            {
                getAppRef().getAudioManager().setVolume((volumeToggle.getSelectedID() * 5));
            }
            if (muteToggle.getIsChanged())
            {
                getAppRef().getAudioManager().setMute(muteToggle.getSelectedID() == 1);
            }
            // including these will handle when the key presses occur
            muteToggle.setSelection(appRef.getAudioManager().getMuted() ? 1 : 0);
            volumeToggle.setSelection((int)(getAppRef().getAudioManager().getVolume() / 5));

            if (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isBtnPressed(Buttons.B, 1) || inputManager.isKeyPressed(Keys.Escape))
            {
                if(canExit)
                    appRef.setWnd(WndType.MainMenu);
            }
        }

    }
}
