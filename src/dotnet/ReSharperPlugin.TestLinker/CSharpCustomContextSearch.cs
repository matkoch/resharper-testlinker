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
            var node = dataContext.GetSelectedTreeNode<ITreeNode>();
            if (node == null)
                return null;

            var typesFromTextControlService = dataContext.GetComponent<ITypesFromTextControlService>().NotNull();
            var textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL).NotNull();
            var solution = dataContext.GetData(ProjectModelDataConstants.SOLUTION).NotNull();

            var type = typesFromTextControlService.GetTypesFromCaretOrFile(textControl, solution).SingleOrDefault();
            if (type == null)
                return null;

            var context = new CustomContext(type, solution, node.GetTypeConversionRule(), node.Language, node);
            return new CustomSearchRequest(dataContext.GetComponent<LinkedTypesService>(), context);
        }

        private static bool IsTypeMember(ITreeNode node)
        {
            return node.GetContainingNode<IClassBody>() != null;
        }
    }
}