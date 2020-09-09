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

        public TestMemento(List<string> Task, List<string> CorrectAnswers, List<string> WrongAnswers, List<string> Comments, List<string> Project, int Number, bool Formed)
        {
            this.Comments = Comments;
            this.CorrectAnswers = CorrectAnswers;
            this.Formed = Formed;
            this.Number = Number;
            this.Project = Project;
            this.WrongAnswers = WrongAnswers;
            this.Task = Task;
        }
    }
}
