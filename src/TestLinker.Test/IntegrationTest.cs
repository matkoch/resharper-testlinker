using System;
using System.Linq;
using NUnit.Framework;

namespace TestLinker.Test
{
  [TestFixture]
  [Subject(typeof(AdvancedCalculator), typeof(SimpleCalculator))]
  public class IntegrationTest
  {
    [Test]
    public void Test()
    {

    }
  }

  [TestFixture]
  public class UnrelatedTest
  {
    [Test]
    public void Test()
    {

    }
  }
}
