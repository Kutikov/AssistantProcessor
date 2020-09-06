using System;
using System.Collections.Generic;
using System.Text;

namespace AssistantProcessor.Models
{
    public class RowAnalized
    {
        public RowType rowType;
        public int rowNumber;
        public List<int> nativeNumbers;
        public bool includedToAnalysis;
        public string content;
        public string hiddenContent;
        public string visibleEditedContent;
        public int blockNumber;
    }
}
