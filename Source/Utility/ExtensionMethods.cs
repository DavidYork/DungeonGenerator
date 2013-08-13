using System;
using System.Collections.Generic;

namespace DavidYork {
    public static class Extensions {
        public static bool ContainsDirection(this Direction left, Direction right) {
            if (right == left) return true;
            switch (left) {
                case Direction.Center: return false;
                case Direction.West: return right == Direction.Northwest || right == Direction.Southwest;
                case Direction.East: return right == Direction.Northeast || right == Direction.Southeast;
                case Direction.North: return right == Direction.Northeast || right == Direction.Northwest;
                case Direction.South: return right == Direction.Southeast || right == Direction.Southwest;
                case Direction.Southeast: return right == Direction.South || right == Direction.East;
                case Direction.Northeast: return right == Direction.North || right == Direction.East;
                case Direction.Southwest: return right == Direction.South || right == Direction.West;
                case Direction.Northwest: return right == Direction.North || right == Direction.West;
                default: throw new ArgumentException("Do not recognize direction, cannot suggest movement point");
            }
        }

        public static Point GetMoveInDirection(this Point source, Direction dir) {
            switch (dir) {
                case Direction.Center: return source;
                case Direction.West: return new Point(source.X - 1, source.Y);
                case Direction.East: return new Point(source.X + 1, source.Y);
                case Direction.North: return new Point(source.X, source.Y + 1);
                case Direction.South: return new Point(source.X, source.Y - 1);
                case Direction.Southeast: return new Point(source.X + 1, source.Y - 1);
                case Direction.Northeast: return new Point(source.X + 1, source.Y + 1);
                case Direction.Southwest: return new Point(source.X - 1, source.Y - 1);
                case Direction.Northwest: return new Point(source.X - 1, source.Y + 1);
                default: throw new ArgumentException("Do not recognize direction, cannot suggest movement point");
            }
        }

        // Assumes coordinate system of (0,0) in northeast corner
        public static Direction GetDirectionTo(this Point source, Point dest) {

            Direction rv = Direction.Center;
            if (source.X == dest.X) {
                if (source.Y < dest.Y) rv = Direction.South;
                else if (source.Y > dest.Y) rv = Direction.North;
                else rv = Direction.Center;
            } else if (source.Y == dest.Y) {
                if (source.X < dest.X) rv = Direction.East;
                else if (source.X > dest.X) rv = Direction.West;
                else rv = Direction.Center;
            } else if (source.X < dest.X) {
                if (source.Y < dest.Y) rv = Direction.Northeast;
                else rv = Direction.Southeast;
            } else { // (source.X > dest.X)
                if (source.Y < dest.Y) rv = Direction.Northwest;
                else rv = Direction.Southwest;
            }
            return rv;
        }

        // returns a direction rotated clockwise or counterclockwise
        public static Direction Rotate(this Direction source, bool clockwise, int numTurns = 1) {
            if (source == Direction.Center) {
                Console.WriteLine("Warning, request to rotate Center direction.  This makes no sense, ignoring");
                return Direction.Center;
            }

            Direction rv = source;
            while (numTurns > 0) {
                switch (rv) {
                    case Direction.West: rv = (clockwise) ? Direction.Northwest : Direction.Southwest; break;
                    case Direction.East: rv = (clockwise) ? Direction.Southeast : Direction.Northeast; break;
                    case Direction.North: rv = (clockwise) ? Direction.Northeast : Direction.Northwest; break;
                    case Direction.South: rv = (clockwise) ? Direction.Southwest : Direction.Southeast; break;
                    case Direction.Southeast: rv = (clockwise) ? Direction.South : Direction.East; break;
                    case Direction.Northeast: rv = (clockwise) ? Direction.East : Direction.North; break;
                    case Direction.Southwest: rv = (clockwise) ? Direction.West : Direction.South; break;
                    case Direction.Northwest: rv = (clockwise) ? Direction.North : Direction.West; break;
                }
                --numTurns;
            }
            return rv;
        }

        public static bool IsDPADDirection(this Direction source) {
            return source == Direction.North || source == Direction.South || source == Direction.East || source == Direction.West;
        }

        public static float Area(this Size self) {
            return self.Width * self.Height;
        }

        public static double Distance(this Point source, Point dest) {
            if (source.X == dest.X) return Math.Abs(source.Y - dest.Y);
            if (source.Y == dest.Y) return Math.Abs(source.X - dest.X);
            double dX = source.X - dest.X;
            double dY = source.Y - dest.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public static bool Empty(this Size size) {
            return (size.Width == 0 && size.Height == 0);
        }

        public static bool Empty(this Point point) {
            return (point.X == 0 && point.Y == 0);
        }

        public static Point RandomInside(this Size area, Random rand, int buffer = 0) {
            if (buffer >= (area.iWidth+1) / 2) throw new ArgumentException();
            if (buffer >= (area.iHeight+1) / 2) throw new ArgumentException();
            return new Point(rand.Next(buffer, area.iWidth - buffer), rand.Next(buffer, area.iHeight - buffer));
        }

        public static Point Move(this Point origin, Dpad dir) {
            switch (dir) {
            case Dpad.East: origin.X += 1f; break;
            case Dpad.West: origin.X -= 1f; break;
            case Dpad.North: origin.Y -= 1f; break;
            case Dpad.South: origin.Y += 1f; break;
            }
            return origin;
        }

        public static Dpad Rotate(this Dpad source, bool clockwise, int numTurns = 1) {
            while (numTurns > 0) {
                switch (source) {
                case Dpad.West: source = (clockwise) ? Dpad.North : Dpad.South; break;
                case Dpad.East: source = (clockwise) ? Dpad.South : Dpad.North; break;
                case Dpad.North: source = (clockwise) ? Dpad.East : Dpad.West; break;
                case Dpad.South: source = (clockwise) ? Dpad.West: Dpad.East; break;
                }
                --numTurns;
            }
            return source;
        }
    
    }
}