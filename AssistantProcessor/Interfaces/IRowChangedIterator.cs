using System;
using System.Collections;
using System.Collections.Generic;
using AssistantProcessor.Models;
using AssistantProcessor.UI;

namespace AssistantProcessor.Interfaces
{
    public class IRowChangedIterator : IList<IRowChangedObserver>
    {
        private CoreFile? coreFile;
        private List<AnalizedRowUI> analizedRows = new List<AnalizedRowUI>();
        private List<NativeRowUI> nativeRowUis = new List<NativeRowUI>();
        private AnalizedTestUI? analizedTestUi;
        public IEnumerator<IRowChangedObserver> GetEnumerator()
        {
            yield return coreFile!;
            foreach (var analizedRow in analizedRows)
            {
                yield return analizedRow;
            }
            foreach (var nativeRowUi in nativeRowUis)
            {
                yield return nativeRowUi;
            }
            if (analizedTestUi != null)
            {
                yield return analizedTestUi;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IRowChangedObserver item)
        {
            if (item is CoreFile file)
            {
                coreFile = file;
            }
            if (item.GetType() == typeof(AnalizedTestUI))
            {
                analizedTestUi = (AnalizedTestUI?) item;
            }
            if (item.GetType() == typeof(NativeRowUI))
            {
                if (!nativeRowUis.Contains((NativeRowUI)item))
                {
                    nativeRowUis.Add((NativeRowUI)item);
                }
            }
            if (item.GetType() == typeof(AnalizedRowUI))
            {
                if (!analizedRows.Contains((AnalizedRowUI) item))
                {
                    analizedRows.Add((AnalizedRowUI)item);
                }
            }
        }

        public void Clear()
        {
            coreFile = null;
            analizedTestUi = null;
            analizedRows = new List<AnalizedRowUI>();
            nativeRowUis = new List<NativeRowUI>();
        }

        public bool Contains(IRowChangedObserver item)
        {
            if (item is CoreFile)
            {
                return item.Equals(coreFile);
            }
            if (item.GetType() == typeof(AnalizedTestUI))
            {
                return item.Equals(analizedTestUi);
            }
            if (item.GetType() == typeof(NativeRowUI))
            {
                return nativeRowUis.Contains((NativeRowUI) item);
            }
            return item.GetType() == typeof(AnalizedRowUI) && analizedRows.Contains((AnalizedRowUI)item);
        }

        public void CopyTo(IRowChangedObserver[] array, int arrayIndex)
        {
            
        }

        public bool Remove(IRowChangedObserver item)
        {
            if (item is CoreFile)
            {
                coreFile = null;
                return true;
            }
            if (item.GetType() == typeof(AnalizedTestUI))
            {
                analizedTestUi = null;
                return true;
            }
            if (item.GetType() == typeof(NativeRowUI))
            {
                if (nativeRowUis.Contains((NativeRowUI)item))
                {
                    nativeRowUis.Remove((NativeRowUI)item);
                    return true;
                }
            }
            if (item.GetType() == typeof(AnalizedRowUI))
            {
                if (analizedRows.Contains((AnalizedRowUI)item))
                {
                    analizedRows.Remove((AnalizedRowUI)item);
                    return true;
                }
            }
            return false;
        }

        private int CountF()
        {
            int ret = analizedRows.Count + nativeRowUis.Count;
            if (analizedTestUi != null)
            {
                ret++;
            }
            if (coreFile != null)
            {
                ret++;
            }
            return ret;
        }

        public int Count => CountF();

        public bool IsReadOnly { get; }
        public int IndexOf(IRowChangedObserver item)
        {
            if (item is CoreFile)
            {
                return 0;
            }
            if (item.GetType() == typeof(AnalizedTestUI))
            {
                return 1 + analizedRows.Count + nativeRowUis.Count;
            }
            if (item.GetType() == typeof(NativeRowUI))
            {
                return 1 + analizedRows.Count + nativeRowUis.IndexOf((NativeRowUI)item);
            }
            return analizedRows.IndexOf((AnalizedRowUI) item) + 1;
        }

        public void Insert(int index, IRowChangedObserver item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IRowChangedObserver this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
