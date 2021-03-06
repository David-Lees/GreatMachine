﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GreatMachine.Helpers
{
    public static class PositionHelper
    {
        public static int Convert2Dto1D(int x, int y, int width)
        {
            return y * width + x;
        }

        public static int Convert2Dto1D(Vector2 vector, Vector2 size)
        {
            return (int)Math.Floor(vector.Y * size.X + vector.X);
        }

        public static Vector2 Convert1Dto2D(int offset, int width)
        {
            int x = offset % width;
            int y = offset / width;
            return new Vector2(x, y);
        }

        public static Vector2 GetSectorAsVector(Vector2 position)
        {
            if (Main.Instance != null)
            {
                var x = MathHelper.Clamp(position.X / Main.Instance.SectorSize, 0, Main.Instance.SectorCountX);
                var y = MathHelper.Clamp(position.Y / Main.Instance.SectorSize, 0, Main.Instance.SectorCountY);
                return new Vector2(x, y);
            }
            else return Vector2.Zero;
        }

        public static int GetSector(Vector2 position)
        {
            if (Main.Instance != null)
            {
                return Convert2Dto1D(
                    (int)(position.X / Main.Instance.SectorSize),
                    (int)(position.Y / Main.Instance.SectorSize),
                    Main.Instance.SectorCountX);
            }
            else return 0;
        }

        public static ICollection<int> SurroundingSectors(Vector2 position)
        {
            return new HashSet<int>
            {
                GetSector(position + new Vector2(-2,-2)),
                GetSector(position + new Vector2(-1,-2)),
                GetSector(position + new Vector2(0,-2)),
                GetSector(position + new Vector2(1,-2)),
                GetSector(position + new Vector2(2,-2)),

                GetSector(position + new Vector2(-2,-1)),
                GetSector(position + new Vector2(-1,-1)),
                GetSector(position + new Vector2(0,-1)),
                GetSector(position + new Vector2(1,-1)),
                GetSector(position + new Vector2(2,-1)),

                GetSector(position + new Vector2(-2,0)),
                GetSector(position + new Vector2(-1,0)),
                GetSector(position),
                GetSector(position + new Vector2(1,0)),
                GetSector(position + new Vector2(2,0)),

                GetSector(position + new Vector2(-2,1)),
                GetSector(position + new Vector2(-1,1)),
                GetSector(position + new Vector2(0,1)),
                GetSector(position + new Vector2(1,1)),
                GetSector(position + new Vector2(2,1)),

                GetSector(position + new Vector2(-2,2)),
                GetSector(position + new Vector2(-1,2)),
                GetSector(position + new Vector2(0,2)),
                GetSector(position + new Vector2(1,2)),
                GetSector(position + new Vector2(2,2)),
            };
        }
    }
}
