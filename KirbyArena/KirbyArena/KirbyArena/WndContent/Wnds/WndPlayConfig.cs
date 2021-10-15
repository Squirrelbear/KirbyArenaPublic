using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KirbyArena
{
    public class WndPlayConfig : WndHandle
    {
        protected ToggleOption modeToggle, timeToggle, difficultyToggle;
        protected HealthBar hpToggle;
        protected TabOrder tabOrder;
        protected TextButton menuBtn, playBtn;
        protected ButtonCollection mapCollection;

        public WndPlayConfig(Rectangle displayRect, KirbyGame appRef)
            : base(WndType.PlayConfig, displayRect, appRef)
        {
            Texture2D btnOutTexture = loadTexture("WndContent\\Components\\btnOut");
            Texture2D btnOverTexture = loadTexture("WndContent\\Components\\btnOver");
            Texture2D leftArrowTexture = loadTexture("WndContent\\Components\\leftarrow");
            Texture2D rightArrowTexture = loadTexture("WndContent\\Components\\rightarrow");
            Texture2D heartFullTexture = loadTexture("WndContent\\HUD\\heartfull");
            Texture2D heartHalfTexture = loadTexture("WndContent\\HUD\\hearthalf");
            SpriteFont font = loadFont("hugeFont");
            SpriteFont smallFont = loadFont("smallFont");

            WndComponent background = new WndComponent(displayRect, loadTexture("WndContent\\MainMenu\\othermenubg"));

            Rectangle titleRect = new Rectangle(displayRect.Left, displayRect.Top, displayRect.Width, (int)(displayRect.Height * 0.1));
            Label title = new Label(titleRect, "Configure Play Options", font);
            title.centreInRect(titleRect);

            int widthColLeft = (int)(displayRect.Width * 0.3);
            int widthColRight = (int)(displayRect.Width * 0.7);
            int basicHeight = (int)(displayRect.Height * 0.1);

            // Mode Toggle
            Rectangle modeToggleRect = new Rectangle(displayRect.Left + widthColLeft,
                                                     displayRect.Top + basicHeight,
                                                     widthColRight, basicHeight);
            List<string> modeOptions = new List<string>();
            modeOptions.Add("Single Player");
            modeOptions.Add("2P Coop");
            modeOptions.Add("2P Versus");
            modeOptions.Add("AI Battle");
            modeToggle = new ToggleOption(modeToggleRect, modeOptions, leftArrowTexture, rightArrowTexture, font);

            Rectangle modeLabelRect = new Rectangle(displayRect.Left + 5,displayRect.Top + basicHeight,
                                                     (int)font.MeasureString("Game Mode:").X, basicHeight);
            Label modeLabel = new Label(modeLabelRect, "Game Mode:", font);
            modeLabel.centreInRect(modeLabelRect);

            // Time Toggle
            Rectangle timeToggleRect = new Rectangle(displayRect.Left + widthColLeft,
                                                     displayRect.Top + basicHeight * 2,
                                                     widthColRight, basicHeight);
            List<string> timeOptions = new List<string>();
            timeOptions.Add("3 min");
            timeOptions.Add("5 min");
            timeOptions.Add("10 min");
            timeOptions.Add("15 min");
            timeOptions.Add("Unlimited");
            timeToggle = new ToggleOption(timeToggleRect, timeOptions, leftArrowTexture, rightArrowTexture, font);

            Rectangle timeLabelRect = new Rectangle(displayRect.Left + 5, displayRect.Top + basicHeight * 2,
                                                     (int)font.MeasureString("Game Time:").X, basicHeight);
            Label timeLabel = new Label(timeLabelRect, "Game Time:", font);
            timeLabel.centreInRect(timeLabelRect);

            // Difficulty Toggle
            Rectangle difficultyToggleRect = new Rectangle(displayRect.Left + widthColLeft,
                                                     displayRect.Top + basicHeight * 3,
                                                     widthColRight, basicHeight);
            List<string> difficultyOptions = new List<string>();
            difficultyOptions.Add("Easy");
            difficultyOptions.Add("Medium");
            difficultyOptions.Add("Hard");
            difficultyToggle = new ToggleOption(difficultyToggleRect, difficultyOptions, leftArrowTexture, rightArrowTexture, font);

            Rectangle difficultyLabelRect = new Rectangle(displayRect.Left + 5, displayRect.Top + basicHeight * 3,
                                                     (int)font.MeasureString("Difficulty:").X, basicHeight);
            Label difficultyLabel = new Label(difficultyLabelRect, "Difficulty:", font);
            difficultyLabel.centreInRect(difficultyLabelRect);

            // Lives Toggle
            Rectangle hpRect = new Rectangle(displayRect.Left + widthColLeft + (widthColRight - basicHeight * 10) / 2,
                                                     displayRect.Top + basicHeight * 4,
                                                     basicHeight * 10, basicHeight);
            hpToggle = new HealthBar(hpRect, heartFullTexture, heartHalfTexture, font);
            hpToggle.setTrackMouse(true);

            Rectangle hpLabelRect = new Rectangle(displayRect.Left + 5, displayRect.Top + basicHeight * 4,
                                                     (int)font.MeasureString("Initial Lives:").X, basicHeight);
            Label hpLabel = new Label(hpLabelRect, "Initial Lives:", font);
            hpLabel.centreInRect(hpLabelRect);

            // Map selection
            Rectangle mapRect = new Rectangle(displayRect.Left + widthColLeft,
                                                     displayRect.Top + basicHeight * 5,
                                                     basicHeight * 7, basicHeight);
            Panel mapSelector = new Panel(mapRect);
            LayoutManger mapLayout = new LayoutManger(mapRect, 1, 7);
            Button btnSnowLevel = new Button(mapLayout.nextRect(), loadTexture("WndContent\\LevelIcons\\LevelIconChristmasOn"), loadTexture("WndContent\\LevelIcons\\LevelIconChristmasOff"), true, (int)GameOptions.LevelID.SnowLevel);
            Button btnIslandLevel = new Button(mapLayout.nextRect(), loadTexture("WndContent\\LevelIcons\\LevelIconIslandOn"), loadTexture("WndContent\\LevelIcons\\LevelIconIslandOff"), false, (int)GameOptions.LevelID.IslandLevel);
            Button btnDesertLevel = new Button(mapLayout.nextRect(), loadTexture("WndContent\\LevelIcons\\LevelIconDesertOn"), loadTexture("WndContent\\LevelIcons\\LevelIconDesertOff"), false, (int)GameOptions.LevelID.DesertLevel);
            Button btnNinjaLevel = new Button(mapLayout.nextRect(), loadTexture("WndContent\\LevelIcons\\LevelIconNinjaOn"), loadTexture("WndContent\\LevelIcons\\LevelIconNinjaOff"), false, (int)GameOptions.LevelID.NinjaLevel);
            //Button btnPacManLevel = new Button(mapLayout.nextRect(), loadTexture("WndContent\\LevelIcons\\LevelIconNinjaOn"), loadTexture("WndContent\\LevelIcons\\LevelIconNinjaOff"), false, (int)GameOptions.LevelID.PacManLevel);
            mapSelector.addComponent(btnSnowLevel);
            mapSelector.addComponent(btnIslandLevel);
            mapSelector.addComponent(btnDesertLevel);
            mapSelector.addComponent(btnNinjaLevel);
            //mapSelector.addComponent(btnPacManLevel);
            mapCollection = new ButtonCollection(mapRect);
            mapCollection.add(btnSnowLevel);
            mapCollection.add(btnIslandLevel);
            mapCollection.add(btnDesertLevel);
            mapCollection.add(btnNinjaLevel);
            //mapCollection.add(btnPacManLevel);

            Rectangle mapLabelRect = new Rectangle(displayRect.Left + 5, displayRect.Top + basicHeight * 5,
                                                     (int)font.MeasureString("Map Select:").X, basicHeight);
            Label mapLabel = new Label(hpLabelRect, "Map Select:", font);
            mapLabel.centreInRect(mapLabelRect);

            // Buttons
            int btnWidth = (int)(displayRect.Width * 0.25);
            Rectangle menuBtnRect = new Rectangle(displayRect.Center.X - btnWidth - (int)(displayRect.Width * 0.05),
                                                     displayRect.Top + basicHeight * 7,
                                                     btnWidth, basicHeight);
            menuBtn = new TextButton(menuBtnRect, "Main Menu", font, btnOverTexture, btnOutTexture, false, 0);

            Rectangle playBtnRect = new Rectangle(displayRect.Center.X + (int)(displayRect.Width * 0.05),
                                                     displayRect.Top + basicHeight * 7,
                                                     btnWidth, basicHeight);
            playBtn = new TextButton(playBtnRect, "Start Game", font, btnOverTexture, btnOutTexture, false, 1);

            addComponent(background);
            addComponent(title);
            addComponent(modeLabel);
            addComponent(modeToggle);
            addComponent(timeLabel);
            addComponent(timeToggle);
            addComponent(difficultyLabel);
            addComponent(difficultyToggle);
            addComponent(hpLabel);
            addComponent(hpToggle);
            addComponent(mapLabel);
            addComponent(mapSelector);
            addComponent(menuBtn);
            addComponent(playBtn);

            tabOrder = new TabOrder(inputManager);
            tabOrder.addComponent(modeToggle);
            tabOrder.addComponent(timeToggle);
            tabOrder.addComponent(difficultyToggle);
            tabOrder.addComponent(hpToggle);
            tabOrder.addComponent(mapCollection);
            tabOrder.addComponent(menuBtn);
            tabOrder.addComponent(playBtn);

            List<Rectangle> tabRects = new List<Rectangle>();
            Rectangle divArea = new Rectangle(displayRect.X, displayRect.Y + basicHeight, displayRect.Width, (int)(displayRect.Height * 0.5));
            LayoutManger tabOutlineLayout = new LayoutManger(divArea, 5, 1);
            tabRects.Add(tabOutlineLayout.nextRect());
            tabRects.Add(tabOutlineLayout.nextRect());
            tabRects.Add(tabOutlineLayout.nextRect());
            tabRects.Add(tabOutlineLayout.nextRect());
            tabRects.Add(tabOutlineLayout.nextRect());
            tabRects.Add(new Rectangle(-1, -1, 0, 0));
            tabRects.Add(new Rectangle(-1, -1, 0, 0));
            TabOutliner tabOutliner = new TabOutliner(loadTexture("WndContent\\Components\\BlackBorder"),
                                                        tabOrder, tabRects);
            addComponent(tabOutliner);

            modeToggle.setFocusRect(tabRects[0]);
            timeToggle.setFocusRect(tabRects[1]);
            difficultyToggle.setFocusRect(tabRects[2]);
            hpToggle.setFocusRect(tabRects[3]);
            mapCollection.setFocusRect(tabRects[4]);

            GameOptions options = appRef.getGameOptions();

            difficultyToggle.setSelection((int)options.difficulty);
            int shuffle = (int)options.levelID;
            for (int i = 0; i < shuffle; i++)
            {
                mapCollection.next();
            }
            modeToggle.setSelection((int)options.gameMode);
            timeToggle.setSelection(getLevelTimeID(options.levelTime));
            hpToggle.setHearts(options.lives);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (inputManager.isKeyPressed(Keys.Escape) || inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isBtnPressed(Buttons.B, 1))
            {
                saveOptions();
                appRef.setWnd(WndType.MainMenu);
                return;
            }

            tabOrder.update(gameTime);

            if ((menuBtn.getHasFocus() && !playBtn.getIsClicked()) || menuBtn.getIsClicked())
            {
                playBtn.setSelected(false);
                menuBtn.setSelected(true);
                if (inputManager.isKeyPressed(Keys.Enter) || inputManager.isBtnPressed(Buttons.A, 1) || menuBtn.getIsClicked())
                {
                    saveOptions();
                    appRef.setWnd(WndType.MainMenu);
                    return;
                }
                else if (inputManager.isKeyPressed(Keys.Left) || inputManager.isKeyPressed(Keys.Right)
                    || inputManager.isBtnPressed(Buttons.DPadLeft, 1) || inputManager.isBtnPressed(Buttons.DPadRight, 1))
                {
                    tabOrder.setFocus(playBtn);
                    menuBtn.setSelected(false);
                }
            }
            else if (playBtn.getHasFocus() && !menuBtn.getIsClicked() || playBtn.getIsClicked())
            {
                menuBtn.setSelected(false);
                playBtn.setSelected(true);
                if (inputManager.isKeyPressed(Keys.Enter) || inputManager.isBtnPressed(Buttons.A, 1) || playBtn.getIsClicked())
                {
                    saveOptions();
                    appRef.setWnd(WndType.LevelViewer);
                    return;
                }
                else if (inputManager.isKeyPressed(Keys.Left) || inputManager.isKeyPressed(Keys.Right)
                    || inputManager.isBtnPressed(Buttons.DPadLeft, 1) || inputManager.isBtnPressed(Buttons.DPadRight, 1))
                {
                    tabOrder.setFocus(menuBtn);
                    playBtn.setSelected(false);
                }
            }
            else
            {
                menuBtn.setSelected(false);
                playBtn.setSelected(false);
            }
        }

        private void saveOptions()
        {
            GameOptions options = new GameOptions();
            options.difficulty = (GameOptions.Difficulty)difficultyToggle.getSelectedID();
            options.levelID = (GameOptions.LevelID)mapCollection.getSelected().getActionID();
            options.gameMode = (GameOptions.GameMode)modeToggle.getSelectedID();
            options.levelTime = getLevelTime();
            options.lives = hpToggle.getHearts();
            appRef.setGameOptions(options);
        }

        private int getLevelTime()
        {
            if (timeToggle.getSelectedID() == 4)
            {
                return -1;
            }

            return int.Parse(timeToggle.getSelectedText().Split(' ')[0]) * 1000 * 60;
        }

        private int getLevelTimeID(int time)
        {
            int modTime = time / (1000 * 60);
            switch (modTime)
            {
                case 3:
                    return 0;
                case 5:
                    return 1;
                case 10:
                    return 2;
                case 15:
                    return 3;
                default:
                    return 4;
            }
        }
    }
}
