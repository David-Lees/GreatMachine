using GreatMachine.Helpers;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace GreatMachine.Tests
{
    public class PositionHelperTests
    {
        [Test]
        public void Convert1Dto2D()
        {
            // Arrange
            var width = 4;            
            var test1 = new Vector2(0, 0);
            var test2 = new Vector2(3, 1);
            var test3 = new Vector2(1, 1);
            var test4 = new Vector2(0, 1);

            // Act
            var result1 = PositionHelper.Convert1Dto2D(0, width);
            var result2 = PositionHelper.Convert1Dto2D(7, width);
            var result3 = PositionHelper.Convert1Dto2D(5, width);
            var result4 = PositionHelper.Convert1Dto2D(4, width);

            // Assert
            Assert.AreEqual(test1, result1);
            Assert.AreEqual(test2, result2);
            Assert.AreEqual(test3, result3);
            Assert.AreEqual(test4, result4);
        }

        [Test]
        public void Convert2Dto1D()
        {
            Assert.AreEqual(0, PositionHelper.Convert2Dto1D(0, 0, 4), "0,0");
            Assert.AreEqual(7, PositionHelper.Convert2Dto1D(3, 1, 4), "3,1");
            Assert.AreEqual(5, PositionHelper.Convert2Dto1D(1, 1, 4), "1,1");
            Assert.AreEqual(4, PositionHelper.Convert2Dto1D(0, 1, 4), "0,4");
        }
    }
}