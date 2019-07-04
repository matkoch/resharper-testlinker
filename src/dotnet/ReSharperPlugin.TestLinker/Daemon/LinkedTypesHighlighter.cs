//#if RIDER
//using System.Linq;
//using JetBrains.ReSharper.Daemon.CodeInsights;
//using JetBrains.ReSharper.Feature.Services.Daemon;
//using JetBrains.ReSharper.Host.Platform.Icons;
//using JetBrains.ReSharper.Psi.CSharp.Tree;
//using JetBrains.ReSharper.Psi.Tree;
//using JetBrains.ReSharper.UnitTestFramework;
//using JetBrains.ReSharper.UnitTestFramework.Resources;
//using JetBrains.Util;
//using ReSharperPlugin.TestLinker.Navigation;
//using ReSharperPlugin.TestLinker.Utils;
//
//namespace ReSharperPlugin.TestLinker.Daemon
//{
//    [ElementProblemAnalyzer(typeof(IClassLikeDeclaration))]
//    public class LinkedTypesHighlighter : ElementProblemAnalyzer<IClassLikeDeclaration>
//    {
//        private readonly LinkedTypesCodeInsightsProvider _provider2;
//        private readonly IconHost _iconHost;
//        private readonly IUnitTestElementStuff _unitTestElementStuff;
//
//        public LinkedTypesHighlighter(
//            LinkedTypesCodeInsightsProvider provider2,
//            IconHost iconHost,
//            IUnitTestElementStuff unitTestElementStuff)
//        {
//            _provider2 = provider2;
//            _iconHost = iconHost;
//            _unitTestElementStuff = unitTestElementStuff;
//        }
//
//        protected override void Run(IClassLikeDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
//        {
//            if (element.DeclaredElement == null ||
//                _unitTestElementStuff.GetElement(element.DeclaredElement) != null)
//                return;
//
//            var linkedTypes = LinkedTypesUtil.GetLinkedTypes(element.DeclaredElement).ToList();
//            if (linkedTypes.Count == 0)
//                return;
//
//            consumer.AddHighlighting(
//                new CodeInsightsHighlighting(
//                    element.GetNameDocumentRange(),
//                    $"{linkedTypes.Count} linked {NounUtil.ToPluralOrSingular("test", linkedTypes.Count)}",
//                    "Links between production and test code",
//                    _provider2,
//                    element.DeclaredElement,
//                    _iconHost.Transform(UnitTestingThemedIcons.TestFixtureToolWindow.Id))
//            );
//        }
//    }
////    [SolutionComponent]
////    public class TestLinkerCodeInsightsProvider : ICodeInsightsProvider
////    {
////        private readonly LinkedTypesService _linkedTypesService;
////        private readonly IMainWindowPopupWindowContext _mainWindowPopupWindowContext;
////
////        public TestLinkerCodeInsightsProvider(
////            LinkedTypesService linkedTypesService,
////            MainWindowPopupWindowContext mainWindowPopupWindowContext)
////        {
////            _linkedTypesService = linkedTypesService;
////            _mainWindowPopupWindowContext = mainWindowPopupWindowContext;
////        }
////
////        public void OnClick(CodeInsightsHighlighting highlighting)
////        {
////            var typeElement = highlighting.DeclaredElement as ITypeElement;
////            var declaration = typeElement?.GetFirstDeclaration<IClassLikeDeclaration>();
////            if (declaration == null)
////                return;
////
////            var linkedTypes = _linkedTypesService.GetLinkedTypes(typeElement);
////
////            GotoLinkedTypesActionBase.ExecuteProlonged(
////                declaration.GetSolution(),
////                new []{typeElement}.ToSet(),
////                linkedTypes.ToList(),
////                textControl: null,
////                _mainWindowPopupWindowContext.Source);
////        }
////
////        public void OnExtraActionClick(CodeInsightsHighlighting highlighting, string actionId)
////        {
////        }
////
////        public string ProviderId => nameof(TestLinkerCodeInsightsProvider);
////        public string DisplayName => "TestLinker";
////        public CodeLensAnchorKind DefaultAnchor => CodeLensAnchorKind.Top;
////
////        public ICollection<CodeLensRelativeOrdering> RelativeOrderings => new CodeLensRelativeOrdering[]
////            {new CodeLensRelativeOrderingFirst()};
////    }
//}
//#endif