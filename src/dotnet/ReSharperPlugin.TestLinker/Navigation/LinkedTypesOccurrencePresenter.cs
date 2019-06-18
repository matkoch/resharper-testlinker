using System.Drawing;
using JetBrains.Application.UI.Controls.JetPopupMenu;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.TestLinker.Navigation
{
    [OccurrencePresenter]
    public class LinkedTypesOccurrencePresenter : DeclaredElementOccurrencePresenter
    {
        public override bool IsApplicable(IOccurrence occurrence)
        {
            return occurrence is LinkedTypesOccurrence;
        }

        protected override void DisplayMainText(IMenuItemDescriptor descriptor, IOccurrence occurrence, OccurrencePresentationOptions options,
            IDeclaredElement declaredElement)
        {
            base.DisplayMainText(descriptor, occurrence, options, declaredElement);
            if (occurrence is LinkedTypesOccurrence linkedTypeOccurrence && linkedTypeOccurrence.HasNameDerived)
                descriptor.Text.SetStyle(FontStyle.Bold);
        }
    }
}