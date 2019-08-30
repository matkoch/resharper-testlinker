
using JetBrains.Annotations;
using JetBrains.Application.PersistentMap;
using JetBrains.ReSharper.Daemon.CSharp.CallGraph;
using JetBrains.Serialization;
#if RIDER
using System.Collections.Generic;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;
using JetBrains.UI.Icons;
using System.Linq;
using JetBrains.ReSharper.Daemon.CodeInsights;
using JetBrains.ReSharper.Daemon.UsageChecking.SwaExtension;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Host.Features.CodeInsights.Providers;
using JetBrains.ReSharper.Host.Platform.Icons;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using JetBrains.Util;
using ReSharperPlugin.TestLinker.Actions;
using ReSharperPlugin.TestLinker.Navigation;
using ReSharperPlugin.TestLinker.Utils;

namespace ReSharperPlugin.TestLinker.Daemon
{
    public class LinkedTypesContextNavigationCodeInsightsProvider : ContextNavigationCodeInsightsProviderBase<GotoLinkedTypesAction, LinkedTypesNavigationProvider>
    {
        public LinkedTypesContextNavigationCodeInsightsProvider(Shell shell) 
            : base(shell)
        {
        }

        protected override string Noun(IDeclaredElement element, int count)
        {
            return "linked " + NounUtil.ToPluralOrSingular("type", count);
        }

        protected override int GetBaseCount(
            SolutionAnalysisService swa,
            IGlobalUsageChecker usageChecker,
            IDeclaredElement element,
            ElementId? elementId)
        {
            throw new System.NotImplementedException();
        }

        protected override int GetOwnCount(
            SolutionAnalysisService swa,
            IGlobalUsageChecker usageChecker,
            IDeclaredElement element,
            ElementId? elementId)
        {
            return 0;
        }

        public override string ProviderId => nameof(LinkedTypesContextNavigationCodeInsightsProvider);
        
        protected override IconId IconId => UnitTestingThemedIcons.TestFixtureToolWindow.Id;

        public override ICollection<CodeLensRelativeOrdering> RelativeOrderings =>
            new[] {new CodeLensRelativeOrderingFirst()};
    }
    
    [PolymorphicMarshaller()]
    public class LinkedTypesDataElement : ISwaExtensionData, ISwaExtensionInfo
    {
        [UsedImplicitly]
        public static UnsafeReader.ReadDelegate<object> ReadDelegate = CallGraphInfo.Read;
        
        [UsedImplicitly]
        public static UnsafeWriter.WriteDelegate<object> WriteDelegate = (w, o) => CallGraphInfo.Write(w, o as CallGraphInfo);

        public ISwaExtensionInfo ToInfo(CollectUsagesStagePersistentData persistentData)
        {
            return this;
        }

        public void AddData(ISwaExtensionData data)
        {
        }

        public void ProcessBeforeInterior(ITreeNode element, IParameters parameters)
        {
        }

        public void ProcessAfterInterior(ITreeNode element, IParameters parameters)
        {
        }

        public void ProcessNode(ITreeNode element, IParameters parameters)
        {
        }
    }
    
    public class LinkedTypesSwaExtensionProvider : SwaExtensionProviderBase
    {
        public LinkedTypesSwaExtensionProvider(string name, bool isEnabled = true) : base(name, isEnabled)
        {
        }

        public override ISwaExtensionData CreateUsageDataElement(UsageData owner)
        {
            throw new System.NotImplementedException();
        }

        public override void Merge(ISwaExtensionInfo oldData, ISwaExtensionInfo newData)
        {
            throw new System.NotImplementedException();
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }
    }

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
#endif