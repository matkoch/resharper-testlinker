using System;
using System.Linq;
using JetBrains.Annotations;

namespace TestLinker.Test
{
  [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
  public class SubjectAttribute : Attribute
  {
    public SubjectAttribute (params Type[] subjectTypes)
    {
    }
  }
}