using System.Linq;
using JetBrains.ReSharper.Daemon.CodeInsights;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Host.Platform.Icons;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using ReSharperPlugin.TestLinker.Navigation;
using ReSharperPlugin.TestLinker.Utils;

namespace ReSharperPlugin.TestLinker
{
    [ElementProblemAnalyzer(typeof(IClassLikeDeclaration))]
    public class CodeInsightsHighlighter : ElementProblemAnalyzer<IClassLikeDeclaration>
    {
        private readonly CustomTypesCodeInsightsProvider _provider;
        private readonly IconHost _iconHost;

        public CodeInsightsHighlighter(
            CustomTypesCodeInsightsProvider provider,
            IconHost iconHost)
        {
            _provider = provider;
            _iconHost = iconHost;
        }

        protected override void Run(IClassLikeDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.DeclaredElement == null)
                return;

            var linkedTypes = LinkedTypesUtil.GetLinkedTypes(element.DeclaredElement).ToList();
            if (linkedTypes.Count == 0)
                return;

            consumer.AddHighlighting(
                new CodeInsightsHighlighting(
                    element.GetNameDocumentRange(),
                    linkedTypes.Count + " linked types7",
                    "Cognitive complexity value of ",
                    _provider,
                    element.DeclaredElement,
                    _iconHost.Transform(UnitTestingThemedIcons.TestFixtureToolWindow.Id))
            );
        }
    }
//    [SolutionComponent]
//    public class TestLinkerCodeInsightsProvider : ICodeInsightsProvider
//    {
//        private readonly LinkedTypesService _linkedTypesService;
//        private readonly IMainWindowPopupWindowContext _mainWindowPopupWindowContext;
//
//        public TestLinkerCodeInsightsProvider(
//            LinkedTypesService linkedTypesService,
//            MainWindowPopupWindowContext mainWindowPopupWindowContext)
//        {
//            _linkedTypesService = linkedTypesService;
//            _mainWindowPopupWindowContext = mainWindowPopupWindowContext;
//        }
//
//        public void OnClick(CodeInsightsHighlighting highlighting)
//        {
//            var typeElement = highlighting.DeclaredElement as ITypeElement;
//            var declaration = typeElement?.GetFirstDeclaration<IClassLikeDeclaration>();
//            if (declaration == null)
//                return;
//
//            var linkedTypes = _linkedTypesService.GetLinkedTypes(typeElement);
//
//            GotoLinkedTypesActionBase.ExecuteProlonged(
//                declaration.GetSolution(),
//                new []{typeElement}.ToSet(),
//                linkedTypes.ToList(),
//                textControl: null,
//                _mainWindowPopupWindowContext.Source);
//        }
//
//        public void OnExtraActionClick(CodeInsightsHighlighting highlighting, string actionId)
//        {
//        }
//
//        public string ProviderId => nameof(TestLinkerCodeInsightsProvider);
//        public string DisplayName => "TestLinker";
//        public CodeLensAnchorKind DefaultAnchor => CodeLensAnchorKind.Top;
//
//        public ICollection<CodeLensRelativeOrdering> RelativeOrderings => new CodeLensRelativeOrdering[]
//            {new CodeLensRelativeOrderingFirst()};
//    }
}