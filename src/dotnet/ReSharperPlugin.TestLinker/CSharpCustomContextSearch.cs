using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Conversions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;
using TestLinker.Utils;

namespace TestLinker
{
    [ShellFeaturePart]
    public class CSharpCustomContextSearch : ICustomContextSearch
    {
        public bool IsAvailable(IDataContext dataContext)
        {
            return true;
        }

        public bool IsContextApplicable(IDataContext dataContext)
        {
            return ContextNavigationUtil.CheckDefaultApplicability<CSharpLanguage>(dataContext);
        }

        public CustomSearchRequest CreateSearchRequest(IDataContext dataContext)
        {
            var selectedTreeNode = dataContext.GetSelectedTreeNode<ITreeNode>();
            var typesFromTextControlService = dataContext.GetComponent<ITypesFromTextControlService>().NotNull();
            var textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL);
            var solution = dataContext.GetData(ProjectModelDataConstants.SOLUTION);

            var declaredElements = dataContext.GetData(PsiDataConstants.DECLARED_ELEMENTS_FROM_ALL_CONTEXTS);
            var type = declaredElements?.SingleOrDefault() as ITypeElement
                       ?? typesFromTextControlService.GetTypesFromCaretOrFile(textControl.NotNull(), solution.NotNull()).SingleOrDefault();
            if (type == null)
                return null;

            return new CustomSearchRequest(type);
        }
    }
}