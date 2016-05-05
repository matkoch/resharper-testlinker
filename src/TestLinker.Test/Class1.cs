using System;
using System.Linq;
using NUnit.Framework;

namespace TestLinker.Test
{
  [TestFixture]
  [Subject(typeof(AdvancedCalculator), typeof(SimpleCalculator))]
  public class IntegrationTest
  {
  }
}
