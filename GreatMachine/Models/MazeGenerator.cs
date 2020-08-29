using GreatMachine.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreatMachine.Models
{
    enum CellState
    {
        CELL_PATH_N = 0x01,
        CELL_PATH_E = 0x02,
        CELL_PATH_S = 0x04,
        CELL_PATH_W = 0x08,
        CELL_VISTED = 0x10,
    }

    public class MazeGenerator
    {
        private readonly int Width;
        private readonly int[] Maze;

        private int P(int x, int y) => PositionHelper.Convert2Dto1D(x, y, Width);

        private readonly Stack<Tuple<int, int>> stack;

        public int[] GetMaze() => Maze;

        public MazeGenerator(int width, int height)
        {
            var rand = new Random();
            Width = width;            
            Maze = new int[width * height];

            stack = new Stack<Tuple<int, int>>();

            stack.Push(new Tuple<int, int>(0, 0));
            Maze[0] = (int)CellState.CELL_VISTED;
            var VisitedCells = 1;

            while (VisitedCells < width * height)
            {
                // create a set of the unvisted neighbours
                var neighbours = new List<int>();

                var currentItem = stack.Peek();

                // North neighbour
                if (currentItem.Item2 > 0 && (Maze[P(currentItem.Item1, currentItem.Item2 - 1)] & (int)CellState.CELL_VISTED) == 0)
                {
                    neighbours.Add(0);
                }

                // East neighbour
                if (currentItem.Item1 < width - 1 && (Maze[P(currentItem.Item1 + 1, currentItem.Item2)] & (int)CellState.CELL_VISTED) == 0)
                {
                    neighbours.Add(1);
                }

                // South neighbour
                if (currentItem.Item2 < height - 1 && (Maze[P(currentItem.Item1, currentItem.Item2 + 1)] & (int)CellState.CELL_VISTED) == 0)
                {
                    neighbours.Add(2);
                }

                // West neighbour
                if (currentItem.Item1 > 0 && (Maze[P(currentItem.Item1 - 1, currentItem.Item2)] & (int)CellState.CELL_VISTED) == 0)
                {
                    neighbours.Add(3);
                }

                if (!neighbours.Any())
                {
                    // Choose available neighbour at random
                    int next_cell_dir = neighbours[rand.Next(0, neighbours.Count - 1)];

                    // Create a path between the neighbout and hte current cell
                    switch (next_cell_dir)
                    {
                        case 0:
                            Maze[P(currentItem.Item1, currentItem.Item2)] |= (int)CellState.CELL_PATH_N;
                            Maze[P(currentItem.Item1, currentItem.Item2 -1)] |= (int)CellState.CELL_PATH_S;
                            stack.Push(new Tuple<int, int>(currentItem.Item1, currentItem.Item2 - 1));
                            break;

                        case 1:
                            Maze[P(currentItem.Item1, currentItem.Item2)] |= (int)CellState.CELL_PATH_E;
                            Maze[P(currentItem.Item1 - 1, currentItem.Item2)] |= (int)CellState.CELL_PATH_W;
                            stack.Push(new Tuple<int, int>(currentItem.Item1 + 1, currentItem.Item2));
                            break;

                        case 2:
                            Maze[P(currentItem.Item1, currentItem.Item2)] |= (int)CellState.CELL_PATH_S;
                            Maze[P(currentItem.Item1, currentItem.Item2 + 1)] |= (int)CellState.CELL_PATH_N;
                            stack.Push(new Tuple<int, int>(currentItem.Item1, currentItem.Item2 + 1));
                            break;

                        case 3:
                            Maze[P(currentItem.Item1, currentItem.Item2)] |= (int)CellState.CELL_PATH_W;
                            Maze[P(currentItem.Item1 + 1, currentItem.Item2)] |= (int)CellState.CELL_PATH_E;
                            stack.Push(new Tuple<int, int>(currentItem.Item1 - 1, currentItem.Item2));
                            break;
                    }

                    VisitedCells++;
                    Maze[P(currentItem.Item1, currentItem.Item2)] |= (int)CellState.CELL_VISTED;
                }
                else
                {
                    stack.Pop(); // backtrack
                }

            }
        }
        
    }
}
