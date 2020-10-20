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

        public int CountVisible(List<string> ids, CoreFile coreFile)
        {
            return ids.Count(id => coreFile.Rows.Find(x => x.rowId == id)!.includedToAnalysis);
        }

        public string GetEncodedTest(ParseType parseType, CoreFile coreFile)
        {
            switch (parseType)
            {
                case ParseType.LINEAR:
                    if (formed)
                    {
                        string test = "?\n";
                        foreach (string task1 in task)
                        {
                            RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == task1)!;
                            if (rowAnalized.includedToAnalysis)
                            {
                                test += rowAnalized.visibleEditedContent + " ";
                            }
                        }
                        foreach (string true1 in correctAnswers)
                        {
                            RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == true1)!;
                            if (rowAnalized.includedToAnalysis)
                            {
                                test += "\n+" + rowAnalized.visibleEditedContent;
                            }
                        }
                        foreach (string false1 in wrongAnswers)
                        {
                            RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == false1)!;
                            if (rowAnalized.includedToAnalysis)
                            {
                                test += "\n-" + rowAnalized.visibleEditedContent;
                            }
                        }
                        if (CountVisible(comments, coreFile) > 0)
                        {
                            test += "\n!\n";
                            foreach (string comment1 in comments)
                            {
                                RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == comment1)!;
                                if (rowAnalized.includedToAnalysis)
                                {
                                    test += rowAnalized.visibleEditedContent + " ";
                                }
                            }
                        }
                        return test + "\n";
                    }
                    break;
                case ParseType.PREMIUM:
                    if (formed)
                    {
                        string test = "::" + number + "::\n";
                        foreach (string task1 in task)
                        {
                            RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == task1)!;
                            if (rowAnalized.includedToAnalysis)
                            {
                                test += rowAnalized.visibleEditedContent + " ";
                            }
                        }
                        test += "\n{";
                        foreach (string true1 in correctAnswers)
                        {
                            RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == true1)!;
                            if (rowAnalized.includedToAnalysis)
                            {
                                test += "\n=" + rowAnalized.visibleEditedContent;
                            }
                        }
                        foreach (string false1 in wrongAnswers)
                        {
                            RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == false1)!;
                            if (rowAnalized.includedToAnalysis)
                            {
                                test += "\n~" + rowAnalized.visibleEditedContent;
                            }
                        }
                        if (CountVisible(comments, coreFile) > 0)
                        {
                            test += "\n#\n";
                            foreach (string comment1 in comments)
                            {
                                RowAnalized rowAnalized = coreFile.Rows.Find(x => x.rowId == comment1)!;
                                if (rowAnalized.includedToAnalysis)
                                {
                                    test += rowAnalized.visibleEditedContent + " ";
                                }
                            }
                        }
                        return test + "\n}\n";
                    }

                    break;
            }
            return "";
        }

        public List<string> OrderedConnectedIds(List<string> orderedRowIds)
        {
            List<string> ret = new List<string>();
            int lowerIndex = project.Select(rowId => orderedRowIds.FindIndex(x => x == rowId)).Concat(new int[]{}).Min();
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
            testId = "";
            task = new List<string>();
            correctAnswers = new List<string>();
            comments = new List<string>();
            wrongAnswers = new List<string>();
            project = new List<string>();
        }

        public ObjectMemento SaveState()
        {
            return new TestMemento(task, correctAnswers, wrongAnswers, comments, project, number, formed, included, testId);
        }

        public ObjectMemento RestoreState(ObjectMemento objectMemento)
        {
            TestMemento testMemento2 = new TestMemento(task, correctAnswers, wrongAnswers, comments, project, number, formed, included, testId);
            TestMemento testMemento = (TestMemento) objectMemento;
            task = testMemento.Task;
            correctAnswers = testMemento.CorrectAnswers;
            wrongAnswers = testMemento.WrongAnswers;
            comments = testMemento.Comments;
            project = testMemento.Project;
            number = testMemento.Number;
            formed = testMemento.Formed;
            included = testMemento.Included;
            return testMemento2;
        }

        public bool HasVisibleRows(List<RowAnalized> rows)
        {
            return project.Any(row => rows.Find(x => x.rowId == row)!.includedToAnalysis);
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
