using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Occurrences;

namespace TestLinker
{
    public class CustomOccurrence : DeclaredElementOccurrence
    {
        private readonly int myParameterIndex;

        public CustomOccurrence([NotNull] ICustomEntry entry)
            : base(entry.DeclaredElement)
        {
            var parameterEntry = entry as ICustomParameterEntry;
            myParameterIndex = parameterEntry != null ? parameterEntry.ParameterIndex : -1;
        }

        public bool UseParameter
        {
            get { return myParameterIndex > -1; }
        }

        public int ParameterIndex
        {
            get { return myParameterIndex; }
        }

        public override string DumpToString()
        {
            return UseParameter
                ? base.DumpToString() + string.Format(" HIGHLIGHT PARAMETER {0}", myParameterIndex)
                : base.DumpToString();
        }
    }
}