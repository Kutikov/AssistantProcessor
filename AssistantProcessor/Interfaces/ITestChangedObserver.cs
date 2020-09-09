using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface ITestChangedObserver
    {
        public void OnTestAdded(TestAnalized analyseBlock);
        public void OnTestDeleted(TestAnalized analyseBlock);
        public void OnTestFormed(TestAnalized analyseBlock);
    }
}
