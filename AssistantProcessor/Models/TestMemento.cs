using System.Collections.Generic;

namespace AssistantProcessor.Models
{
    public class TestMemento : ObjectMemento
    {
        public List<string> Task { get; }
        public List<string> CorrectAnswers { get; }
        public List<string> WrongAnswers { get; }
        public List<string> Comments { get; }
        public List<string> Project { get; }
        public int Number { get; }
        public bool Formed { get; }
        public bool Included { get; }

        private readonly string testId;

        public TestMemento(List<string> Task, List<string> CorrectAnswers, List<string> WrongAnswers, List<string> Comments, List<string> Project, int Number, bool Formed, bool Included, string testId)
        {
            this.Comments = Comments;
            this.CorrectAnswers = CorrectAnswers;
            this.Formed = Formed;
            this.Number = Number;
            this.Project = Project;
            this.WrongAnswers = WrongAnswers;
            this.Task = Task;
            this.Included = Included;
            this.testId = testId;
        }

        public ObjectMemento FindAndRestore(CoreFile coreFile)
        {
            TestAnalized? testAnalized = coreFile.AnalyseBlocks.Find(x => x.testId == testId);
            return testAnalized?.RestoreState(this)!;
        }
    }
}
