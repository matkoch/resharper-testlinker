using JetBrains.ReSharper.Psi;

namespace TestLinker
{
    public interface ICustomEntry
    {
        /// <summary>
        /// Declared Element provides an API
        /// </summary>
        IDeclaredElement DeclaredElement { get; }

        /// <summary>
        /// Target type
        /// </summary>
        IType Type { get; }
    }
}