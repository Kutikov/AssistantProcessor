using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;

namespace AssistantProcessor.Models
{
    public class RowAnalized : IUndoRedoObject
    {
        public RowType rowType;
        public string rowId; //static
        public List<int> nativeNumbers;
        public bool includedToAnalysis;
        public string content; //static
        public string hiddenContent;
        public string visibleEditedContent;
        public string testId;

        public RowAnalized(RowNative rowNative, FilterPatterns filterPatterns, CoreFile coreFile)
        {
            bool analized = false;
            content = rowNative.content;
            nativeNumbers = new List<int>{ rowNative.rowNumber };
            rowId = "row" + rowNative.rowNumber;
            includedToAnalysis = true;
            if (content.Trim() == "")
            {
                includedToAnalysis = false;
                if (filterPatterns.tasksDividedByNewRow)
                {
                    coreFile.OnNextTestDetected();
                }
            }
            else
            {
                int i = 0;
                while (!analized && i < filterPatterns.correctAnswerRegexes.Count)
                {
                    if (filterPatterns.correctAnswerRegexes[i].IsMatch(content))
                    {
                        analized = true;
                        MatchCollection match = filterPatterns.correctAnswerRegexes[i].Matches(content);
                        hiddenContent = match[0].Value;
                        visibleEditedContent = content.Replace(hiddenContent, "").Trim();
                        testId = coreFile.tempTest.testId;
                        coreFile.tempTest.project.Add(rowId);
                        coreFile.tempTest.correctAnswers.Add(rowId);
                        rowType = RowType.CORRECT_ANSWER;
                    }
                    i++;
                }
                i = 0;
                while (!analized && i < filterPatterns.wrongAnswerRegexes.Count)
                {
                    if (filterPatterns.wrongAnswerRegexes[i].IsMatch(content))
                    {
                        analized = true;
                        MatchCollection match = filterPatterns.wrongAnswerRegexes[i].Matches(content);
                        hiddenContent = match[0].Value;
                        visibleEditedContent = content.Replace(hiddenContent, "").Trim();
                        testId = coreFile.tempTest.testId;
                        coreFile.tempTest.project.Add(rowId);
                        coreFile.tempTest.wrongAnswers.Add(rowId);
                        rowType = RowType.WRONG_ANSWER;
                    }
                    i++;
                }
                if (coreFile.tempTest.correctAnswers.Count == 0)
                {
                    if (filterPatterns.taskRegexes[0].IsMatch(content))
                    {
                        MatchCollection match = filterPatterns.taskRegexes[0].Matches(content);
                        hiddenContent = match[0].Value;
                        visibleEditedContent = content.Replace(hiddenContent, "").Trim();
                    }
                    else
                    {
                        visibleEditedContent = content.Trim();
                        hiddenContent = "";
                    }
                    testId = coreFile.tempTest.testId;
                    coreFile.tempTest.project.Add(rowId);
                    coreFile.tempTest.task.Add(rowId);
                    rowType = RowType.TASK;
                }
                else
                {
                    if (!analized && !filterPatterns.tasksDividedByNewRow && coreFile.tempTest.wrongAnswers.Count > 0)
                    {
                        i = 0;
                        while (!analized && i < filterPatterns.taskRegexes.Count)
                        {
                            if (filterPatterns.taskRegexes[i].IsMatch(content))
                            {
                                coreFile.OnNextTestDetected();
                                MatchCollection match = filterPatterns.taskRegexes[i].Matches(content);
                                hiddenContent = match[0].Value;
                                visibleEditedContent = content.Replace(hiddenContent, "").Trim();
                                testId = coreFile.tempTest.testId;
                                coreFile.tempTest.project.Add(rowId);
                                coreFile.tempTest.task.Add(rowId);
                                rowType = RowType.TASK;
                                return;
                            }
                            i++;
                        }
                    }
                    if ((!analized || coreFile.tempTest.wrongAnswers.Count == 0) && rowType != RowType.CORRECT_ANSWER)
                    {
                        hiddenContent = "";
                        visibleEditedContent = content.Trim();
                        rowType = RowType.WRONG_ANSWER;
                        coreFile.tempTest.project.Add(rowId);
                        coreFile.tempTest.wrongAnswers.Add(rowId);
                        testId = coreFile.tempTest.testId;
                    }
                }
            }
        }

