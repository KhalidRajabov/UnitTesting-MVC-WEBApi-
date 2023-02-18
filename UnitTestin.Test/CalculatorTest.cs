using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.App;
using Xunit;

namespace UnitTestin.Test
{
    public class CalculatorTest
    {
        public Calculator? Calculator { get; set; }
        public Mock<ICalculatorService> myMock { get; set; }
        public CalculatorTest()
        {
            myMock= new Mock<ICalculatorService>();
            this.Calculator= new Calculator(myMock.Object);


            //without mocking. This is not suggested because it takes time to complete as normal
            //mocking is far faster than usual way
            //this.Calculator = new Calculator(new CalculatorService());
        }
        [Theory]
        [InlineData(2,5,7)]
        [InlineData(3,10,13)]
        public void Add_simpleValues_ReturnTotalValue(int a, int b, int expectedTotal) 
        {

            myMock.Setup(x=>x.Add(a, b)).Returns(expectedTotal);
            Assert.Equal(expectedTotal, Calculator.Add(a, b));

            myMock.Verify(x=>x.Add(a,b), Times.AtLeast(1));

            //P.S. both assert and verify can be used multiple times in test method
        }

        [Theory]
        [InlineData(0,5,0)]
        [InlineData(3,0,0)]
        public void Add_zeroValues_ReturnZeroValue(int a, int b, int expectedTotal) 
        {
            //creates the copy of the base method and uses the copy for test, the real service remains untouched
            myMock.Setup(x => x.Add(a, b)).Returns(expectedTotal);
            Assert.Equal(expectedTotal, Calculator.Add(a,b));
        }
        [Theory]
        [InlineData(3,5,15)]
        [InlineData(3, 2,6)]
        public void Multip_simpleValues_ReturnMultipValue(int a, int b, int expectedTotal)
        {
            //creates the copy of the base method and uses the copy for test, the real service remains untouched
            myMock.Setup(x => x.Multip(a, b)).Returns(expectedTotal);
            
            Assert.Equal(expectedTotal, Calculator.Multip(a,b));
        }


        [Theory]
        [InlineData(3, 5)]
        [InlineData(3, 2)]
        public void Multip_zeroValues_ReturnException(int a, int b)
        {
            myMock.Setup(x => x.Multip(a,b)).Throws(new Exception("a can not be zero"));
            Exception exception= Assert.Throws<Exception>(()=> Calculator.Multip(a, b));
            Assert.Equal("a can not be zero", exception.Message);
        }
    }
}
