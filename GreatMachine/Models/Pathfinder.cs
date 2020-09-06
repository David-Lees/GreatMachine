using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreatMachine.Models
{
    public class Pathfinder
    {
        private readonly int MapWidth;
        private readonly int MapHeight;

        private readonly bool[] ObstacleMap;
        private readonly int[] FlowFieldZ;

        public Pathfinder(int width, int height)
        {
            MapHeight = height;
            MapWidth = width;

            ObstacleMap = new bool[width * height];
            FlowFieldZ = new int[width * height];
        }

        private int P(int x, int y) => MathHelper.Clamp(PositionHelper.Convert2Dto1D(x, y, MapWidth), 0, MapWidth * MapHeight);

        public void SetObstacle(int x, int y, bool isObstacle)
        {
            // set obstacle
            SetObstacleInner(x, y, isObstacle);

            if (isObstacle)
            {
                // make obstacles wider so emenies don't hug walls and get stuck too much
                SetObstacleInner(x - 1, y -1, true);
                SetObstacleInner(x, y - 1, true);
                SetObstacleInner(x + 1, y - 1, true);
                SetObstacleInner(x - 1, y, true);
                SetObstacleInner(x + 1, y, true);
                SetObstacleInner(x - 1, y + 1, true);
                SetObstacleInner(x, y + 1, true);
                SetObstacleInner(x + 1, y + 1, true);
            }
        }

        private void SetObstacleInner(int x, int y, bool isObstacle)
        {
            if (x >= 0 && y >= 0 && x < MapWidth && y < MapHeight) ObstacleMap[P(x, y)] = isObstacle;
        }

        public void CalculateWavePropogation(int startX, int startY)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (ObstacleMap[P(x, y)])
                    {
                        FlowFieldZ[P(x, y)] = -1;
                    }
                    else
                    {
                        FlowFieldZ[P(x, y)] = 0;
                    }
                }
            }

            var nodes = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(startX, startY, 1)
            };

            var new_nodes = new HashSet<Tuple<int, int, int>>();

            while (nodes.Count > 0)
            {
                new_nodes.Clear();
                foreach (var n in nodes)
                {
                    int x = MathHelper.Clamp(n.Item1, 0, MapWidth);
                    int y = MathHelper.Clamp(n.Item2, 0, MapHeight);
                    int d = n.Item3;

                    FlowFieldZ[P(x, y)] = d;

                    if (x < MapWidth - 1 && (x + 1) < MapWidth && FlowFieldZ[P(x + 1, y)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x + 1, y, d + 1));

                    if (x > 0 && (x - 1) < MapWidth && FlowFieldZ[P(x - 1, y)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x - 1, y, d + 1));


                    if (y < MapHeight - 1 && (y + 1) < MapWidth && FlowFieldZ[P(x, y + 1)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x, y + 1, d + 1));


                    if (y > 0 && (y - 1) < MapWidth && FlowFieldZ[P(x, y - 1)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x, y - 1, d + 1));
                }

                nodes.Clear();
                nodes.AddRange(new_nodes);
            }
        }

        public int Stength(int x, int y)
        {
            return FlowFieldZ[P(x, y)];
        }


        public Vector2 GetVector(Vector2 position, out bool shootAttempt)
        {
            shootAttempt = false;
            var x = (int)Math.Floor(position.X / Main.Instance.SectorSize);
            var y = (int)Math.Floor(position.Y / Main.Instance.SectorSize);

            List<Tuple<int, Vector2>> possibleDirections = new List<Tuple<int, Vector2>>();

            if (x > 0 && y > 0 && x < MapWidth - 1 && y < MapHeight - 1)
            {
                AddVector(x - 1, y - 1, possibleDirections);
                AddVector(x, y - 1, possibleDirections);
                AddVector(x + 1, y - 1, possibleDirections);
                AddVector(x - 1, y + 1, possibleDirections);
                AddVector(x + 1, y + 1, possibleDirections);
                AddVector(x - 1, y + 1, possibleDirections);
                AddVector(x, y, possibleDirections);
                AddVector(x + 1, y + 1, possibleDirections);
            }

            // Filter out obstacles
            possibleDirections = possibleDirections.Where(p => p.Item1 > 0).ToList();

            if (possibleDirections.Count > 0)
            {
                var random = Main.Instance.Random.Next(50);

                shootAttempt = random < 5;

                // Head for the player
                if (random < 40)
                {
                    // Find lowest number
                    var min = possibleDirections.Min(e => e.Item1);

                    // Pick from multiple possible same lowest numbers
                    var vector = possibleDirections.Where(e => e.Item1 == min).OrderBy(e => Guid.NewGuid()).First().Item2;
                    return vector;
                }

                // head away from the player
                if (random < 45)
                {
                    // Find lowest number
                    var max = possibleDirections.Max(e => e.Item1);

                    // Pick from multiple possible same highest numbers
                    var vector = possibleDirections.Where(e => e.Item1 == max).OrderBy(e => Guid.NewGuid()).First().Item2;
                    return vector;
                }

                // pick a random direction
                return possibleDirections.OrderBy(e => Guid.NewGuid()).First().Item2;
            }
            else
            {
                // Return an entirely random vector.This may mean heading straight for a wall                 
                var vector = new Vector2(Main.Instance.Random.Next(0, 2) - 0.5f, Main.Instance.Random.Next(0, 2) - 0.5f);
                vector.Normalize();
                vector *= Main.Instance.SectorSize;
                return vector;
            }
        }

        private void AddVector(int x, int y, List<Tuple<int, Vector2>> possibleDirections)
        {
            possibleDirections.Add(new Tuple<int, Vector2>(Stength(x, y), new Vector2(
                x * Main.Instance.SectorSize + Main.Instance.SectorSize / 2,
                y * Main.Instance.SectorSize + Main.Instance.SectorSize / 2)));
        }
    }
}
