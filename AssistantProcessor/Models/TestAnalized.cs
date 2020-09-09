using System.Collections.Generic;
using AssistantProcessor.Interfaces;

namespace AssistantProcessor.Models
{
    public class TestAnalized : IUndoRedoObject
    {
        public string taskId; //static
        public List<string> task;
        public List<string> correctAnswers;
        public List<string> wrongAnswers;
        public List<string> comments;
        public List<string> project;
        public int number;
        public bool formed;

        public ObjectMemento SaveState()
        {
            return new TestMemento(task, correctAnswers, wrongAnswers, comments, project, number, formed);
        }

        public void RestoreState(ObjectMemento objectMemento)
        {
            TestMemento testMemento = (TestMemento) objectMemento;
            task = testMemento.Task;
            correctAnswers = testMemento.CorrectAnswers;
            wrongAnswers = testMemento.WrongAnswers;
            comments = testMemento.Comments;
            project = testMemento.Project;
            number = testMemento.Number;
            formed = testMemento.Formed;
        }
    }
}
