using GreatMachine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreatMachine.Models
{
    enum Directions { North, East, South, West }

    class MazeCell
    {
        public int? North { get; set; }
        public int? South { get; set; }
        public int? East { get; set; }
        public int? West { get; set; }
             
        public bool Visited { get; set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public MazeCell(int x, int y)
        {
            X = x;
            Y = y;
            East = null;
            North = null;
            West = null;
            South = null;
            Visited = false;
        }
    }

    public class MazeGenerator
    {
        private readonly int Width;
        private readonly int Height;
        private readonly MazeCell[] Maze;

        private int P(int x, int y) => PositionHelper.Convert2Dto1D(x, y, Width);

        public MazeGenerator(int width, int height)
        {
            var rand = Main.Instance.Random;
            Width = width;
            Height = height;            
            Maze = new MazeCell[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Maze[P(x, y)] = new MazeCell(x, y);
                }
            }

            var stack = new Stack<MazeCell>();
            var first = Maze[0];
            first.Visited = true;
            stack.Push(first);

            int VisitedCells = 1;

            while (stack.Count > 0)
            {
                var neighbours = new List<Directions>();
                var currentItem = stack.Peek();
                var index = Array.IndexOf(Maze, currentItem);
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
                            if (north != null) currentItem.North = Array.IndexOf(Maze, north);
                            stack.Push(north);
                            break;

                        case Directions.East:
                            if (east != null) currentItem.East = Array.IndexOf(Maze, east);
                            stack.Push(east);
                            break;

                        case Directions.South:
                            if (south != null) currentItem.South = Array.IndexOf(Maze, south);
                            stack.Push(south);
                            break;

                        case Directions.West:
                            if (west != null) currentItem.West = Array.IndexOf(Maze, west);
                            stack.Push(west);
                            break;
                    }
                    VisitedCells++;
                }
                else
                {                    
                    stack.Pop(); // backtrack
                }
                Maze[index] = currentItem;
            }

            // Invert links
            for(int i = 0; i < width * height; i++)
            {
                var item = Maze[i];
                if (item.North.HasValue) Maze[item.North.Value].South = i;
                if (item.South.HasValue) Maze[item.South.Value].North = i;
                if (item.East.HasValue) Maze[item.East.Value].West = i;
                if (item.West.HasValue) Maze[item.West.Value].East = i;
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

                    StringBuilder sb = new StringBuilder();
                    sb.Append($"{x},{y}");                    
                    if (sector.North != null)
                    {
                        var tn = Maze[sector.North.Value];
                        sb.Append($" N {tn.X},{tn.Y};");
                    }
                    if (sector.South != null)
                    {
                        var ts = Maze[sector.South.Value];
                        sb.Append($" S {ts.X},{ts.Y};");
                    }
                    if (sector.East != null)
                    {
                        var te = Maze[sector.East.Value];
                        sb.Append($" E {te.X},{te.Y};");
                    }
                    if (sector.West != null)
                    {
                        var tw = Maze[sector.West.Value];
                        sb.Append($" W {tw.X},{tw.Y};");
                    }

                    // Debug: add text
                    walls.Add(new TextEntity(x * (cellsize + 1) + 1, y * (cellsize + 1) + 1, sb.ToString()));

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
