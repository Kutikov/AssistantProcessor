using System.Collections.Generic;
using System.Linq;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;

namespace AssistantProcessor.Models
{
    public class TestAnalized : IUndoRedoObject
    {
        public string testId; //static
        public List<string> task;
        public List<string> correctAnswers;
        public List<string> wrongAnswers;
        public List<string> comments;
        public List<string> project;
        public bool included;
        public int number;
        public bool formed;

        public TestAnalized(int number, string testId)
        {
            this.number = number;
            included = true;
            formed = false;
            this.testId = testId;
            task = new List<string>();
            correctAnswers = new List<string>();
            comments = new List<string>();
            wrongAnswers = new List<string>();
            project = new List<string>();
        }

        public List<string> OrderedConnectedIds(List<string> orderedRowIds)
        {
            List<string> ret = new List<string>();
            int lowerIndex = project.Select(rowId => orderedRowIds.FindIndex(x => x == rowId)).Concat(new[] {0}).Min();
            for (int i = lowerIndex; i < project.Count + lowerIndex; i++)
            {
                ret.Add(orderedRowIds[i]);
            }
            return ret;
        }

        public void ConnectToRow(RowAnalized newRowAnalized)
        {
            project.Add(newRowAnalized.rowId);
            switch (newRowAnalized.rowType)
            {
                case RowType.COMMENT:
                    comments.Add(newRowAnalized.rowId);
                    break;
                case RowType.WRONG_ANSWER:
                    wrongAnswers.Add(newRowAnalized.rowId);
                    break;
                case RowType.CORRECT_ANSWER:
                    correctAnswers.Add(newRowAnalized.rowId);
                    break;
                case RowType.TASK:
                    task.Add(newRowAnalized.rowId);
                    break;
            }
        }

        public void DisconnectRow(string rowId)
        {
            comments.Remove(rowId);
            correctAnswers.Remove(rowId);
            wrongAnswers.Remove(rowId);
            task.Remove(rowId);
            project.Remove(rowId);
        }

        private TestAnalized()
        {

        }

        public ObjectMemento SaveState()
        {
            return new TestMemento(task, correctAnswers, wrongAnswers, comments, project, number, formed, included);
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
            included = testMemento.Included;
        }

        public bool HasVisibleRows(List<RowAnalized> rows)
        {
            return project.Any(row => rows.Find(x => x.rowId == row).includedToAnalysis);
        }

        public TestAnalized Clone()
        {
            return new TestAnalized
            {
                comments = comments.ToList(),
                wrongAnswers = wrongAnswers.ToList(),
                task = task.ToList(),
                correctAnswers = correctAnswers.ToList(),
                project = project.ToList(),
                included = included,
                formed = formed,
                number = number,
                testId =  testId
            };
        }
    }
}
