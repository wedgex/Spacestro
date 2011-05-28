using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Spacestro.Cloud.Library
{
    public struct InputState
    {
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
      

        public bool HasKeyDown()
        {
            return this.Up || this.Down ||this.Left || this.Right;
        }

        public void resetStates()
        {
            this.Up = false;
            this.Down = false;
            this.Left = false;
            this.Right = false;
            
        }

        public bool[] getStateList()
        {
            return new bool[] { this.Up, this.Down, this.Left, this.Right };
        }
    }
}
