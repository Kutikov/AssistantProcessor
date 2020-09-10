using AssistantProcessor.Enums;
using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface IRowChangedObserver
    {
        public void OnRowAdded(RowAnalized rowAnalized);
        public void OnRowConcatenated(string rowId);
        public void OnRowDiversed(string rowId);
        public void OnRowDeleted(string rowId);
        public void OnRowMovedNext(string rowId);
        public void OnRowMovedPrev(string rowId);
        public void OnRowTypeChanged(string rowId, RowType rowType);
    }
}
