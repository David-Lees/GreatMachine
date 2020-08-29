using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreatMachine.Models
{
    public class BaseEntity
    {
        public Vector2 Position { get; set; }

        public int Sector { get; set; }

        public bool Invulnerable { get; set; }

        public int Health { get; set; }

        public int Lifespan { get; set; }

        public bool IsDead => Health < 0;
    }
}
