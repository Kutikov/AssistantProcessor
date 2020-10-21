using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;
using AssistantProcessor.UI;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

#pragma warning disable 8604

namespace AssistantProcessor.Models
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class CoreFile : IRowChangedObserver, ITestChangedObserver, IUndoRedoObject
    {
        public string? fileSource;
        public string? content;
        public FilterPatterns? filterPatterns;
        public List<RowNative> rowNatives;
        public ParseType ParseType;
        public List<RowAnalized> Rows;
        public List<TestAnalized> AnalyseBlocks;
        public List<string> rowsIdsOrdered;
        public List<string> testIdsOrdered;
        private ActionBlock? actionBlock;

        [JsonIgnore] public Stack<ActionBlock> actionsActionBlocksPrev;
        [JsonIgnore] public Stack<ActionBlock> actionsActionBlocksNext;
        [JsonIgnore] public IRowChangedIterator IRowChangedObservers;
        [JsonIgnore] public List<ITestChangedObserver> ITestChangedObservers;

        [JsonIgnore] public TestAnalized? tempTest;
        [JsonIgnore] public MainWindow mainWindow;
        [JsonIgnore] private AnalizedTestUI? analizedTestUi;

        #nullable enable

        private CoreFile(string fileSource, MainWindow mainWindow, bool isFile)
        {
            this.mainWindow = mainWindow;
            if (isFile)
            {
                this.fileSource = fileSource;
            }
            else
            {
                this.content = fileSource;
            }
            rowsIdsOrdered = new List<string>();
            testIdsOrdered = new List<string>();
            rowNatives = new List<RowNative>();
            ITestChangedObservers = new List<ITestChangedObserver> {this};
            IRowChangedObservers = new IRowChangedIterator {this};
            actionsActionBlocksNext = new Stack<ActionBlock>();
            actionsActionBlocksPrev = new Stack<ActionBlock>();
            AnalyseBlocks = new List<TestAnalized>();
            Rows = new List<RowAnalized>();
            ParseType = ParseType.LINEAR;
        }

        public static CoreFile GetInstance(string? fileSource, string? content, MainWindow mainWindow)
        {
            bool isFile = fileSource != null;
            var coreFile = new CoreFile((isFile ? fileSource : content)!, mainWindow, isFile);
            if (isFile)
            {
                coreFile.DecodeFile(fileSource);
            }
            else
            {
                coreFile.DestructText(content);
            }
            return coreFile;
        }

        private void DecodeFile(string fileSource)
        {
            DestructText("");
        }

        private void DestructText(string text)
        {
            string[] enteries = text.Replace("\r", "").Split(new[] {"\n"}, StringSplitOptions.None);
            for (int i = 0; i < enteries.Length; i++)
            {
                rowNatives.Add(new RowNative(enteries[i], i));
            }
            //try
            foreach(var n in rowNatives)
            {
                NativeRowUI nativeRowUi = new NativeRowUI(n, this);
                mainWindow.NativePanel.Children.Add(nativeRowUi);
            }
        }

        public void AI_Analize(FilterPatterns filterPatterns, ParseType ParseType)
        {
            this.filterPatterns = filterPatterns;
            this.ParseType = ParseType;
            tempTest = new TestAnalized(0, "test" + 0);
            foreach (var rowNative in rowNatives)
            {
                if (rowNative.included)
                {
                    RowAnalized rowAnalized = new RowAnalized(rowNative, filterPatterns, this);
                    foreach (var iRowChangedObserver in IRowChangedObservers)
                    {
                        iRowChangedObserver.OnRowAdded(rowAnalized);
                    }
                }
            }
            OnNextTestDetected();
            analizedTestUi = new AnalizedTestUI(this);
            mainWindow.EditorHolder.Children.Add(analizedTestUi);
            analizedTestUi.ReInit(AnalyseBlocks.Find(X => X.testId == testIdsOrdered[0]));
        }

        public TestAnalized? GetNextNonFormedTestAnalized(string testId)
        {
            try
            {
                int index = testIdsOrdered.FindIndex(x => x == testId);
                if (index < testIdsOrdered.Count - 1)
                {
                    while (AnalyseBlocks.Find(x => x.testId == testIdsOrdered[index + 1]).formed && index < testIdsOrdered.Count - 1)
                    {
                        index++;
                    }
                    return AnalyseBlocks.Find(x => x.testId == testIdsOrdered[index + 1]);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
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
            coreFile.IRowChangedObservers = new IRowChangedIterator();
            coreFile.actionsActionBlocksNext = new Stack<ActionBlock>();
            coreFile.actionsActionBlocksPrev = new Stack<ActionBlock>();
            coreFile.filterPatterns.Decode();
            return coreFile;
        }
        #endregion

        #region UndoRedo
        private void FinishAction()
        {
            actionsActionBlocksPrev.Push(actionBlock?.Clone());
            actionBlock = null;
        }

        private ActionBlock ImplementObjectsChanges(ActionBlock result)
        {
            ActionBlock changes = new ActionBlock();
            KeyValuePair<EditorAction, IdsRowMemento> idsRowMemento = new KeyValuePair<EditorAction, IdsRowMemento>();
            for (int i = 0; i < result.Mementoes.Count; i++)
            {
                if (result.Mementoes[i].GetType() == typeof(IdsRowMemento))
                {
                    idsRowMemento = new KeyValuePair<EditorAction, IdsRowMemento>(result.EditorActions[i], (IdsRowMemento) result.Mementoes[i]);
                    break;
                }
            }
            if (idsRowMemento.Value != null)
            {
                changes.AddAction(idsRowMemento.Key, RestoreState(idsRowMemento.Value));
            }
            Dictionary<RowMemento, EditorAction> rowMementoes = new Dictionary<RowMemento, EditorAction>();
            for (int i = 0; i < result.Mementoes.Count; i++)
            {
                if (result.Mementoes[i].GetType() == typeof(RowMemento))
                {
                    rowMementoes.Add((RowMemento)result.Mementoes[i], result.EditorActions[i]);
                }
            }
            Dictionary<TestMemento, EditorAction> testMementoes = new Dictionary<TestMemento, EditorAction>();
            for (int i = 0; i < result.Mementoes.Count; i++)
            {
                if (result.Mementoes[i].GetType() == typeof(TestMemento))
                {
                    testMementoes.Add((TestMemento)result.Mementoes[i], result.EditorActions[i]);
                }
            }
            foreach (var (rowMemento, action) in rowMementoes)
            {
                changes.AddAction(action, rowMemento.FindAndRestore(this));
            }
            foreach (var (testMemento, action) in testMementoes)
            {
                changes.AddAction(action, testMemento.FindAndRestore(this));
            }
            return changes;
        }

        public bool Undo()
        {
            if (actionsActionBlocksPrev.TryPop(out ActionBlock result))
            {
                ActionBlock response = ImplementObjectsChanges(result);
                if (result.EditorActions.Contains(EditorAction.TEST_FORMED))
                {
                    string prevTestId = testIdsOrdered[testIdsOrdered.IndexOf(analizedTestUi?.GetTestAnalized().testId) - 1];
                    analizedTestUi?.ReInit(AnalyseBlocks.Find(x => x.testId == prevTestId)!);
                }
                else
                {
                    analizedTestUi?.Undo();
                }
                actionsActionBlocksNext.Push(response.Clone());
                return true;
            }
            return false;
        }

        public bool Redo()
        {
            if (actionsActionBlocksNext.TryPop(out ActionBlock result))
            {
                ActionBlock response = ImplementObjectsChanges(result);
                actionsActionBlocksPrev.Push(response.Clone());
                if (result.EditorActions.Contains(EditorAction.TEST_FORMED))
                {
                    string prevTestId = testIdsOrdered[testIdsOrdered.IndexOf(analizedTestUi?.GetTestAnalized().testId) + 1];
                    analizedTestUi?.ReInit(AnalyseBlocks.Find(x => x.testId == prevTestId)!);
                }
                else
                {
                    analizedTestUi?.Undo();
                }
                return true;
            }
            return false;
        }
        #endregion

        #region ExportImport

        public void Export()
        {
            string exportString = "";
            if (ParseType == ParseType.PREMIUM)
            {
                exportString = "<GIFT>\n";
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var testAnalized in AnalyseBlocks)
            {
                exportString += testAnalized.GetEncodedTest(ParseType, this);
            }
            if (ParseType == ParseType.LINEAR)
            {
                Encoding win1251 = Encoding.GetEncoding("windows-1251");
                Encoding utf8 = Encoding.UTF8;
                byte[] utf8Bytes = utf8.GetBytes(exportString);
                byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
                exportString = win1251.GetString(win1251Bytes);
            }
            Encoding encoding = ParseType switch
            {
                ParseType.PREMIUM => Encoding.UTF8,
                ParseType.LINEAR => Encoding.GetEncoding(1251),
                _ => Encoding.UTF8
            };
            File.WriteAllText(Path.Combine(SpecialDirectories.Desktop, "main2.qst"), exportString, encoding);
        }
        #endregion

        #region Implementation
        public void OnNextTestDetected()
        {
            foreach (var iTestChangedObserver in ITestChangedObservers)
            {
                TestAnalized t = tempTest.Clone();
                iTestChangedObserver.OnTestAdded(t);
            }
            tempTest = new TestAnalized(testIdsOrdered.Count, "test" + testIdsOrdered.Count);
        }

        public void OnRowAdded(RowAnalized rowAnalized)
        {
            Rows.Add(rowAnalized);
            rowsIdsOrdered.Add(rowAnalized.rowId);
        }


#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        public void OnRowConcatenated(string? rowIdIdTop, string? rowIdBottom)
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        {
            RowAnalized rowAnalized;
            int nextRow;
            if (rowIdIdTop != null)
            {
                rowAnalized = Rows.Find(x => x.rowId == rowIdIdTop);
                nextRow = rowsIdsOrdered.IndexOf(rowIdIdTop) + 1;
                while (!Rows.Find(x => x.rowId == rowsIdsOrdered[nextRow]).includedToAnalysis)
                {
                    nextRow++;
                    if (nextRow == rowsIdsOrdered.Count)
                    {
                        return;
                    }
                }
            }
            else
            {
                rowAnalized = Rows.Find(x => x.rowId == rowIdBottom);
                nextRow = rowsIdsOrdered.IndexOf(rowIdBottom) - 1;
                while (!Rows.Find(x => x.rowId == rowsIdsOrdered[nextRow]).includedToAnalysis)
                {
                    nextRow--;
                    if (nextRow < 0)
                    {
                        return;
                    }
                }
            }
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
                            if (rowIdBottom == null)
                            {
                                rowAnalized.visibleEditedContent = rowAnalized.visibleEditedContent + " " + rowNext.hiddenContent +
                                                                   rowNext.visibleEditedContent;
                            }
                            else
                            {
                                rowAnalized.visibleEditedContent = rowNext.visibleEditedContent + " " + rowAnalized.hiddenContent +
                                                                   rowAnalized.visibleEditedContent;
                                rowAnalized.hiddenContent = rowNext.hiddenContent;
                            }
                            rowAnalized.nativeNumbers.AddRange(rowNext.nativeNumbers);
                            rowNext.includedToAnalysis = false;
                            foreach (var t in rowNext.nativeNumbers)
                            {
                                rowNatives.Find(x => x.rowNumber == t).included = true;
                            }
                            foreach (var iRowChangedObserver in IRowChangedObservers)
                            {
                                if (iRowChangedObserver.GetType() != GetType())
                                {
                                    iRowChangedObserver.OnRowDeleted(rowNext.rowId);
                                }
                            }
                            actionBlock = new ActionBlock();
                            ObjectMemento o3 = testAnalized.SaveState();
                            ObjectMemento o4 = testAnalized2.SaveState();
                            testAnalized2.DisconnectRow(rowNext.rowId);
                            testAnalized.ConnectToRow(rowNext);
                            actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o3);
                            actionBlock.AddAction(EditorAction.ROW_CONCATENATED, o4);
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
                    rowAnalized.visibleEditedContent = rowAnalized.visibleEditedContent.Substring(0, position);
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
                foreach (var t in rowAnalized.nativeNumbers)
                {
                    rowNatives.Find(x => x.rowNumber == t).included = false;
                }
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
                rowAnalized.rowType = rowType;
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
            ObjectMemento o1 = SaveState();
            AnalyseBlocks.Add(test);
            testIdsOrdered.Add(test.testId);
            actionBlock = new ActionBlock();
            actionBlock.AddAction(EditorAction.TEST_ADDED, o1);
            FinishAction();
        }

        public void OnTestAdded(List<string> rowIds)
        {
            RowAnalized rw = Rows.Find(x => x.rowId == rowIds[0]);
            TestAnalized parentTestAnalized = AnalyseBlocks.Find(x => x.testId == rw.testId);
            if (rw != null && parentTestAnalized != null)
            {
                ObjectMemento o1 = SaveState();
                ObjectMemento o2 = parentTestAnalized.SaveState();
                int index = testIdsOrdered.FindIndex(x => x == rw.testId) + 1;
                tempTest = new TestAnalized(testIdsOrdered.Count, "test" + testIdsOrdered.Count);
                TestAnalized t = tempTest.Clone();
                foreach (var iTestChangedObserver in ITestChangedObservers)
                {
                    if (iTestChangedObserver.GetType() == GetType())
                    {
                        AnalyseBlocks.Add(t);
                        testIdsOrdered.Insert(index, t.testId);
                    }
                    else
                    {
                        iTestChangedObserver.OnTestAdded(t);
                    }
                }
                List<ObjectMemento> objectMementoes = new List<ObjectMemento> { o1, o2 };
                foreach (var rowId in rowIds)
                {
                    RowAnalized rw0 = Rows.Find(x => x.rowId == rowId);
                    if (rw0 != null)
                    {
                        objectMementoes.Add(rw0.SaveState());
                        parentTestAnalized.DisconnectRow(rw0.rowId);
                        t.ConnectToRow(rw0);
                    }
                }
                actionBlock = new ActionBlock();
                foreach (var objectMemento in objectMementoes)
                {
                    actionBlock.AddAction(EditorAction.TEST_ADDED, objectMemento);
                }
                FinishAction();
            }
        }

        public void OnTestDeleted(TestAnalized test)
        {
            ObjectMemento o1 = test.SaveState();
            test.included = false;
            testIdsOrdered.Remove(test.testId);
            actionBlock = new ActionBlock();
            actionBlock.AddAction(EditorAction.TEST_DELETED, o1);
            FinishAction();
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
        #endregion

        #region Memento
        public ObjectMemento SaveState()
        {
            return new IdsRowMemento(rowsIdsOrdered, testIdsOrdered);
        }

        public ObjectMemento RestoreState(ObjectMemento objectMemento)
        {
            ObjectMemento objectMemento2 = new IdsRowMemento(rowsIdsOrdered, testIdsOrdered);
            rowsIdsOrdered = ((IdsRowMemento) objectMemento).IdsRList;
            testIdsOrdered = ((IdsRowMemento) objectMemento).IdsTList;
            return objectMemento2;
        }
        #endregion
    }
}
