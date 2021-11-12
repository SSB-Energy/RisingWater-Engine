using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisingWater
{
    public enum ControlBits
    {
        UP = 1 << 0,
        DOWN = 1 << 1,
        LEFT = 1 << 2, 
        RIGHT = 1 << 3,
        JUMP = 1 << 4
    }

    /// <summary>
    /// Controls class. Stores controls packets as integers and operates on them using
    /// bitwise operators, essentially treating them as boolean arrays.
    /// </summary>
    public class Controls
    {
        public const int UP = (int)ControlBits.UP;
        public const int DOWN = (int)ControlBits.DOWN;
        public const int LEFT = (int)ControlBits.LEFT;
        public const int RIGHT = (int)ControlBits.RIGHT;
        public const int JUMP = (int)ControlBits.JUMP;

        public int Raw;
        public int RawPrev;

        public float LStick_X;
        public float LStick_Y;
        public float RStick_X;
        public float RStick_Y;
        public float LTrigger;
        public float RTrigger;

        public Controls(int raw = 0, int rawLast = 0)
        {
            Raw = raw;
            RawPrev = rawLast;
        }

        public bool Held(int raw)
        {
            return (Raw & raw) == raw;
        }

        public bool Pressed(int raw)
        {
            return Held(raw) && ((RawPrev & raw) == 0);
        }

        public bool Released(int raw)
        {
            return ((Raw & raw) == 0) && ((RawPrev & raw) == raw);
        }

        public void Press(int raw)
        {
            Raw |= raw;
        }

        public void Release(int raw)
        {
            Raw &= ~raw;
        }

        public void Tick()
        {
            RawPrev = Raw;
        }

        public static Controls operator +(Controls a, Controls b)
        {
            return new Controls((byte)(a.Raw | b.Raw), a.RawPrev);
        }

        public static Controls operator +(Controls a, byte b)
        {
            return new Controls((byte)(a.Raw | b), a.RawPrev);
        }

        public static Controls operator -(Controls a, Controls b)
        {
            return new Controls((byte)(a.Raw & b.Raw), a.RawPrev);
        }

        public static Controls operator -(Controls a, byte b)
        {
            return new Controls((byte)(a.Raw & b), a.RawPrev);
        }

        public static explicit operator int(Controls a)
        {
            return a.Raw;
        }

        public static explicit operator Controls(int a)
        {
            return new Controls(a);
        }
    }
}
