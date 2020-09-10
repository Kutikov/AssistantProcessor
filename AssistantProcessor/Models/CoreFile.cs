using System.Collections.Generic;
using System.Linq;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;
using Newtonsoft.Json;

namespace AssistantProcessor.Models
{
    public class CoreFile : IRowChangedObserver, ITestChangedObserver, IUndoRedoObject
    {
        public string fileSource;
        public string content;
        public FilterPatterns filterPatterns;
        public List<RowNative> rowNatives;
        public ParseType ParseType;
        public List<RowAnalized> Rows;
        public List<TestAnalized> AnalyseBlocks;
        public List<string> rowsIdsOrdered;
        public List<string> testIdsOrdered;
        private ActionBlock actionBlock;

        [JsonIgnore] public Stack<ActionBlock> actionsActionBlocksPrev;
        [JsonIgnore] public Stack<ActionBlock> actionsActionBlocksNext;
        [JsonIgnore] public List<IRowChangedObserver> IRowChangedObservers;
        [JsonIgnore] public List<ITestChangedObserver> ITestChangedObservers;

        [JsonIgnore] public TestAnalized tempTest; 

        #nullable enable
        public CoreFile(string? fileSource, string? content, FilterPatterns filterPatterns, ParseType parseType)
        {
            this.content = content;
            this.fileSource = fileSource;
            this.filterPatterns = filterPatterns;
            ParseType = parseType;
            rowsIdsOrdered = new List<string>();
            testIdsOrdered = new List<string>();
            ITestChangedObservers = new List<ITestChangedObserver>();
            IRowChangedObservers = new List<IRowChangedObserver>();
            actionsActionBlocksNext = new Stack<ActionBlock>();
            actionsActionBlocksPrev = new Stack<ActionBlock>();
        }
        #nullable disable

        #region Saving
        public string Encode()
        {
            filterPatterns.Encode();
            return JsonConvert.SerializeObject(this);
        }

        public static CoreFile Decode(string source)
        {
            CoreFile coreFile = JsonConvert.DeserializeObject<CoreFile>(source);
            coreFile.ITestChangedObservers = new List<ITestChangedObserver>();
            coreFile.IRowChangedObservers = new List<IRowChangedObserver>();
            coreFile.actionsActionBlocksNext = new Stack<ActionBlock>();
            coreFile.actionsActionBlocksPrev = new Stack<ActionBlock>();
            coreFile.filterPatterns.Decode();
            return coreFile;
        }
        #endregion

        #region UndoRedo
        private void FixAction(EditorAction editorAction, ObjectMemento objectMemento)
        {
        }

        private void FinishAction()
        {
            actionsActionBlocksPrev.Push(actionBlock.Clone());
            actionBlock = null;
        }

        public bool Undo()
        {
            if (actionsActionBlocksPrev.TryPop(out ActionBlock result))
            {
                //restore
                return true;
            }
            return false;
        }

        public bool Redo()
        {
            if (actionsActionBlocksNext.TryPop(out ActionBlock result))
            {
                return true;
            }
            return false;
        }
        #endregion

        public void OnRowAdded(RowAnalized rowAnalized)
        {
            Rows.Add(rowAnalized);
            rowsIdsOrdered.Add(rowAnalized.rowId);
        }

