using System;
using System.Collections.Generic;
using System.Text;
using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface ITestChangedObserver
    {
        public void OnTestAdded(AnalyseBlock analyseBlock);
        public void OnTestDeleted(AnalyseBlock analyseBlock);
        public void OnTestFormed(AnalyseBlock analyseBlock);
    }
}
