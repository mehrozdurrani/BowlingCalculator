using CalculatorFunctions;
using FrameClass;

namespace CalculatorFunctionTest;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        /*
         Testing Strike,Spar, 4|0 thats results 38
         */


        //Test1 - Arrange

        Calculator calculator = new Calculator();
        Frames[] frames = new Frames[]
        {
          new Frames { roll1 = 10},
          new Frames {roll1=6,roll2=4},
          new Frames{roll1=4,roll2=0},
        };
        int expected = 38;


        //Test2 - Act

        int actual = calculator.CalculateScore(frames);

        //Test3 - Assert

        Assert.AreEqual(expected, actual);
    }
    [TestMethod]
    public void TestMethod2()
    {
        /*
        Testing 12 Strikes that results 300
        */

        //Test 2 - Arrange

        Calculator calculator = new Calculator();
        Frames[] frames = new Frames[]
        {
          new Frames { roll1 = 10},
          new Frames {roll1=10},
          new Frames{roll1=10},
          new Frames{roll1=10},
          new Frames{roll1=10},
          new Frames{roll1=10},
          new Frames{roll1=10},
          new Frames{roll1=10},
          new Frames{roll1=10},
          new Frames{roll1=10,roll2=10,roll3=10}
        };
        int expected = 300;


        //Test 2 - Act

        int actual = calculator.CalculateScore(frames);

        //Test 2 - Assert

        Assert.AreEqual(expected, actual);

    }
}
