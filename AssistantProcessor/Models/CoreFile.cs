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

        public void OnNextTestDetected()
        {
            if (tempTest != null)
            {
                foreach (var iTestChangedObserver in ITestChangedObservers)
                {
                    TestAnalized t = tempTest.Clone();
                    iTestChangedObserver.OnTestAdded(t);
                }
            }
            tempTest = new TestAnalized(testIdsOrdered.Count, "test" + testIdsOrdered.Count);
        }

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
                    if (rowNext != null)
                    { 
                        TestAnalized testAnalized = AnalyseBlocks.Find(x => x.testId == rowAnalized.testId);
                        TestAnalized testAnalized2 = AnalyseBlocks.Find(x => x.testId == rowNext.testId);
                        if (testAnalized != null)
                        {
                            ObjectMemento o2 = rowNext.SaveState();
                            ObjectMemento o1 = rowAnalized.SaveState();
                            rowAnalized.visibleEditedContent = rowAnalized.visibleEditedContent + rowNext.hiddenContent +
                                                               rowNext.visibleEditedContent;
                            rowNext.includedToAnalysis = false;
                            actionBlock = new ActionBlock();
                            if (testAnalized != testAnalized2)
                            {
                                ObjectMemento o3 = testAnalized.SaveState();
                                ObjectMemento o4 = testAnalized2.SaveState();
                                testAnalized2.DisconnectRow(rowNext.rowId);
                                testAnalized.ConnectToRow(rowNext);
                                actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o3);
                                actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o4);
                            }
                            actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o1);
                            actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o2);
                            FinishAction();
                        }
                    }
                }
            }
        }

        public void OnRowDiversed(string rowId, int position)
        {
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == rowId);
            if (rowAnalized != null)
            {
                TestAnalized testAnalized = AnalyseBlocks.Find(x => x.testId == rowAnalized.testId);
                if (testAnalized != null)
                {
                    ObjectMemento o1 = rowAnalized.SaveState();
                    ObjectMemento o2 = SaveState();
                    ObjectMemento o3 = testAnalized.SaveState();
                    RowAnalized newRowAnalized = new RowAnalized(rowAnalized, position, this);
                    Rows.Add(newRowAnalized);
                    testAnalized.ConnectToRow(newRowAnalized);
                    actionBlock = new ActionBlock();
                    actionBlock.AddAction(EditorAction.ROW_DIVERSED, o1);
                    actionBlock.AddAction(EditorAction.ROW_DIVERSED, o2);
                    actionBlock.AddAction(EditorAction.ROW_DIVERSED, o3);
                    FinishAction();
                }
            }
        }

        public void OnRowDeleted(string rowId)
        {
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == rowId);
            if (rowAnalized != null)
            {
                ObjectMemento o1 = SaveState();
                ObjectMemento o2 = rowAnalized.SaveState();
                rowAnalized.includedToAnalysis = false;
                TestAnalized testAnalized = AnalyseBlocks.Find(x => x.testId == rowAnalized.testId);
                actionBlock = new ActionBlock();
                if (testAnalized != null)
                {
                    if (!testAnalized.HasVisibleRows(Rows))
                    {
                        foreach (var iTestChangedObserver in ITestChangedObservers)
                        {
                            iTestChangedObserver.OnTestDeleted(testAnalized);
                        }
                    }
                }
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
                TestAnalized test = AnalyseBlocks.FirstOrDefault(x => x.testId == testIdsOrdered[nextTest - 1]);
                if (test != null)
                {
                    ObjectMemento o3 = test.SaveState();
                    test.DisconnectRow(rowId);
                    actionBlock = new ActionBlock();
                    TestAnalized testNext = AnalyseBlocks.FirstOrDefault(x => x.testId == testIdsOrdered[nextTest]);
                    if (testNext == null)
                    {
                        testNext = new TestAnalized(testIdsOrdered.Count, "test" + testIdsOrdered.Count);
                        AnalyseBlocks.Add(testNext);
                        testIdsOrdered.Add(testNext.testId);
                    }
                    else
                    {
                        ObjectMemento o4 = testNext.SaveState();
                        actionBlock.AddAction(EditorAction.ROW_MOVED_NEXT, o4);
                    }
                    rowAnalized.testId = testIdsOrdered[nextTest];
                    testNext.ConnectToRow(rowAnalized);
                    actionBlock.AddAction(EditorAction.ROW_MOVED_NEXT, o1);
                    actionBlock.AddAction(EditorAction.ROW_MOVED_NEXT, o2);
                    actionBlock.AddAction(EditorAction.ROW_MOVED_NEXT, o3);
                    FinishAction();
                }
            }
        }

        public void OnRowMovedPrev(string testId)
        {
            TestAnalized thisTestAnalized = AnalyseBlocks.FirstOrDefault(x => x.testId == testId);
            TestAnalized nextTestAnalized = AnalyseBlocks.FirstOrDefault(x => x.testId == testIdsOrdered[testIdsOrdered.FindIndex(y => y == testId) + 1]);
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == nextTestAnalized.OrderedConnectedIds(rowsIdsOrdered)[0]);
            if (thisTestAnalized != null && nextTestAnalized != null && rowAnalized != null)
            {
                ObjectMemento o1 = rowAnalized.SaveState();
                ObjectMemento o2 = thisTestAnalized.SaveState();
                ObjectMemento o3 = nextTestAnalized.SaveState();
                nextTestAnalized.DisconnectRow(rowAnalized.rowId);
                thisTestAnalized.ConnectToRow(rowAnalized);
                rowAnalized.testId = testId;
                actionBlock = new ActionBlock();
                if (!nextTestAnalized.HasVisibleRows(Rows))
                {
                    ObjectMemento o4 = SaveState();
                    foreach (var iTestChangedObserver in ITestChangedObservers)
                    {
                        iTestChangedObserver.OnTestDeleted(nextTestAnalized);
                    }
                    actionBlock.AddAction(EditorAction.TEST_DELETED, o4);
                }
                actionBlock.AddAction(EditorAction.ROW_MOVED_PREV, o1);
                actionBlock.AddAction(EditorAction.ROW_MOVED_PREV, o2);
                actionBlock.AddAction(EditorAction.ROW_MOVED_PREV, o3);
                FinishAction();
            }
        }

        public void OnRowTypeChanged(string rowId, RowType rowType)
        {
            RowAnalized rowAnalized = Rows.Find(x => x.rowId == rowId);
            if (rowAnalized != null)
            {
                ObjectMemento o2 = rowAnalized.SaveState();
                int thisTest = testIdsOrdered.IndexOf(rowAnalized.testId);
                TestAnalized test = AnalyseBlocks.FirstOrDefault(x => x.testId == testIdsOrdered[thisTest]);
                if (test != null)
                {
                    ObjectMemento o3 = test.SaveState();
                    test.DisconnectRow(rowId);
                    test.ConnectToRow(rowAnalized);
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
            testIdsOrdered.Add(test.testId);
        }

        public void OnTestDeleted(TestAnalized test)
        {
            ObjectMemento o1 = test.SaveState();
            test.included = false;
            testIdsOrdered.Remove(test.testId);
            actionBlock.AddAction(EditorAction.TEST_DELETED, o1);
        }

        public void OnTestFormed(string testId)
        {
            TestAnalized testAnalized = AnalyseBlocks.Find(x => x.testId == testId);
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
