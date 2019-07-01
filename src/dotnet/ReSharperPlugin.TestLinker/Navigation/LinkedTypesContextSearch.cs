using System.Linq;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;
using ReSharperPlugin.TestLinker.Utils;

namespace ReSharperPlugin.TestLinker.Navigation
{
    [ShellFeaturePart]
    public class LinkedTypesContextSearch : IContextSearch
    {
        public bool IsAvailable(IDataContext dataContext)
        {
            return true;
        }

        public bool IsContextApplicable(IDataContext dataContext)
        {
            return ContextNavigationUtil.CheckDefaultApplicability<CSharpLanguage>(dataContext);
        }

        public LinkedTypesSearchRequest CreateSearchRequest(IDataContext dataContext)
        {
            var typesFromTextControlService = dataContext.GetComponent<ITypesFromTextControlService>().NotNull();
            var textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL);
            var solution = dataContext.GetData(ProjectModelDataConstants.SOLUTION);

            var declaredElements = dataContext.GetData(PsiDataConstants.DECLARED_ELEMENTS_FROM_ALL_CONTEXTS);
            var type = declaredElements?.SingleOrDefault() as ITypeElement
                       ?? typesFromTextControlService.GetTypesFromCaretOrFile(textControl.NotNull(), solution.NotNull()).SingleOrDefault();

            return type != null ? new LinkedTypesSearchRequest(type) : null;
        }
    }
}