using Architect;
using System;
using System.Collections.Generic;
using System.Text;
using DavidYork;

// Simple test program to generate a dungeon and then display it on the screen with ASCII
namespace Test.DungeonFloorGenerator {
    class Program {
        static void Main(string[] args) {
            Dungeon.DefaultMinDepth = 1;
            Dungeon.DefaultMaxDepth = 3;
            Dungeon.DefaultMaxRoomsPerFloor = new Size(15, 10);
            Dungeon.DefaultMinRoomsPerFloor = new Size(1, 1);

            do {
                Console.Clear();
                var generator = new Dungeon();
                generator.Generate();

                foreach (var floor in generator.Floors) {
                    var Rooms = floor.Rooms;
                    for (int y = 0; y < Rooms.GetLength(1); y++) {
                        string[] rows = new string[] { "", "", "", "", "", "" };
                        for (int x = 0; x < Rooms.GetLength(0); x++) {
                            var room = Rooms[x, y];
                            renderRoom(room, rows);
                        }
                        foreach (var s in rows)
                            Console.WriteLine(s);
                    }
                    Console.WriteLine("\n");
                }
                Console.WriteLine("Enter to generate another dungeon");
                Console.WriteLine("Space + Enter to quit");
            } while (Console.Read() != ' ');

        }

        static void renderRoom(Dungeon.Room room, string[] rows) {
            rows[0] += (room.ExitNorth) ? "  ||    " : "        ";
            rows[1] += (room.ExitNorth) ? "  ||    " : "        ";
            rows[2] += "------  ";
            char ch = (char)((int)'a' + room.Origin);
            if (room.Origin <= 2 && room.Step == 1) {
                if (room.Origin == 1) {
                    rows[3] += String.Format("| SS |{0}", (room.ExitEast) ? "--" : "  ");
                    rows[4] += String.Format("| SS |{0}", (room.ExitEast) ? "--" : "  ");
                } else {
                    rows[3] += String.Format("| EE |{0}", (room.ExitEast) ? "--" : "  ");
                    rows[4] += String.Format("| EE |{0}", (room.ExitEast) ? "--" : "  ");
                }
            } else {
                rows[3] += String.Format("|{0,3} |{1}", room.Origin, (room.ExitEast) ? "--" : "  ");
                if (room.Terminal) {
                    rows[4] += String.Format("|{0,3} |{1}", "T" + room.Step.ToString(), (room.ExitEast) ? "--" : "  ");
                } else {
                    rows[4] += String.Format("|{0,3} |{1}", (room.Step == 1) ? "* 1" : room.Step.ToString(), (room.ExitEast) ? "--" : "  ");
                }
            }
            rows[5] += "------  ";
        }
    }
}
