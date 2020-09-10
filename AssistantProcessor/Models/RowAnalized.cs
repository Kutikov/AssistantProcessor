﻿using System.Collections.Generic;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;

namespace AssistantProcessor.Models
{
    public class RowAnalized : IUndoRedoObject
    {
        public RowType rowType;
        public int rowNumber;
        public string rowId; //static
        public List<int> nativeNumbers;
        public bool includedToAnalysis;
        public string content; //static
        public string hiddenContent;
        public string visibleEditedContent;
        public string testId;

        public RowAnalized(RowNative rowNative, FilterPatterns filterPatterns, CoreFile coreFile)
        {
            content = rowNative.content;
            rowNumber = rowNative.rowNumber;
            rowId = "row" + rowNative.rowNumber;
            if (coreFile.tempTest.correctAnswers.Count == 0 && coreFile.tempTest.wrongAnswers.Count == 0)
            {

            }
            else
            {
                if()
            }
        }

        public ObjectMemento SaveState()
        {
            return new RowMemento(rowType, rowNumber, nativeNumbers, includedToAnalysis, hiddenContent, visibleEditedContent, testId);
        }

        public void RestoreState(ObjectMemento objectMemento)
        {
            RowMemento rowMemento = (RowMemento) objectMemento;
            rowType = rowMemento.RowType;
            rowNumber = rowMemento.RowNumber;
            nativeNumbers = rowMemento.NativeNumbers;
            includedToAnalysis = rowMemento.IncludedToAnalysis;
            hiddenContent = rowMemento.HiddenContent;
            visibleEditedContent = rowMemento.VisibleEditedContent;
            testId = rowMemento.TestId;
        }
    }
}
