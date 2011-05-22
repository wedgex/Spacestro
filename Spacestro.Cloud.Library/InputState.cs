using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Spacestro.Cloud.Library
{
    public struct InputState
    {
        public bool Left { get; set; }
        public bool Up { get; set; }
        public bool Right { get; set; }
        public bool Down { get; set; }
                

        public InputState(bool left, bool up, bool right, bool down) : this()
        {
            Left = left;
            Up = up;
            Right = right;
            Down = down;
        }

        public bool HasKeyDown()
        {
            return this.Left || this.Right || this.Up || this.Down;
        }
    }
}
