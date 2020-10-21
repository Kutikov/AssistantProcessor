using System.Collections.Generic;
using System.Linq;

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

        public TestMemento(IEnumerable<string> Task, IEnumerable<string> CorrectAnswers, IEnumerable<string> WrongAnswers, IEnumerable<string> Comments, IEnumerable<string> Project, int Number, bool Formed, bool Included, string testId)
        {
            this.Comments = Comments.ToList();
            this.CorrectAnswers = CorrectAnswers.ToList();
            this.Formed = Formed;
            this.Number = Number;
            this.Project = Project.ToList();
            this.WrongAnswers = WrongAnswers.ToList();
            this.Task = Task.ToList();
            this.Included = Included;
            this.testId = testId;
        }

        public ObjectMemento FindAndRestore(CoreFile coreFile)
        {
            TestAnalized? testAnalized = coreFile.analizedTestsList.Find(x => x.testId == testId);
            return testAnalized?.RestoreState(this)!;
        }
    }
}
