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
using DavidYork;

namespace Architect
{
    // Abstracts a dungeon layout. Dungeon is a series of Room instances 
    // and rules to connect them
    public class Dungeon {

        #region defaults
        // Defaults
        public static Size DefaultRoomSize = new Size(12, 9);
        public static int DefaultMinDepth = 1;
        public static int DefaultMaxDepth = 10;
        public static Size DefaultMinRoomsPerFloor = new Size(2, 2);
        public static Size DefaultMaxRoomsPerFloor = new Size(5, 5);
        public static StartDirection DefaultStartDir = StartDirection.DontCare;

        // Means every room will exist, no empty space between them
        // At the time of this writing only "true" is supported
        public static bool DefaultPacked = true;
        #endregion

        #region defines

        public enum StartDirection { West, East, South, North, Up, DontCare };

        public class Tile {
            enum Type { door, up, down, wall, ground }
        }
        
        public class Room {
            public int Origin; // Index of turtle
            public int Step; // Steps from turtle origin
            public Room(Size size, int origin, int idx) {
                Tiles = new Tile[(int)size.Width, (int)size.Height];
                Origin = origin;
                Step = idx;
            }
            public Tile[,] Tiles;
            public Point[] Down;
            public Point[] Up; // May actually be on a wall

            public bool ExitEast = false;
            public bool ExitNorth = false;
            public bool Terminal = false;
        }
        
        public class Floor {
            public Floor(Size size) {
                Rooms = new Room[(int)size.Width, (int)size.Height];
            }
            public Room[,] Rooms;
            public Point End;
            public Room At(Point pt) { return Rooms[pt.iX, pt.iY]; }
            public bool Filled {
                get {
                    foreach (var room in Rooms) {
                        if (room == null || room.Origin == 0) return false;
                    }
                    return true;
                }
            }
            public void MakeJoin(Point first, Point second) {
                if (first.Distance(second) != 1f)
                    throw new ArgumentException("Rooms must be Dpad adjacent");
                Direction facing = first.GetDirectionTo(second);
                if (facing == Direction.South || facing == Direction.West) {
                    Point tmp = first;
                    first = second;
                    second = tmp;
                }
                facing = first.GetDirectionTo(second);
                Room target = this.At(first);
                if (facing == Direction.North) target.ExitNorth = true;
                else if (facing == Direction.East) target.ExitEast = true;
                else throw new ArgumentException();
            }
            public Point? FindNullRoom() {
                Point rv = Point.Zero;
                for (rv.X = 0; rv.X < Rooms.GetLength(0); rv.X++) {
                    for (rv.Y = 0; rv.Y < Rooms.GetLength(1); rv.Y++) {
                        if (Rooms[rv.iX, rv.iY] == null) 
                            return rv;
                    }
                }
                return null;
            }
            public bool IsValid(Point point) {
                return (point.iX >= 0 && 
                    point.iY >= 0 && 
                    point.iX < Rooms.GetLength(0) && 
                    point.iY < Rooms.GetLength(1));
            }
        }
        
        #endregion

        #region public members / properties
        public Random rand;
        public bool? packed;
        public Point RoomStart;
        public Point PointInRoomStart;
        public Floor[] Floors;
        public int NumFloors { get { return Floors.Length; } }
        public int MinDepth, MaxDepth;
        public Size RoomSize;
        public Size MinRoomsPerFloor;
        public Size MaxRoomsPerFloor;
        public StartDirection? StartDir;
        public int? Seed {
            get { return _seed; }
            set {
                _seed = value;
                rand = (_seed.HasValue) ? new Random(_seed.Value) : new Random();
            }
        }
        #endregion

        #region implementation details
        int? _seed = null;
        #endregion

        #region methods
        public Dungeon(int? seed = null) {
            Seed = seed;
        }

        // Call this function to actually generate the dungeon
        public void Generate() {
            if (RoomSize.Empty()) RoomSize = DefaultRoomSize;
            if (MinDepth == 0) MinDepth = DefaultMinDepth;
            if (MaxDepth == 0) MaxDepth = DefaultMaxDepth;
            if (MinRoomsPerFloor.Empty()) 
                MinRoomsPerFloor = DefaultMinRoomsPerFloor;
            if (MaxRoomsPerFloor.Empty()) 
                MaxRoomsPerFloor = DefaultMaxRoomsPerFloor;
            if (!packed.HasValue) packed = DefaultPacked;
            if (!StartDir.HasValue) StartDir = DefaultStartDir;

            // Actually generate with generator function.  Multiple functions
            // would be selected here
            doGenerate();
        }

