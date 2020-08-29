using GreatMachine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreatMachine.Models
{
    enum Directions { North, East, South, West }

    class MazeCell
    {
        private MazeCell north;
        private MazeCell south;
        private MazeCell east;
        private MazeCell west;

        public MazeCell North
        {
            get { return north; }
            set
            {
                north = value;
                value.south = this;
            }
        }
        public MazeCell East
        {
            get { return east; }
            set
            {
                east = value;
                value.west = this;
            }
        }
        public MazeCell South
        {
            get { return south; }
            set
            {
                south = value;
                value.north = this;
            }
        }
        public MazeCell West
        {
            get { return west; }
            set
            {
                west = value;
                value.east = this;
            }
        }
        public bool Visited { get; set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public MazeCell(int x, int y)
        {
            X = x;
            Y = y;
            east = null;
            north = null;
            west = null;
            south = null;
            Visited = false;
        }
    }

    public class MazeGenerator
    {
        private readonly int Width;
        private readonly int Height;
        private readonly List<MazeCell> Maze = new List<MazeCell>();

        private int P(int x, int y) => PositionHelper.Convert2Dto1D(x, y, Width);

        public MazeGenerator(int width, int height)
        {
            var rand = Main.Instance.Random;
            Width = width;
            Height = height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Maze.Add(new MazeCell(x, y));
                }
            }

            var stack = new Stack<MazeCell>();
            var first = Maze.First();
            first.Visited = true;
            stack.Push(first);

            int VisitedCells = 1;

            while (stack.Count > 0)
            {
                var neighbours = new List<Directions>();
                var currentItem = stack.Peek();
                currentItem.Visited = true;
                var x = currentItem.X;
                var y = currentItem.Y;

                var north = Maze.SingleOrDefault(m => (m.X == x) && (m.Y == (y - 1)) && !m.Visited);
                var east = Maze.SingleOrDefault(m => (m.X == (x + 1)) && (m.Y == y) && !m.Visited);
                var south = Maze.SingleOrDefault(m => (m.X == x) && (m.Y == (y + 1)) && !m.Visited);
                var west = Maze.SingleOrDefault(m => (m.X == (x - 1)) && (m.Y == y) && !m.Visited);

                if (north != null) neighbours.Add(Directions.North);
                if (south != null) neighbours.Add(Directions.South);
                if (east != null) neighbours.Add(Directions.East);
                if (west != null) neighbours.Add(Directions.West);

                if (neighbours.Count > 0)
                {
                    var dir = neighbours[rand.Next(neighbours.Count)];

                    // Create a path between the neighbour and the current cell                    
                    switch (dir)
                    {
                        case Directions.North:
                            currentItem.North = north;
                            stack.Push(north);
                            break;

                        case Directions.East:
                            currentItem.East = east;
                            stack.Push(east);
                            break;

                        case Directions.South:
                            currentItem.South = south;
                            stack.Push(south);
                            break;

                        case Directions.West:
                            currentItem.West = west;
                            stack.Push(west);
                            break;
                    }
                    VisitedCells++;
                }
                else
                {
                    break;
                    //stack.Pop(); // backtrack
                }
            }
        }

        public List<BaseEntity> GetWalls(int cellsize)
        {
            var wallCoords = new HashSet<Tuple<int, int>>();
            var walls = new List<BaseEntity>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var sector = Maze[P(x, y)];

                    // Debug: add text
                    walls.Add(new TextEntity(
                        x * (cellsize + 1) + 1,
                        y * (cellsize + 1) + 1,
                        $"{x},{y}" +
                        $"{(sector.North != null ? " N" + " " + sector.North.X + ", " + sector.North.Y : " ")}" +
                        $"{(sector.East != null ? " E" + " " + sector.East.X + ", " + sector.East.Y : " ")}" +
                        $"{(sector.South != null ? " S" + " " + sector.South.X + ", " + sector.South.Y : " ")}" +
                        $"{(sector.West != null ? " W" + " " + sector.West.X + ", " + sector.West.Y : " ")}"));

                    for (int j = 0; j < cellsize; j++)
                    {
                        if (sector.North == null)
                        {
                            wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) + j, y * (cellsize + 1) - 1));
                        }

                        if (sector.East == null)
                        {
                            wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) + cellsize, y * (cellsize + 1) + j));
                        }
                        if (sector.South == null)
                        {
                            wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) + j, y * (cellsize + 1) + cellsize));
                        }
                        if (sector.West == null)
                        {
                            wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) - 1, y * (cellsize + 1) + j));
                        }
                    }
                    if (sector.South == null || sector.East == null)
                    {
                        wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) + cellsize, y * (cellsize + 1) + cellsize));
                    }
                    if (sector.North == null || sector.East == null)
                    {
                        wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) + cellsize, y * (cellsize + 1) - 1));
                    }
                    if (sector.North == null || sector.West == null)
                    {
                        wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) - 1, y * (cellsize + 1) - 1));
                    }
                    if (sector.South == null || sector.West == null)
                    {
                        wallCoords.Add(new Tuple<int, int>(x * (cellsize + 1) - 1, y * (cellsize + 1) + cellsize));
                    }
                }
            }

            foreach (var c in wallCoords)
            {
                walls.Add(new Wall(c.Item1 + 1, c.Item2 + 1));
            }

            return walls;
        }


    }
}
