using GreatMachine.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GreatMachine.Pathfinding
{
    public class Pathfinder
    {
        private int MapWidth;
        private int MapHeight;



        private readonly bool[] ObstacleMap;
        private readonly int[] FlowFieldZ;

        public Pathfinder(int width, int height)
        {
            MapHeight = height;
            MapWidth = width;

            ObstacleMap = new bool[width * height];
            FlowFieldZ = new int[width * height];
        }

        private int P(int x, int y) => PositionHelper.Convert2Dto1D(x, y, MapWidth);

        public void SetObstacle(int x, int y, bool isObstacle)
        {
            ObstacleMap[P(x, y)] = isObstacle;
        }

        public void CalculateWavePropogation(int startX, int startY)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (x == 0 || y == 0 || x == (MapWidth - 1) || y == (MapHeight - 1) || ObstacleMap[P(x, y)])
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
                    int x = n.Item1;
                    int y = n.Item2;
                    int d = n.Item3;

                    FlowFieldZ[P(x, y)] = d;

                    if ((x + 1) < MapWidth && FlowFieldZ[P(x + 1, y)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x + 1, y, d + 1));

                    if ((x - 1) < MapWidth && FlowFieldZ[P(x - 1, y)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x - 1, y, d + 1));


                    if ((y + 1) < MapWidth && FlowFieldZ[P(x, y + 1)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x, y + 1, d + 1));


                    if ((y - 1) < MapWidth && FlowFieldZ[P(x, y - 1)] == 0)
                        new_nodes.Add(new Tuple<int, int, int>(x, y - 1, d + 1));
                }

                nodes.Clear();
                nodes.AddRange(new_nodes);
            }
        }

    }
}
