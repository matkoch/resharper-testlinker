using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Progress;

namespace TestLinker
{
    public interface ICustomFinder
    {
        [NotNull]
        IEnumerable<ICustomEntry> SearchCustom(IProgressIndicator progressIndicator, CustomContext context);
    }
}