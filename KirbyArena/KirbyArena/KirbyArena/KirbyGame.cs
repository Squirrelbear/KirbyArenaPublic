using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Reflection;
using System.Diagnostics;

namespace KirbyArena
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class KirbyGame : Microsoft.Xna.Framework.Game
    {
        #region Instance Variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private InputManager inputManager;
        private WndHandle curWnd;
        private GameOptions gameOptions;
        private AudioManager audioManager;

        //private int prefWidth = 1920, prefHeight = 1080;
        private Rectangle displayRect;
        private RasterizerState rasterizerState;

        // debug variables:
        private bool debugMode = false;
        #endregion

        #region Initialisation/Unloading
        public KirbyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            
            if(!debugMode)
                graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // prepare graphics configuration
            int wndHeight = GraphicsDevice.Viewport.Height;
            int wndWidth = GraphicsDevice.Viewport.Width;
            int viewHeight = wndWidth / 16 * 9;
            displayRect = new Rectangle(0, (wndHeight - viewHeight) / 2, wndWidth, viewHeight);
            rasterizerState = new RasterizerState() { ScissorTestEnable = true };

            //Console.WriteLine("Display: W:" + wndWidth + " H:" + wndHeight);
            //Console.WriteLine("Rect: X:" + displayRect.X + " Y:" + displayRect.Y + " W:" + displayRect.Width + " H:" + displayRect.Height);
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Configure input manager to expect up to 2 controllers.
            inputManager = new InputManager(2);
            List<string> songfiles = new List<string>();
            songfiles.Add("Audio/Christmas_Greengreen");
            songfiles.Add("Audio/SSBM_FountainOfDreams");
            songfiles.Add("Audio/Kirby64Track04Popstar");
            songfiles.Add("Audio/Kirby64Track12BattlewithComrades");
            songfiles.Add("Audio/Kirby64Track14BossBattle");
            songfiles.Add("Audio/Kirby64Track16RockStar");
            songfiles.Add("Audio/Kirby64Track18InsidetheRuins");
            songfiles.Add("Audio/Kirby64Track20AquaStar");
            songfiles.Add("Audio/Kirby64Track21DowntheMountainStream");
            songfiles.Add("Audio/Kirby64Track25Shiverstar");
            songfiles.Add("Audio/KirbyAirRideTrack2");
            songfiles.Add("Audio/KirbySuperStarUltra01MaskedDededeTheme");
            audioManager = new AudioManager(songfiles, AudioManager.MusicMode.Repeat, this);
            audioManager.setTrackAndPlay(0);
            //audioManager.setMute(debugMode);

            // force preload of options
            getGameOptions();
            audioManager.setMute(gameOptions.mute);
            audioManager.setVolume(gameOptions.volume);
            setWnd(WndHandle.WndType.MainMenu);

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }
        #endregion

        #region update/draw
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputManager.update(gameTime);

            // Allows the game to exit
            if (debugMode && (inputManager.isBtnPressed(Buttons.Back, 1) || inputManager.isKeyPressed(Keys.OemTilde)))
                this.Exit();

            curWnd.update(gameTime);
            audioManager.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.GraphicsDevice.ScissorRectangle = displayRect;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                      null, null, rasterizerState);
            //spriteBatch.Begin();

            curWnd.draw(spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region HelperMethods
        public InputManager getInputManager()
        {
            return inputManager;
        }

        public void setWnd(WndHandle.WndType wndType)
        {
            switch (wndType)
            {
                case WndHandle.WndType.Credits:
                    curWnd = new WndCredits(displayRect, this);
                    break;
                case WndHandle.WndType.Help:
                    curWnd = new WndHelp(displayRect, this);
                    break;
                case WndHandle.WndType.LevelViewer:
                    curWnd = new WndLevelViewer(displayRect, this);
                    break;
                case WndHandle.WndType.MainMenu:
                    curWnd = new WndMainMenu(displayRect, this);
                    break;
                case WndHandle.WndType.Options:
                    curWnd = new WndOptions(displayRect, this);
                    break;
                case WndHandle.WndType.PlayConfig:
                    curWnd = new WndPlayConfig(displayRect, this);
                    break;
                case WndHandle.WndType.ExitGame:
                    this.Exit();
                    break;
            }

            if (wndType != WndHandle.WndType.ExitGame && wndType != WndHandle.WndType.LevelViewer)
            {
                if(audioManager.getCurSongID() != 0)
                    audioManager.setTrackAndPlay(0);
                audioManager.setMode(AudioManager.MusicMode.Repeat);
            }
        }

        public void setGameOptions(GameOptions gameOptions)
        {
            this.gameOptions = gameOptions;
        }

        public GameOptions getGameOptions()
        {
            if (gameOptions == null)
            {
                gameOptions = new GameOptions();
            }

            return gameOptions;
        }

        public AudioManager getAudioManager()
        {
            return audioManager;
        }

        public string getAppVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }
        #endregion
    }
}