        void doGenerate() {
            Size floorSize = new Size(
                rand.Next(MinRoomsPerFloor.iWidth, MaxRoomsPerFloor.iWidth),
                rand.Next(MinRoomsPerFloor.iHeight, MaxRoomsPerFloor.iHeight)
                );
            Floors = new Floor[rand.Next(MinDepth, MaxDepth + 1)];

            // Pick start location
            StartDirection dir = StartDir.Value;
            if (dir == StartDirection.DontCare) {
                dir = (StartDirection)rand.Next(0, 5);
            }
            switch (dir) {
            case StartDirection.Up:
                RoomStart = floorSize.RandomInside(rand);
                break;
            case StartDirection.East:
                RoomStart = new Point(floorSize.Width - 1,
                    rand.Next(floorSize.iHeight));
                break;
            case StartDirection.West:
                RoomStart = new Point(0, rand.Next(floorSize.iHeight));
                break;
            case StartDirection.North:
                RoomStart = new Point(rand.Next(floorSize.iWidth), 0);
                break;
            case StartDirection.South:
                RoomStart = new Point(rand.Next(floorSize.iWidth),
                    floorSize.Height - 1);
                break;
            }

            // Create floors
            Point start = RoomStart;
            for (int i = 0; i < Floors.Length; i++) {

                // TODO: Select a floor generating algorithm
                // TODO: Don't ignore "packed"
                Floors[i] = doGenFloorPacked(floorSize, start);
                start = Floors[i].End;
            }
        }

        Floor doGenFloorPacked(Size size, Point start) {
            Floor floor = new Floor(size);

            // Pick end
            Point end = start;
            // TODO: magic number alert
            double distance = 0;
            for (int i = 0; i < 5; i++) {
                Point next = size.RandomInside(rand);
                if (next.Distance(start) > distance) {
                    end = next;
                    distance = next.Distance(start);
                }
            }
            floor.End = end;
            floor.Rooms[start.iX, start.iY] = new Room(RoomSize, 1, 1);
            if (start != end)
                floor.Rooms[end.iX, end.iY] = new Room(RoomSize, 2, 1);

            // Turtle from start to dead end, then end to original turtle
            int turtleIdx = 1;
            doTurtle(floor, turtleIdx, start, false);
            turtleIdx++;
            doTurtle(floor, turtleIdx, end, true);
            turtleIdx++;

            // Turtle generator
            while (!floor.Filled) {
                doTurtle(floor, turtleIdx, floor.FindNullRoom().Value, true);
                turtleIdx++;
            }

            return floor;
        }

        // Set "merge" to true to force a merge with another room which
        // has already been visited by a turtle.  This will return false
        // if there are no such rooms
        bool doTurtle(Floor floor, int origin, Point start, bool merge, 
            int idx = 1) 
        {

            // Create a room if needed
            if (floor.At(start) == null) {
                Room room = new Room(RoomSize, origin, idx);
                floor.Rooms[start.iX, start.iY] = room;
            }

            Dpad dir = (Dpad)rand.Next(0, 3);
            for (int i=0; i<4; i++) {
                Point moved = start.Move(dir);
                //Console.WriteLine(String.Format("Considering {0} {1} {2}",
                //    origin, idx, dir));
                if (floor.IsValid(moved)) {

                    // Found a new empty space with no room
                    if (floor.At(moved) == null) {

                        // Create room, join room, send turtle into room
                        floor.Rooms[moved.iX, moved.iY] = new Room(RoomSize, origin, idx + 1);
                        floor.MakeJoin(start, moved);

                        // If we're merged or not bothering to backtrack
                        // then we're done here
                        bool mergedInTheFUTURE = doTurtle(floor, origin, moved, merge, idx + 1);
                        if (mergedInTheFUTURE || !merge)
                            return true;

                    // Found a space that isn't from this turtle
                    } else if (floor.At(moved).Origin != origin) {
                        // If we are to merge then do it!
                        if (merge) {
                            //Console.WriteLine(String.Format("Merging branch {0} {1} {2}",
                            //    origin, idx, dir));
                            floor.MakeJoin(start, moved);
                            return true;
                        }
                    }
                }

                //Console.WriteLine(String.Format("Didn't like {0} {1} {2}",
                //    origin, idx, dir));
                dir = dir.Rotate(true);
            }

            // Didn't ever find a termination condition down this path
            //Console.WriteLine(String.Format("FAIL fucking off {0} {1} {2}",
            //    origin, idx, dir));
            floor.At(start).Terminal = true;
            return false;
        }
        #endregion
    }
}
