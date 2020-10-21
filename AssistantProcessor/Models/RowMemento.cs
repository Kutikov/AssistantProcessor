﻿using System.Collections.Generic;
using System.Linq;
using AssistantProcessor.Enums;

namespace AssistantProcessor.Models
{
    public class RowMemento : ObjectMemento
    {
        public RowType RowType { get; }
        public List<int> NativeNumbers { get; }
        public bool IncludedToAnalysis { get; }
        public string HiddenContent { get; }
        public string VisibleEditedContent { get; }
        public string TestId { get; }

        private readonly string rowId;

        public RowMemento(RowType RowType, IEnumerable<int> NativeNumbers, bool IncludedToAnalysis, string HiddenContent, string VisibleEditedContent, string testId, string rowId)
        {
            this.TestId = testId;
            this.HiddenContent = HiddenContent;
            this.IncludedToAnalysis = IncludedToAnalysis;
            this.NativeNumbers = NativeNumbers.ToList();
            this.VisibleEditedContent = VisibleEditedContent;
            this.RowType = RowType;
            this.rowId = rowId;
        }

        public ObjectMemento FindAndRestore(CoreFile coreFile)
        {
            RowAnalized? rowAnalized = coreFile.analizedRowsList.Find(x => x.rowId == rowId);
            return rowAnalized?.RestoreState(this)!;
        }
    }
}
