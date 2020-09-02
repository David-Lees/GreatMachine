//using GreatMachine.Models;
//using NUnit.Framework;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using GreatMachine.Helpers;
//using Moq;

//namespace GreatMachine.Tests
//{
//    public class CollisionTests
//    {

//        [Test]
//        public void Overlaps_CircleOnCircle_NotOverlapping()
//        {
//            // Arrange
//            var texture = new Mock<Texture2D>(null, null, null);

//            var circle1 = new Player()
//            {
//                Position = new Vector2(20, 20),
//                SpriteSheet = new SpriteSheet(32, 32)
//            };

//            var circle2 = new Player()
//            {
//                Position = new Vector2(160, 160),
//                SpriteSheet = new SpriteSheet(32, 32)
//            };

//            // Act
//            var result = CollisionHelper.Overlaps(circle1, circle2);

//            // Assert
//            Assert.IsFalse(result);
//        }

//        [Test]
//        public void Overlaps_CircleOnCircle_Overlapping()
//        {
//            // Arrange
//            var circle1 = new Player()
//            {
//                Position = new Vector2(20, 20),
//                SpriteSheet = new SpriteSheet(32, 32)
//            };

//            var circle2 = new Player()
//            {
//                Position = new Vector2(40, 20),
//                SpriteSheet = new SpriteSheet(32, 32)
//            };

//            // Act
//            var result = CollisionHelper.Overlaps(circle1, circle2);

//            // Assert
//            Assert.IsTrue(result);
//        }

//        [Test]
//        public void Overlaps_CircleOnRectangle_NotOverlapping()
//        {
//            // Arrange
//            var circle = new Player()
//            {
//                Position = new Vector2(20, 20),
//                SpriteSheet = new SpriteSheet(32, 32)
//            };

//            var rectangle = new Wall(400, 20)
//            {
//                SpriteSheet = new SpriteSheet(64, 64)
//            };

//            // Act
//            var result = CollisionHelper.Overlaps(circle, rectangle);

//            // Assert
//            Assert.IsFalse(result);
//        }

//        [Test]
//        public void Overlaps_CircleOnRectangle_Overlapping()
//        {
//            // Arrange
//            var circle = new Player()
//            {
//                Position = new Vector2(20, 20),
//                SpriteSheet = new SpriteSheet(32, 32)
//            };

//            var rectangle = new Wall(30, 20)
//            {
//                SpriteSheet = new SpriteSheet(64, 64)
//            };

//            // Act
//            var result = CollisionHelper.Overlaps(circle, rectangle);

//            // Assert
//            Assert.IsTrue(result);
//        }

//        [Test]
//        public void CalculateRebounds_Works_Case1()
//        {
//            // Arrange
//            var ball = new MoveableEntity
//            {
//                Position = new Vector2(1, 1),
//                Velocity = new Vector2(3, 3),
//                SpriteSheet = new SpriteSheet(0, 0)
//            };

//            var wall = new Wall(0, 2) { SpriteSheet = new SpriteSheet(10, 10) };

//            // Act
//            var destination = CollisionHelper.CalculateRebounds(ball, new Wall[] { wall });

//            // Assert
//            Assert.AreEqual(new Vector2(4, 0), destination);
//        }

//        [Test]
//        public void CalculateRebounds_Works_Case2()
//        {
//            // Arrange
//            var ball = new MoveableEntity
//            {
//                Position = new Vector2(10, 10),
//                Velocity = new Vector2(30, 30),
//                SpriteSheet = new SpriteSheet(5, 5)
//            };
            
//            var wall = new Wall(0, 20) { SpriteSheet = new SpriteSheet(100, 100) };
//            var wall2 = new Wall(100, 20) { SpriteSheet = new SpriteSheet(100, 100) };
//            var wall3 = new Wall(200, 20) { SpriteSheet = new SpriteSheet(100, 100) };

//            // Act
//            var destination = CollisionHelper.CalculateRebounds(ball, new Wall[] { wall, wall2, wall3 });

//            // Assert
//            Assert.AreEqual(new Vector2(40, 0), destination);
//        }

//        [Test]
//        public void CalculateRebounds_Works_Case3()
//        {
//            // Arrange
//            var ball = new MoveableEntity
//            {
//                Position = new Vector2(10, 10),
//                Velocity = new Vector2(30, 30),
//                SpriteSheet = new SpriteSheet(5, 5)
//            };

//            var wall = new Wall(0, 20) { SpriteSheet = new SpriteSheet(100, 2) };

//            // Act
//            var destination = CollisionHelper.CalculateRebounds(ball, new Wall[] { wall });

//            // Assert
//            Assert.AreEqual(new Vector2(40, 0), destination);
//        }

//        [Test]
//        public void CalculateRebounds_Works_Case4()
//        {
//            // Arrange
//            var ball = new MoveableEntity
//            {
//                Position = new Vector2(10, 10),
//                Velocity = new Vector2(30, 30),
//                SpriteSheet = new SpriteSheet(5, 5)
//            };

//            var wall = new Wall(20, 0) { SpriteSheet = new SpriteSheet(2, 100) };

//            // Act
//            var destination = CollisionHelper.CalculateRebounds(ball, new Wall[] { wall });

//            // Assert
//            Assert.AreEqual(new Vector2(0, 40), destination);
//        }

//        [Test]
//        public void CalculateRebounds_Works_Case5()
//        {
//            // Arrange
//            var ball = new MoveableEntity
//            {
//                Position = new Vector2(80, 10),
//                Velocity = new Vector2(80, 80),
//                SpriteSheet = new SpriteSheet(5, 5)
//            };

//            var wall = new Wall(100, 0) { SpriteSheet = new SpriteSheet(100, 100) };
//            var wall2 = new Wall(0, 100) { SpriteSheet = new SpriteSheet(100, 100) };            

//            // Act
//            var destination = CollisionHelper.CalculateRebounds(ball, new Wall[] { wall, wall2 });

//            // Assert
//            Assert.AreEqual(new Vector2(0, 70), destination);
//        }

//        [Test]
//        public void LinesIntersectAt_returns_intersection()
//        {
//            // Arrange
//            var A = new Vector2(0, 0);
//            var B = new Vector2(16, 8);
//            var C = new Vector2(10, 10);
//            var D = new Vector2(15, 0);

//            // Act
//            var result = CollisionHelper.LinesIntersectAt(A, B, C, D);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(new Vector2(12, 6), result);
//        }
//    }
//}
