using NUnit.Framework;

namespace TestLinker.Test
{
  [Subject(typeof(ICalculator))]
  [TestFixture]
  public abstract class CalculatorTestBase
  {
    [Test]
    public void BaseTest()
    {
    }
  }
}