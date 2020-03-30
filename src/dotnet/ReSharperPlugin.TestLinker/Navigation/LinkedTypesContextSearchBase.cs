using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;

namespace ReSharperPlugin.TestLinker.Navigation
{
    public abstract class LinkedTypesContextSearchBase : DeclaredElementContextSearchBase<LinkedTypesSearchRequest>
    {
        protected override LinkedTypesSearchRequest CreateSearchRequest(IDataContext dataContext, IDeclaredElement element,
            IDeclaredElement initialTarget)
        {
            var textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL);

            return element is ClassLikeTypeElement type ? CreateSearchRequest(type, textControl) : null;
        }

        protected abstract LinkedTypesSearchRequest CreateSearchRequest(ITypeElement type, ITextControl textControl);
    }
}