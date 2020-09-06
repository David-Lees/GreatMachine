using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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

            // Add some extra links so that we have some loops and multiple path choices
            for (int i = 0; i < 50; i++)
            {
                var x = Main.Instance.Random.Next(3, width - 3);
                var y = Main.Instance.Random.Next(3, height - 3);
                Maze[P(x, y)].South = P(x, y + 1);
            }
            for (int i = 0; i < 30; i++)
            {
                var x = Main.Instance.Random.Next(3, width - 3);
                var y = Main.Instance.Random.Next(3, height - 3);
                Maze[P(x, y)].East = P(x + 1, y);
            }

            // Invert links
            for (int i = 0; i < width * height; i++)
            {
                var item = Maze[i];
                if (item.North.HasValue) Maze[item.North.Value].South = i;
                if (item.South.HasValue) Maze[item.South.Value].North = i;
                if (item.East.HasValue) Maze[item.East.Value].West = i;
                if (item.West.HasValue) Maze[item.West.Value].East = i;
            }
        }

        public List<BaseEntity> GetWalls()
        {
            var cellsize = 5;
            var wallCoords = new HashSet<Tuple<int, int, SpriteSheet>>();
            var walls = new List<BaseEntity>();
                           
            wallCoords.Add(new Tuple<int, int, SpriteSheet>(-1,-1, Corner()));
            wallCoords.Add(new Tuple<int, int, SpriteSheet>(-1, -1 + (cellsize + 1) * Height, Corner()));
            for (int y = 0; y < Height; y++)
            {                
                wallCoords.Add(new Tuple<int, int, SpriteSheet>(-1, y * (cellsize + 1), Vertical()));
            }
            for (int x = 0; x < Width; x++)
            {                
                wallCoords.Add(new Tuple<int, int, SpriteSheet>(x * (cellsize + 1), -1, Horizontal()));                
                wallCoords.Add(new Tuple<int, int, SpriteSheet>(x * (cellsize + 1) + cellsize, -1, Corner()));
                for (int y = 0; y < Height; y++)
                {
                    var sector = Maze[P(x, y)];
                    if (sector.East == null)
                    {                        
                        wallCoords.Add(new Tuple<int, int, SpriteSheet>(x * (cellsize + 1) + cellsize, y * (cellsize + 1), Vertical()));
                    }
                    if (sector.South == null)
                    {                        
                        wallCoords.Add(new Tuple<int, int, SpriteSheet>(x * (cellsize +1), y * (cellsize + 1) + cellsize, Horizontal()));
                    }
                    if (sector.South == null || sector.East == null)
                    {                        
                        wallCoords.Add(new Tuple<int, int, SpriteSheet>(x * (cellsize + 1) + cellsize, y * (cellsize + 1) + cellsize, Corner()));
                    }
                    if (sector.South == null || sector.West == null)
                    {                        
                        wallCoords.Add(new Tuple<int, int, SpriteSheet>(x * (cellsize + 1) - 1, y * (cellsize + 1) - 1, Corner()));
                    }
                }
            }

            foreach (var c in wallCoords)
            {
                walls.Add(
                    new Wall(
                        (c.Item1 + 1) * Main.Instance.SectorSize + (c.Item3.SpriteWidth / 2),
                        (c.Item2 + 1) * Main.Instance.SectorSize + (c.Item3.SpriteHeight / 2), 
                        c.Item3.SpriteWidth, 
                        c.Item3.SpriteHeight, 
                        c.Item3));
            }
          
            return walls;
        }

        private static SpriteSheet Horizontal() => Main.Instance.Assets.HorizontalWallSheets.OrderBy(x => Guid.NewGuid()).First();       
        private static SpriteSheet Vertical() => Main.Instance.Assets.VerticalWallSheets.OrderBy(x => Guid.NewGuid()).First();        
        private static SpriteSheet Corner() => Main.Instance.Assets.CornerWallSheets.OrderBy(x => Guid.NewGuid()).First();
        

        public Texture2D CreateMazeTexture()
        {
            var texWidth = Width * 6 + 1;
            var texHeight = Height * 6 + 1;

            Texture2D texture = new Texture2D(Main.Instance.GraphicsDevice, texWidth, texHeight);
            Color[] data = new Color[texWidth * texHeight];

            // Set background black
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                data[pixel] = Color.Black;
            }

            var wallColour = Color.DarkOrange;
            for (int i = 0; i < texWidth; i++)
            {
                data[i] = wallColour;
                data[PositionHelper.Convert2Dto1D(i, texHeight - 1, texWidth)] = wallColour;
            }
            for (int i = 0; i < texHeight; i++)
            {
                data[PositionHelper.Convert2Dto1D(0, i, texWidth)] = wallColour;
                data[PositionHelper.Convert2Dto1D(texWidth - 1, i, texWidth)] = wallColour;
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cell = Maze[P(x, y)];
                    if (!cell.East.HasValue)
                    {
                        data[PositionHelper.Convert2Dto1D(x * 6 + 6, y * 6, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 6, y * 6 + 1, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 6, y * 6 + 2, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 6, y * 6 + 3, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 6, y * 6 + 4, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 6, y * 6 + 5, texWidth)] = wallColour;
                    }
                    if (!cell.South.HasValue)
                    {
                        data[PositionHelper.Convert2Dto1D(x * 6, y * 6 + 6, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 1, y * 6 + 6, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 2, y * 6 + 6, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 3, y * 6 + 6, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 4, y * 6 + 6, texWidth)] = wallColour;
                        data[PositionHelper.Convert2Dto1D(x * 6 + 5, y * 6 + 6, texWidth)] = wallColour;
                    }
                    if (!cell.East.HasValue || !cell.South.HasValue)
                    {
                        data[PositionHelper.Convert2Dto1D(x * 6 + 6, y * 6 + 6, texWidth)] = wallColour;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }
    }
}
