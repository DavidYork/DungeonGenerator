using System;
using System.Collections.Generic;

namespace DavidYork {
    public struct Point {
        public Point(float x, float y) { X = x; Y = y; }
        public Point(int x, int y) { X = x; Y = y; }
        public static readonly Point Zero = new Point(0, 0);
        public float X, Y;
        public int iX { get { return (int)X; } set { X = value; } }
        public int iY { get { return (int)Y; } set { Y = value; } }
        public override string ToString() {
            return String.Format("({0},{1})", X, Y);
        }
        public static bool operator==(Point lhs, Point rhs) {
            return rhs.X == lhs.X && rhs.Y == lhs.Y;
        }
        public static bool operator !=(Point lhs, Point rhs) {
            return !(lhs == rhs);
        }
    }

    public struct Size {
        public Size(float w, float h) { Width = w; Height = h; }
        public Size(int w, int h) { Width = w; Height = h; }
        public static readonly Size Zero = new Size(0, 0);
        public float Width, Height;
        public int iWidth { get { return (int)Width; } set { Width = value; } }
        public int iHeight { get { return (int)Height; } }
        public override string ToString() {
            return String.Format("({0},{1})", Width, Height);
        }
        public static bool operator !=(Size lhs, Size rhs) {
            return !(lhs == rhs);
        }
        public static bool operator ==(Size lhs, Size rhs) {
            return rhs.Width == lhs.Width && rhs.Height == lhs.Height;
        }
    }

    public enum Direction {
        Center, North, Northeast, East, Southeast, South, Southwest, West, Northwest
    }

    public enum Dpad {
        North, South, East, West
    }

    public class AlreadyGeneratedException : Exception { }
}
