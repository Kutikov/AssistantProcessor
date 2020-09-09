using System.Collections.Generic;
using System.Linq;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;
using Newtonsoft.Json;

namespace AssistantProcessor.Models
{
    public class CoreFile : IRowChangedObserver, ITestChangedObserver
    {
        public string fileSource;
        public string content;
        public FilterPatterns filterPatterns;
        public List<RowNative> rowNatives;
        public ParseType ParseType;
        public List<RowAnalized> Rows;
        private List<TestAnalized> AnalyseBlocks;

        [JsonIgnore]
        private Stack<KeyValuePair<int, List<EditorAction>>> editorActions;
        [JsonIgnore]
        private Queue<int> actionNumbers;
        [JsonIgnore]
        private bool record;
        [JsonIgnore]
        private Dictionary<int, List<TestAnalized>> analyseBlocks;
        [JsonIgnore]
        private Dictionary<int, List<RowAnalized>> rowAnalizeds;

        [JsonIgnore] public List<IRowChangedObserver> IRowChangedObservers;
        [JsonIgnore] public List<ITestChangedObserver> ITestChangedObservers;

        #nullable enable
        public CoreFile(string? fileSource, string? content, FilterPatterns filterPatterns, ParseType parseType)
        {
            this.content = content;
            this.fileSource = fileSource;
            this.filterPatterns = filterPatterns;
            this.ParseType = parseType;
            editorActions = new Stack<KeyValuePair<int, List<EditorAction>>>();
            editorActions.Push(new KeyValuePair<int, List<EditorAction>>(0, new List<EditorAction>()));
            actionNumbers = new Queue<int>();
            actionNumbers.Enqueue(0);
            rowNatives = new List<RowNative>();
            analyseBlocks = new Dictionary<int, List<TestAnalized>>
            {
                {0, new List<TestAnalized>()}
            };
            rowAnalizeds = new Dictionary<int, List<RowAnalized>>
            {
                {0, new List<RowAnalized>()}
            };
            ITestChangedObservers = new List<ITestChangedObserver>();
            IRowChangedObservers = new List<IRowChangedObserver>();
        }
        #nullable disable

        public string Encode()
        {
            int order = actionNumbers.Peek();
            Rows = rowAnalizeds[order];
            AnalyseBlocks = analyseBlocks[order];
            return JsonConvert.SerializeObject(this);
        }

        public static CoreFile Decode(string source)
        {
            CoreFile coreFile = JsonConvert.DeserializeObject<CoreFile>(source);

            coreFile.editorActions = new Stack<KeyValuePair<int, List<EditorAction>>>();
            coreFile.editorActions.Push(new KeyValuePair<int, List<EditorAction>>(0, new List<EditorAction>()));
            coreFile.actionNumbers = new Queue<int>();
            coreFile.actionNumbers.Enqueue(0);
            coreFile.rowNatives = new List<RowNative>();
            coreFile.analyseBlocks = new Dictionary<int, List<TestAnalized>>
            {
                {0, coreFile.AnalyseBlocks}
            };
            coreFile.rowAnalizeds = new Dictionary<int, List<RowAnalized>>
            {
                {0, coreFile.Rows}
            };
            coreFile.ITestChangedObservers = new List<ITestChangedObserver>();
            coreFile.IRowChangedObservers = new List<IRowChangedObserver>();
            return coreFile;
        }

        private void FixAction(EditorAction editorAction)
        {
            if (!record)
            {
                int order = actionNumbers.Dequeue() + 3;
                actionNumbers.Enqueue(order);
                editorActions.Push(new KeyValuePair<int, List<EditorAction>>(order, new List<EditorAction>
                {
                    editorAction
                }));
                analyseBlocks.Remove(order - 3);
                rowAnalizeds.Remove(order - 3);
                analyseBlocks.Add(order, JsonConvert.DeserializeObject<List<TestAnalized>>(JsonConvert.SerializeObject(analyseBlocks.ElementAt(0).Value)));
                rowAnalizeds.Add(order, JsonConvert.DeserializeObject<List<RowAnalized>>(JsonConvert.SerializeObject(rowAnalizeds.ElementAt(0).Value)));
                record = true;
            }
            else
            {
                editorActions.Peek().Value.Add(editorAction);
            }
        }
        public bool Undo()
        {

            return false;
        }

        public void OnRowAdded(string rowId)
        {
            
        }

        public void OnRowConcatenated(string rowId)
        {
            FixAction(EditorAction.ROW_CONCATENATED);
        }

        public void OnRowDiversed(string rowId)
        {
            FixAction(EditorAction.ROW_DIVERSED);
        }

        public void OnRowDeleted(string rowId)
        {
            FixAction(EditorAction.ROW_DELETED);
        }

        public void OnRowMovedNext(string rowId)
        {
            FixAction(EditorAction.ROW_MOVED_NEXT);
        }

        public void OnRowMovedPrev(string rowId)
        {
            FixAction(EditorAction.ROW_MOVED_PREV);
        }

        public void OnRowTypeChanged(string rowId, RowType rowType)
        {
            FixAction(EditorAction.ROW_TYPE_CHAHGED);
        }

        public void OnTestAdded(TestAnalized analyseBlock)
        {
           analyseBlocks.ElementAt(0).Value.Add(analyseBlock);
        }

        public void OnTestDeleted(TestAnalized analyseBlock)
        {
            FixAction(EditorAction.TEST_DELETED);
            analyseBlocks.ElementAt(0).Value.Remove(analyseBlock);
        }

        public void OnTestFormed(TestAnalized analyseBlock)
        {
            FixAction(EditorAction.TEST_FORMED);
            analyseBlocks.ElementAt(0).Value.Add(analyseBlock);
        }
    }
}
