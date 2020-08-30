using GreatMachine.Models;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GreatMachine.Helpers;
using Moq;

namespace GreatMachine.Tests
{
    public class CollisionTests
    {

        [Test]
        public void Overlaps_CircleOnCircle_NotOverlapping()
        {
            // Arrange
            var texture = new Mock<Texture2D>(null, null, null);

            var circle1 = new Player()
            {
                Position = new Vector2(20, 20),
                SpriteSheet = new SpriteSheet(32, 32)
            };

            var circle2 = new Player()
            {
                Position = new Vector2(160, 160),
                SpriteSheet = new SpriteSheet(32, 32)
            };

            // Act
            var result = CollisionHelper.Overlaps(circle1, circle2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Overlaps_CircleOnCircle_Overlapping()
        {
            // Arrange
            var circle1 = new Player()
            {
                Position = new Vector2(20, 20),
                SpriteSheet = new SpriteSheet(32, 32)
            };

            var circle2 = new Player()
            {
                Position = new Vector2(40, 20),
                SpriteSheet = new SpriteSheet(32, 32)
            };

            // Act
            var result = CollisionHelper.Overlaps(circle1, circle2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Overlaps_CircleOnRectangle_NotOverlapping()
        {
            // Arrange
            var circle = new Player()
            {
                Position = new Vector2(20, 20),
                SpriteSheet = new SpriteSheet(32, 32)
            };

            var rectangle = new Wall(400, 20)
            {
                SpriteSheet = new SpriteSheet(64, 64)
            };

            // Act
            var result = CollisionHelper.Overlaps(circle, rectangle);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Overlaps_CircleOnRectangle_Overlapping()
        {
            // Arrange
            var circle = new Player()
            {
                Position = new Vector2(20, 20),
                SpriteSheet = new SpriteSheet(32, 32)
            };

            var rectangle = new Wall(30, 20)
            {
                SpriteSheet = new SpriteSheet(64, 64)
            };

            // Act
            var result = CollisionHelper.Overlaps(circle, rectangle);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
