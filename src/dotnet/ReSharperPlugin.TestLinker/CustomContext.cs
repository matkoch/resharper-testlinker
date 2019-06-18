using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace TestLinker
{
    public sealed class CustomContext
    {
        private readonly ISolution mySolution;
        private readonly ITypeElement myTypeElement;
        private readonly ITypeConversionRule myConversionRule;
        private readonly PsiLanguageType myLanguageType;
        private readonly ITreeNode myTreeNode;

        public CustomContext(ITypeElement typeElement, ISolution solution, ITypeConversionRule conversionRule, PsiLanguageType languageType, ITreeNode treeNode)
        {
            mySolution = solution;
            myTypeElement = typeElement;
            myConversionRule = conversionRule;
            myLanguageType = languageType;
            myTreeNode = treeNode;
        }


        public IPsiServices PsiServices
        {
            get { return mySolution.GetPsiServices(); }
        }

        public ITypeConversionRule ConversionRule
        {
            get { return myConversionRule; }
        }

        public PsiLanguageType LanguageType
        {
            get { return myLanguageType; }
        }

        public ISolution Solution
        {
            get { return mySolution; }
        }

        public ITypeElement TypeElement
        {
            get { return myTypeElement; }
        }

        public ITreeNode TreeNode
        {
            get { return myTreeNode; }
        }

        public bool IsValid()
        {
            return myTypeElement.IsValid();
        }
    }
}