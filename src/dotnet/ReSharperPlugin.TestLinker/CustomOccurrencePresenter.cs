using System.Drawing;
using JetBrains.Application.UI.Controls.JetPopupMenu;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Presentation;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace TestLinker
{
    [OccurrencePresenter]
    public class CustomOccurrencePresenter : DeclaredElementOccurrencePresenter
    {
        public override bool IsApplicable(IOccurrence occurrence)
        {
            return occurrence is CustomOccurrence;
        }

        protected override void DisplayMainText(IMenuItemDescriptor descriptor, IOccurrence occurrence, OccurrencePresentationOptions options,
            IDeclaredElement declaredElement)
        {
            if (options.TextDisplayStyle == TextDisplayStyle.ChainedCME)
                return;

            DeclaredElementPresenterMarking marking;
            DeclaredElementMenuItemFormatter.Format(declaredElement, EmptySubstitution.INSTANCE, descriptor, options, true, out marking);
            if (options.TextDisplayStyle == TextDisplayStyle.ContainingType)
                OccurrencePresentationUtil.PresentContainingType(descriptor, occurrence);

            var CustomOccurrence = occurrence as CustomOccurrence;
            if (CustomOccurrence != null)
                HighlightType(CustomOccurrence, descriptor.Text, marking);
        }

        public static void HighlightType(CustomOccurrence occurrence, RichText richText, DeclaredElementPresenterMarking marking)
        {
            var text = richText.Text;
            if (string.IsNullOrEmpty(text))
                return;

            var element = occurrence.DisplayElement.GetValidDeclaredElement();
            if (element == null)
                return;

            if (occurrence.UseParameter && marking.ParameterRanges != null)
            {
                if (marking.ParameterRanges.HasMoreThan(occurrence.ParameterIndex))
                {
                    HighlightType(richText, marking.ParameterRanges[occurrence.ParameterIndex].Range);
                    return;
                }

                Logger.LogError("ParameterRanges (Length = {0}) does not contain parameter at index {1}. Text: {2}",
                    marking.ParameterRanges.Length, occurrence.ParameterIndex, richText.Text);
            }

            HighlightType(richText, marking.TypeRange);
        }

        private static void HighlightType(RichText richText, TextRange range)
        {
            richText.SetStyle(FontStyle.Bold, range.StartOffset, range.Length);
            richText.SetForeColor(Color.ForestGreen, range.StartOffset, range.Length);
        }
    }
}