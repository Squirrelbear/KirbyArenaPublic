using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace KirbyArena
{
    public class InputAssociation
    {
        public Keys KeyMoveUp, KeyMoveDown, KeyMoveRight, KeyMoveLeft;
        public Keys KeySuckBlow, KeyAttack, KeyAction;

        public bool loadStoredKeys(int[] data)
        {
            if (data.Length != 7)
            {
                return false;
            }

            KeyMoveUp = (Keys)data[0];
            KeyMoveDown = (Keys)data[1];
            KeyMoveRight = (Keys)data[2];
            KeyMoveLeft = (Keys)data[3];
            KeyAttack = (Keys)data[4];
            KeySuckBlow = (Keys)data[5];
            KeyAction = (Keys)data[6];

            return true;
        }

        public int[] getKeysAsData()
        {
            int[] data = new int[7];

            data[0] = (int)KeyMoveUp;
            data[1] = (int)KeyMoveDown;
            data[2] = (int)KeyMoveRight;
            data[3] = (int)KeyMoveLeft;
            data[4] = (int)KeyAttack;
            data[5] = (int)KeySuckBlow;
            data[6] = (int)KeyAction;

            return data;
        }
    }
}
