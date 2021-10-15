using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace KirbyArena
{
    public class GameOptions
    {
        public enum GameMode { Single, Coop, Versus, AIBattle };
        public enum Difficulty { Easy = 0, Medium = 1, Hard = 2 };
        public enum LevelID { SnowLevel = 0, IslandLevel = 1, DesertLevel = 2, NinjaLevel = 3, PacManLevel = 4, TestLevel = 99 };

        internal GameMode gameMode;
        internal Difficulty difficulty;
        internal LevelID levelID;

        internal int levelTime;
        internal double lives;

        internal InputAssociation p1Input;
        internal InputAssociation p2Input;

        internal float volume;
        internal bool mute;

        public GameOptions()
        {
            difficulty = GameOptions.Difficulty.Easy;
            gameMode = GameOptions.GameMode.Versus;
            levelID = GameOptions.LevelID.SnowLevel;
            levelTime = 3 * 1000 * 60;
            lives = 5;

            if (!loadFileDefaults())
            {
                resetDefaults();
            }
        }

        public void resetDefaults()
        {
            p1Input = getDefaultInput(1);
            p2Input = getDefaultInput(2);

            volume = 100;
            mute = false;
            saveFileDefaults();
        }

        public static InputAssociation getDefaultInput(int player)
        {
            if (player == 1)
            {
                InputAssociation input = new InputAssociation();
                input.KeyAttack = Keys.Space;
                input.KeyMoveDown = Keys.S;
                input.KeyMoveLeft = Keys.A;
                input.KeyMoveRight = Keys.D;
                input.KeyMoveUp = Keys.W;
                input.KeySuckBlow = Keys.LeftShift;
                input.KeyAction = Keys.LeftAlt;
                return input;
            }
            else
            {
                InputAssociation input = new InputAssociation();
                input.KeyAttack = Keys.RightShift;
                input.KeyMoveDown = Keys.Down;
                input.KeyMoveLeft = Keys.Left;
                input.KeyMoveRight = Keys.Right;
                input.KeyMoveUp = Keys.Up;
                input.KeySuckBlow = Keys.RightControl;
                input.KeyAction = Keys.RightAlt;
                return input;
            }
        }

        public Keys getKey(int player, int id)
        {
            if (player == 1)
            {
                return (Keys)p1Input.getKeysAsData()[id];
            }
            else
            {
                return (Keys)p2Input.getKeysAsData()[id];
            }
        }

        public void setKey(int player, int id, Keys key)
        {
            if (player == 1)
            {
                int[] data = p1Input.getKeysAsData();
                data[id] = (int)key;
                p1Input.loadStoredKeys(data);
            }
            else
            {
                int[] data = p2Input.getKeysAsData();
                data[id] = (int)key;
                p2Input.loadStoredKeys(data);
            }

            saveFileDefaults();
        }

        public void saveFileDefaults()
        {
            string[] lines = new string[3];

            int[] data1 = p1Input.getKeysAsData();
            string outData1 = "" + data1[0];
            for (int i = 1; i < data1.Length; i++)
                outData1 += " " + data1[i];

            int[] data2 = p2Input.getKeysAsData();
            string outData2 = "" + data2[0];
            for (int i = 1; i < data2.Length; i++)
                outData2 += " " + data2[i];

            lines[0] = outData1;
            lines[1] = outData2;
            lines[2] = mute + " " + volume;

            File.WriteAllLines("settings.dat", lines);
        }

        public bool loadFileDefaults()
        {
            if (!File.Exists("settings.dat"))
                return false;

            string[] lines = File.ReadAllLines("settings.dat");

            if (lines.Length < 3)
            {
                return false;
            }

            string[] dataAsString = lines[0].Split(' ');

            if (dataAsString.Length != 7)
            {
                return false;
            }

            int[] dataAsInt = new int[7];
            for (int i = 0; i < 7; i++)
            {
                bool successParse = int.TryParse(dataAsString[i], out dataAsInt[i]);
                if (!successParse)
                    return false;
            }

            string[] dataAsString2 = lines[1].Split(' ');

            if (dataAsString2.Length != 7)
            {
                return false;
            }

            int[] dataAsInt2 = new int[7];
            for (int i = 0; i < 7; i++)
            {
                bool successParse = int.TryParse(dataAsString2[i], out dataAsInt2[i]);
                if (!successParse)
                    return false;
            }

            string[] mediaSettings = lines[2].Split(' ');

            bool successParsed = bool.TryParse(mediaSettings[0], out mute);
            if (!successParsed)
                return false;

            successParsed = float.TryParse(mediaSettings[1], out volume);
            if (!successParsed)
                return false;

            p1Input = new InputAssociation();
            p1Input.loadStoredKeys(dataAsInt);
            p2Input = new InputAssociation();
            p2Input.loadStoredKeys(dataAsInt2);

            return true;
        }


    }
}
