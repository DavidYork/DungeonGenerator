/*
The MIT License (MIT)

Copyright (c) 2013 David York

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
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
