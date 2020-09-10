using AssistantProcessor.Enums;
using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface IRowChangedObserver
    {
        public void OnRowAdded(RowAnalized rowAnalized);
        public void OnRowConcatenated(string rowId);
        public void OnRowDiversed(string rowId, int position);
        public void OnRowDeleted(string rowId);
        public void OnRowMovedNext(string rowId);
        public void OnRowMovedPrev(string testId);
        public void OnRowTypeChanged(string rowId, RowType rowType);
    }
}
