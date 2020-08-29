using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace GreatMachine.Tests
{
    public class Tests
    {
        [Test]
        public void Convert1Dto2D()
        {
            // Arrange
            var width = 4;            
            var test1 = new Vector2(0, 0);
            var test2 = new Vector2(3, 1);

            // Act
            var result1 = PositionHelper.Convert1Dto2D(0, width);
            var result2 = PositionHelper.Convert1Dto2D(6, width);

            // Assert
            Assert.AreEqual(test1, result1);
            Assert.AreEqual(test2, result2);
        }

        [Test]
        public void Convert2Dto1D()
        {
            Assert.AreEqual(5, PositionHelper.Convert2Dto1D(1, 1, 4));
            Assert.AreEqual(4, PositionHelper.Convert2Dto1D(0, 1, 4));
        }
    }
}