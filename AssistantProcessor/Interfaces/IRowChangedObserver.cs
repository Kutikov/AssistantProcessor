﻿using System;
using System.Collections.Generic;
using System.Text;
using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface IRowChangedObserver
    {
        public void OnRowAdded(string rowId);
        public void OnRowConcatenated(string rowId);
        public void OnRowDiversed(string rowId);
        public void OnRowDeleted(string rowId);
        public void OnRowMovedNext(string rowId);
        public void OnRowMovedPrev(string rowId);
        public void OnRowTypeChanged(string rowId, RowType rowType);
    }
}
