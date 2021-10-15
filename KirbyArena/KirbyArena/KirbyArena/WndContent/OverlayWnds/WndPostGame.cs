using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KirbyArena
{
    public class WndPostGame : WndHandle
    {
        private ButtonCollection menu;
        private Level level;

        public WndPostGame(Rectangle displayRect, string reason, KirbyGame appRef, Level level)
            : base(WndType.PostGame, displayRect, appRef)
        {
            this.level = level;
            WndComponent background = new WndComponent(displayRect, loadTexture("WndContent\\HUD\\DialogBackground"));
            addComponent(background);
            SpriteFont font = loadFont("hugeFont");

            Rectangle titleRect = new Rectangle(displayRect.Left, displayRect.Top + (int)(displayRect.Height * 0.05), displayRect.Width, (int)(displayRect.Height * 0.15));
            Label titleLabel = new Label(titleRect, "Game Over: " + reason, font);
            titleLabel.setColor(Color.Black);
            titleLabel.centreInRect(titleRect);
            addComponent(titleLabel);

            Rectangle mainRect = new Rectangle(displayRect.Left + (int)(displayRect.Width * 0.1), displayRect.Top + (int)(displayRect.Height * 0.15),
                                                displayRect.Width - (int)(displayRect.Width * 0.2), (int)(displayRect.Height * 0.6));
            int noPlayers = 2;// (appRef.getGameOptions().gameMode == GameOptions.GameMode.Single) ? 1 : 2;
            int modeAdder = 0;
            if (appRef.getGameOptions().gameMode == GameOptions.GameMode.Coop)
            {
                modeAdder = 2;
            }
            else
            {
                modeAdder = 1;
            }

            LayoutManger layoutLabels = new LayoutManger(mainRect, 3 * noPlayers + modeAdder, 2);
            double[] totals = new double[noPlayers];
            double comboTotal = 0;

            for (int i = 1; i <= noPlayers; i++)
            {
                Label pTitle = new Label(layoutLabels.nextRect(), "Player " + i, font);
                layoutLabels.nextRect(); // skip one
                Player p = level.getPlayer(i);
                double totalScore = p.getScore();
                Label pScore = new Label(layoutLabels.nextRect(), "Score:", font);
                Label pScore2 = new Label(layoutLabels.nextRect(), p.getScore() + "", font);
                Label pBonus = new Label(layoutLabels.nextRect(), "Bonus:", font);
                double lives = p.getLives();
                double killBonus = 0;
                double otherLives = (i == 1) ? level.getPlayer(2).getLives() : level.getPlayer(1).getLives();
                if (lives > 0 && otherLives == 0 && appRef.getGameOptions().gameMode != GameOptions.GameMode.Coop)
                {
                    killBonus = 50;
                }
                double bonus = (lives == -0.5) ? 0 : lives * 5;
                totalScore += bonus + killBonus;
                totals[i-1] = totalScore;
                Rectangle pTotalRect = new Rectangle(pScore2.getRect().X + (pScore2.getRect().Width / 2), pScore2.getRect().Y, pScore2.getRect().Width / 2, pScore2.getRect().Height * 2);
                Label pTotal = new Label(pTotalRect, totalScore + "", font);
                pTotal.centreInRect();
                comboTotal += totalScore;
                Label pBonus2 = new Label(layoutLabels.nextRect(), (bonus+killBonus) + "", font);
                addComponent(pTitle);
                addComponent(pScore);
                addComponent(pScore2);
                addComponent(pBonus);
                addComponent(pBonus2);
                addComponent(pTotal);
            }

            if (appRef.getGameOptions().gameMode == GameOptions.GameMode.Coop)
            {
                Label cScore = new Label(layoutLabels.nextRect(), "Combined Score:", font);
                Label cScore2 = new Label(layoutLabels.nextRect(), comboTotal + "", font);
                Rectangle victorRect = layoutLabels.nextRect();
                victorRect.Width = mainRect.Width;

                string victorString = "Level Failed!";
                if (level.getPlayer(1).getLives() > 0
                    && level.getPlayer(2).getLives() > 0)
                    victorString = "Level Victory!";
                Label victor = new Label(victorRect, victorString, font);
                victor.centreInRect();
                addComponent(cScore);
                addComponent(cScore2);
                addComponent(victor);
            }
            else
            {
                modeAdder = 1;

                string victorString = "It's a Draw!";
                if (totals[0] > totals[1])
                    victorString = "Player 1 Wins!";
                else if (totals[1] > totals[0])
                    victorString = "Player 2 Wins!";
                /*if (appRef.getGameOptions().lives == -0.5)
                {
                    if (totals[0] > totals[1])
                        victorString = "Player 1 Wins!";
                    else if (totals[1] > totals[0])
                        victorString = "Player 2 Wins!";
                }
                else
                {
                    if (level.getPlayer(1).getLives() == 0)
                        victorString = "Player 2 Wins!";
                    else if (level.getPlayer(2).getLives() == 0)
                        victorString = "Player 1 Wins!";
                    else if (totals[0] > totals[1])
                        victorString = "Player 1 Wins!";
                    else if (totals[1] > totals[0])
                        victorString = "Player 2 Wins!";
                }*/
                Rectangle victorRect = layoutLabels.nextRect();
                victorRect.Width = mainRect.Width;
                Label victor = new Label(victorRect, victorString, font);
                victor.centreInRect();
                addComponent(victor);
            }

            Rectangle menuRect = new Rectangle((int)(displayRect.X + displayRect.Width * 0.1f), displayRect.Top + (int)(displayRect.Height * 0.75f),
                                            (int)(displayRect.Width * 0.8f), (int)(displayRect.Height * 0.15f));
            menu = new ButtonCollection(menuRect);

            LayoutManger layout = new LayoutManger(menuRect, 1, 2);
            Texture2D btnOutTexture = loadTexture("WndContent\\Components\\btnOut");
            Texture2D btnOverTexture = loadTexture("WndContent\\Components\\btnOver");

            TextButton retryBtn = new TextButton(layout.nextRect(), "Retry", font, btnOverTexture, btnOutTexture, true, 0);
            retryBtn.setFontColor(Color.Maroon);
            retryBtn.setSelectedFontColor(Color.Black);
            TextButton mainmenuBtn = new TextButton(layout.nextRect(), "Main Menu", font, btnOverTexture, btnOutTexture, false, 1);
            mainmenuBtn.setFontColor(Color.Maroon);
            mainmenuBtn.setSelectedFontColor(Color.Black);

            menu.add(retryBtn);
            menu.add(mainmenuBtn);

            addComponent(menu);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isKeyPressed(Keys.Escape))
            {
                appRef.setWnd(WndType.MainMenu);
            }
            else if (inputManager.isKeyPressed(Keys.Enter) || inputManager.isBtnPressed(Buttons.A, 1)
                || menu.isBtnClicked())
            {
                menu.playSelectedSound();
                if (menu.getSelected().getActionID() == 0)
                {
                    appRef.setWnd(WndType.LevelViewer);
                }
                else
                {
                    appRef.setWnd(WndType.MainMenu);
                }
            }
            else if (inputManager.isKeyPressed(Keys.Right) || inputManager.isBtnPressed(Buttons.DPadRight, 1))
            {
                menu.next();
            }
            else if (inputManager.isKeyPressed(Keys.Left) || inputManager.isBtnPressed(Buttons.DPadLeft, 1))
            {
                menu.previous();
            }
        }
    }
}
