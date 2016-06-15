using System;
using System.Linq;

namespace Derived.Specs
{
  public class OtherSubjectsAttribute : Attribute
  {
    public OtherSubjectsAttribute (params Type[] subjectTypes)
    {
    }
  }
}