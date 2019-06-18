using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using TestLinker.Navigation;

namespace TestLinker
{
    public sealed class CustomSearchRequest : SearchRequest
    {
        private readonly LinkedTypesService _service;
        private readonly CustomContext myContext;
        private readonly DeclaredElementEnvoy<IDeclaredElement> myTarget;
        private readonly string myTitle;

        public CustomSearchRequest(LinkedTypesService service, CustomContext context)
        {
            _service = service;
            myContext = context;
            myTarget = new DeclaredElementEnvoy<IDeclaredElement>(context.TypeElement);
            myTitle = String.Format("Exposing APIs for {0}", context.TypeElement.Type()?.GetPresentableName(context.LanguageType) ?? "<null>");
        }

        public override string Title
        {
            get { return myTitle; }
        }

        public override ISolution Solution
        {
            get { return myContext.Solution; }
        }

        public override ICollection SearchTargets
        {
            get { return new IDeclaredElementEnvoy[] {myTarget}; }
        }

        public CustomContext Context
        {
            get { return myContext; }
        }

        public override ICollection<IOccurrence> Search(IProgressIndicator progressIndicator)
        {
            if (!myContext.IsValid())
                return EmptyList<IOccurrence>.InstanceList;

            var declaredElement = myTarget.GetValidDeclaredElement();
            if (declaredElement == null)
                return EmptyList<IOccurrence>.InstanceList;

            return _service
                .GetLinkedTypesEnumerable(new[]{Context.TypeElement})
                .Select(entry => new LinkedTypesOccurrence(entry, OccurrenceType.Occurrence))
                .ToArray();
        }
    }
}