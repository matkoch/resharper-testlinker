using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation;

namespace TestLinker
{
    public interface ICustomContextSearch : IContextSearch
    {
        CustomSearchRequest CreateSearchRequest(IDataContext dataContext);
    }
}