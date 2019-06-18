namespace TestLinker
{
    public interface ICustomParameterEntry : ICustomEntry
    {
        /// <summary>
        /// Parameter number in parameter list
        /// </summary>
        int ParameterIndex { get; }
    }
}