        public void OnRowConcatenated(string rowId)
        {
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == rowId);
            int nextRow = rowsIdsOrdered.IndexOf(rowId) + 1;
            if (nextRow < rowsIdsOrdered.Count)
            {
                RowAnalized rowNext = Rows.Find(x => x.rowId == rowsIdsOrdered[nextRow]);
                if (rowAnalized != null)
                {
                    ObjectMemento o1 = rowAnalized.SaveState();
                    if (rowNext != null)
                    {
                        ObjectMemento o2 = rowNext.SaveState();
                        rowAnalized.visibleEditedContent = rowAnalized.visibleEditedContent + rowNext.hiddenContent +
                                                           rowNext.visibleEditedContent;
                        rowNext.includedToAnalysis = false;
                        actionBlock = new ActionBlock();
                        actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o1);
                        actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o2);
                        FinishAction();
                    }
                }
            }
        }

        public void OnRowDiversed(string rowId)
        {
            FixAction(EditorAction.ROW_DIVERSED, null);
        }

        public void OnRowDeleted(string rowId)
        {
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == rowId);
            if (rowAnalized != null)
            {
                ObjectMemento o1 = SaveState();
                ObjectMemento o2 = rowAnalized.SaveState();
                rowAnalized.includedToAnalysis = false;
                actionBlock = new ActionBlock();
                actionBlock.AddAction(EditorAction.ROW_DELETED, o1);
                actionBlock.AddAction(EditorAction.ROW_DELETED, o2);
                FinishAction();
            }
        }

        public void OnRowMovedNext(string rowId)
        {
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == rowId);
            if (rowAnalized != null)
            {
                ObjectMemento o1 = SaveState();
                ObjectMemento o2 = rowAnalized.SaveState();
                int nextTest = testIdsOrdered.IndexOf(rowAnalized.testId) + 1;
                TestAnalized test = AnalyseBlocks.FirstOrDefault(x => x.taskId == testIdsOrdered[nextTest - 1]);
                if (test != null)
                {
                    ObjectMemento o3 = test.SaveState();
                    test.comments.Remove(rowId);
                    test.correctAnswers.Remove(rowId);
                    test.wrongAnswers.Remove(rowId);
                    test.task.Remove(rowId);
                    test.project.Remove(rowId);
                    rowAnalized.testId = testIdsOrdered[nextTest];
                    actionBlock = new ActionBlock();
                    actionBlock.AddAction(EditorAction.ROW_MOVED_NEXT, o1);
                    actionBlock.AddAction(EditorAction.ROW_MOVED_NEXT, o2);
                    actionBlock.AddAction(EditorAction.ROW_MOVED_NEXT, o3);
                    FinishAction();
                }
                
            }
        }

        public void OnRowMovedPrev(string rowId)
        {
            FixAction(EditorAction.ROW_MOVED_PREV, null);
        }

        public void OnRowTypeChanged(string rowId, RowType rowType)
        {
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == rowId);
            if (rowAnalized != null)
            {
                ObjectMemento o2 = rowAnalized.SaveState();
                int thisTest = testIdsOrdered.IndexOf(rowAnalized.testId);
                TestAnalized test = AnalyseBlocks.FirstOrDefault(x => x.taskId == testIdsOrdered[thisTest]);
                if (test != null)
                {
                    ObjectMemento o3 = test.SaveState();
                    test.comments.Remove(rowId);
                    test.correctAnswers.Remove(rowId);
                    test.wrongAnswers.Remove(rowId);
                    test.task.Remove(rowId);
                    switch (rowType)
                    {
                        case RowType.COMMENT:
                            test.comments.Add(rowId);
                            break;
                        case RowType.CORRECT_ANSWER:
                            test.correctAnswers.Add(rowId);
                            break;
                        case RowType.TASK:
                            test.wrongAnswers.Add(rowId);
                            break;
                        case RowType.WRONG_ANSWER:
                            test.task.Add(rowId);
                            break;
                    }
                    actionBlock = new ActionBlock();
                    actionBlock.AddAction(EditorAction.ROW_TYPE_CHAHGED, o2);
                    actionBlock.AddAction(EditorAction.ROW_TYPE_CHAHGED, o3);
                    FinishAction();
                }
            }
        }

        public void OnTestAdded(TestAnalized test)
        {
            AnalyseBlocks.Add(test);
            testIdsOrdered.Add(test.taskId);
        }

        public void OnTestDeleted(string testId)
        {
            FixAction(EditorAction.TEST_DELETED, null);
            
        }

        public void OnTestFormed(string testId)
        {
            TestAnalized testAnalized = AnalyseBlocks.Find(x => x.taskId == testId);
            if (testAnalized != null)
            {
                actionBlock = new ActionBlock();
                ObjectMemento o1 = testAnalized.SaveState();
                testAnalized.formed = true;
                actionBlock.AddAction(EditorAction.TEST_FORMED, o1);
                FinishAction();
            }
        }

        #region Memento
        public ObjectMemento SaveState()
        {
            return new IdsRowMemento(rowsIdsOrdered, testIdsOrdered);
        }

        public void RestoreState(ObjectMemento objectMemento)
        {
            rowsIdsOrdered = ((IdsRowMemento) objectMemento).IdsRList;
            testIdsOrdered = ((IdsRowMemento) objectMemento).IdsTList;
        }
        #endregion
    }
}
