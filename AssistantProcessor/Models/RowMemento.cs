using System.Collections.Generic;
using AssistantProcessor.Enums;

namespace AssistantProcessor.Models
{
    public class RowMemento : ObjectMemento
    {
        public RowType RowType { get; }
        public int RowNumber { get; }
        public List<int> NativeNumbers { get; }
        public bool IncludedToAnalysis { get; }
        public string HiddenContent { get; }
        public string VisibleEditedContent { get; }
        public string TestId { get; }

        public RowMemento(RowType RowType, int RowNumber, List<int> NativeNumbers, bool IncludedToAnalysis, string HiddenContent, string VisibleEditedContent, string testId)
        {
            this.TestId = testId;
            this.HiddenContent = HiddenContent;
            this.IncludedToAnalysis = IncludedToAnalysis;
            this.NativeNumbers = NativeNumbers;
            this.VisibleEditedContent = VisibleEditedContent;
            this.RowNumber = RowNumber;
            this.RowType = RowType;
        }
    }
}
