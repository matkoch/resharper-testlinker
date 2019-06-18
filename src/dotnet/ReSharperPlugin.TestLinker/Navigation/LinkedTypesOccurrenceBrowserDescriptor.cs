// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.TreeModels;
using JetBrains.IDE.TreeBrowser;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace ReSharperPlugin.TestLinker.Navigation
{
    public sealed class LinkedTypesOccurrenceBrowserDescriptor : OccurrenceBrowserDescriptor
    {
        private readonly TreeSectionModel _model;

        public LinkedTypesOccurrenceBrowserDescriptor (
            ISolution solution,
            ICollection<ITypeElement> typesInContext,
            ICollection<IOccurrence> linkedTypeOccurrences,
            IProgressIndicator indicator = null)
            : base(solution)
        {
            _model = new TreeSectionModel();

            DrawElementExtensions = true;
            Title.Value =
                    $"LinkedTypesOccurrenceBrowserDescriptor: Linked types for {typesInContext.Take(count: 3).Select(x => x.GetClrName().ShortName).Join(", ")}{(typesInContext.Count > 3 ? "..." : string.Empty)}";

            using (ReadLockCookie.Create())
            {
                SetResults(linkedTypeOccurrences, indicator);
            }
        }

        public override TreeModel Model => _model;

        protected override void SetResults (ICollection<IOccurrence> items, IProgressIndicator indicator = null, bool mergeItems = true)
        {
            base.SetResults(items, indicator, mergeItems);
            RequestUpdate(UpdateKind.Structure, immediate: true);
        }
    }
}
