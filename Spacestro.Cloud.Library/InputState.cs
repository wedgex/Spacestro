﻿using System;
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
        public bool Space { get; set; }

        public InputState(int up, int down, int left, int right, int space) : this()
        {
            if (up.Equals(1))
            {
                this.Up = true;
            }
            else
            {
                this.Up = false;
            }

            if (down.Equals(1))
            {
                this.Down = true;
            }
            else
            {
                this.Down = false;
            }

            if (left.Equals(1))
            {
                this.Left = true;
            }
            else
            {
                this.Left = false;
            }

            if (right.Equals(1))
            {
                this.Right = true;
            }
            else
            {
                this.Right = false;
            }

            if (space.Equals(1))
            {
                this.Space = true;
            }
            else
            {
                this.Space = false;
            }
        }

        public bool HasKeyDown()
        {
            return this.Up || this.Down ||this.Left || this.Right || this.Space;
        }

        public void resetStates()
        {
            this.Up = false;
            this.Down = false;
            this.Left = false;
            this.Right = false;
            this.Space = false;
            
        }

        public void setStates(int up, int down, int left, int right, int space)
        {
            if (up.Equals(1))
            {
                this.Up = true;
            }
            else
            {
                this.Up = false;
            }

            if (down.Equals(1))
            {
                this.Down = true;
            }
            else
            {
                this.Down = false;
            }

            if (left.Equals(1))
            {
                this.Left = true;
            }
            else
            {
                this.Left = false;
            }

            if (right.Equals(1))
            {
                this.Right = true;
            }
            else
            {
                this.Right = false;
            }

            if (space.Equals(1))
            {
                this.Space = true;
            }
            else
            {
                this.Space = false;
            }
        }

        public bool[] getStateList()
        {
            return new bool[] { this.Up, this.Down, this.Left, this.Right, this.Space };
        }
    }
}