        public RowAnalized(TestAnalized testAnalized)
        {
            content = "Comment";
            nativeNumbers = new List<int>( );
            rowId = "row" + testAnalized.testId + new Random().Next(1000);
            includedToAnalysis = true;
            hiddenContent = "";
            visibleEditedContent = "Comment";
            rowType = RowType.COMMENT;
            testId = testAnalized.testId;
        }

        public RowAnalized(RowAnalized rowAnalized, int position, CoreFile coreFile)
        {
            List<string> alphabet = new List<string> {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            int z = 0;
            string rowIdtemp = rowAnalized.rowId;
            while (coreFile.rowsIdsOrdered.Contains(rowIdtemp + alphabet[z]))
            {
                z++;
            }
            rowId = rowIdtemp + alphabet[z];
            hiddenContent = "";
            visibleEditedContent = "";
            int index = coreFile.rowsIdsOrdered!.FindIndex(x => x == rowAnalized.rowId);
            if (index < coreFile.rowsIdsOrdered!.Count - 1)
            {
                coreFile.rowsIdsOrdered!.Insert(index + 1, rowId);
            }
            else
            {
                coreFile.rowsIdsOrdered!.Add(rowId);
            }
            content = rowAnalized.visibleEditedContent.Substring(position);
            includedToAnalysis = true;
            nativeNumbers = new List<int>();
            testId = rowAnalized.testId;
            bool analized = false;
            switch (rowAnalized.rowType)
            {
                case RowType.COMMENT:
                    hiddenContent = "";
                    visibleEditedContent = content.Trim();
                    rowType = RowType.COMMENT;
                    break;
                case RowType.CORRECT_ANSWER:
                case RowType.WRONG_ANSWER:
                case RowType.TASK:
                    int i = 0;
                    while (!analized && i < coreFile.filterPatterns!.correctAnswerRegexes.Count)
                    {
                        if (coreFile.filterPatterns!.correctAnswerRegexes[i].IsMatch(content))
                        {
                            analized = true;
                            MatchCollection match = coreFile.filterPatterns!.correctAnswerRegexes[i].Matches(content);
                            hiddenContent = match[0].Value;
                            visibleEditedContent = content.Replace(hiddenContent, "").Trim();
                            rowType = RowType.CORRECT_ANSWER;
                        }
                        i++;
                    }
                    i = 0;
                    while (!analized && i < coreFile.filterPatterns!.wrongAnswerRegexes.Count)
                    {
                        if (coreFile.filterPatterns!.wrongAnswerRegexes[i].IsMatch(content))
                        {
                            analized = true;
                            MatchCollection match = coreFile.filterPatterns!.wrongAnswerRegexes[i].Matches(content);
                            hiddenContent = match[0].Value;
                            visibleEditedContent = content.Replace(hiddenContent, "").Trim();
                            rowType = RowType.WRONG_ANSWER;
                        }
                        i++;
                    }
                    if (!analized)
                    {
                        if (rowAnalized.rowType == RowType.TASK)
                        {
                            hiddenContent = "";
                            visibleEditedContent = content.Trim();
                            rowType = RowType.TASK;
                        }
                        else
                        {
                            hiddenContent = "";
                            visibleEditedContent = content.Trim();
                            rowType = RowType.WRONG_ANSWER;
                        }
                    }
                    break;
            }
        }

        public ObjectMemento SaveState()
        {
            return new RowMemento(rowType, nativeNumbers, includedToAnalysis, hiddenContent, visibleEditedContent, testId, rowId);
        }

        public ObjectMemento RestoreState(ObjectMemento objectMemento)
        {
            ObjectMemento objectMemento2 = new RowMemento(rowType, nativeNumbers, includedToAnalysis, hiddenContent, visibleEditedContent, testId, rowId);
            RowMemento rowMemento = (RowMemento) objectMemento;
            rowType = rowMemento.RowType;
            nativeNumbers = rowMemento.NativeNumbers;
            includedToAnalysis = rowMemento.IncludedToAnalysis;
            hiddenContent = rowMemento.HiddenContent;
            visibleEditedContent = rowMemento.VisibleEditedContent;
            testId = rowMemento.TestId;
            return objectMemento2;
        }
    }
}
