using System;
using System.Collections.Generic;
using System.Text;
using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface IRowChangedObserver
    {
        public void OnRowConcatenated(int rowNumber);
        public void OnRowDeleted(int rowNumber);
        public void OnRowMovedNext(int blockNumber);
        public void OnRowMovedPrev(int blockNumber);
        public void OnRowTypeChanged(int rowNumber, RowType rowType);
    }
}
