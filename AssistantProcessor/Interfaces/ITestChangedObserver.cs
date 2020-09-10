﻿using AssistantProcessor.Models;

namespace AssistantProcessor.Interfaces
{
    public interface ITestChangedObserver
    {
        public void OnTestAdded(TestAnalized test);
        public void OnTestDeleted(string testId);
        public void OnTestFormed(string testId);
    }
}
