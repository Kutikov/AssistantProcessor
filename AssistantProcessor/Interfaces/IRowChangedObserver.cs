using AssistantProcessor.Enums;
using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface IRowChangedObserver
    {
        public void OnRowAdded(RowAnalized rowAnalized);
        public void OnRowCreated(TestAnalized testAnalized);
        public void OnRowConcatenated(string? rowIdTop, string? rowIdBottom);
        public void OnRowDiversed(string rowId, int position);
        public void OnRowDeleted(string rowId);
        public void OnRowMovedNext(string rowId);
        public void OnRowMovedPrev(string testId);
        public void OnRowTypeChanged(string rowId, RowType rowType);
    }
}
