using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;
using AssistantProcessor.Models;

namespace AssistantProcessor.UI
{
    public partial class AnalizedRowUI : UserControl, IRowChangedObserver
    {
        private readonly RowAnalized rowAnalized;
        private readonly CoreFile coreFile;
        private readonly bool inited;
        public AnalizedRowUI(RowAnalized rowAnalized, CoreFile coreFile)
        {
            InitializeComponent();
            this.rowAnalized = rowAnalized;
            this.coreFile = coreFile;
            VisibleEditingTextBox.Text = rowAnalized.visibleEditedContent;
            switch (rowAnalized.rowType)
            {
                case RowType.WRONG_ANSWER:
                    WrongRadio.IsChecked = true;
                    break;
                case RowType.CORRECT_ANSWER:
                    CorrectRadio.IsChecked = true;
                    break;
                case RowType.TASK:
                    TaskRadio.IsChecked = true;
                    break;
                case RowType.COMMENT:
                    CommentRadio.IsChecked = true;
                    break;
            }
            HiddenText.Text = rowAnalized.hiddenContent;
            inited = true;
            coreFile.IRowChangedObservers.Add(this);
        }

        ~AnalizedRowUI()
        {
            coreFile.IRowChangedObservers.Remove(this);
        }

        private void Radio_OnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = ((RadioButton) sender);
            if (rb.IsChecked == true && inited)
            {
                RowType rowType = rb.Name switch
                {
                    "TaskRadio" => RowType.TASK,
                    "CorrectRadio" => RowType.CORRECT_ANSWER,
                    "WrongRadio" => RowType.WRONG_ANSWER,
                    "CommentRadio" => RowType.COMMENT,
                    _ => RowType.TASK
                };
                if (rowType != rowAnalized.rowType)
                {
                    foreach (var iRowChangedObserver in coreFile.IRowChangedObservers)
                    {
                        iRowChangedObserver.OnRowTypeChanged(rowAnalized.rowId, rowType);
                    }
                }
            }
        }

        private void RemoveButton_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var iRowChangedObserver in coreFile.IRowChangedObservers)
            {
                iRowChangedObserver.OnRowDeleted(rowAnalized.rowId);
            }
        }

        private void ToNextButton_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var iRowChangedObserver in coreFile.IRowChangedObservers)
            {
                iRowChangedObserver.OnRowMovedNext(rowAnalized.rowId);
            }
        }

        private void VisibleEditingTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    VisibleEditingTextBox.Text = rowAnalized.visibleEditedContent;
                    break;
                default:
                    rowAnalized.visibleEditedContent = VisibleEditingTextBox.Text;
                    break;
            }
        }

        private void VisibleEditingTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (VisibleEditingTextBox.CaretIndex > 0 &&
                        VisibleEditingTextBox.CaretIndex < VisibleEditingTextBox.Text.Length)
                    {
                        foreach (var iRowChangedObserver in coreFile.IRowChangedObservers)
                        {
                            iRowChangedObserver.OnRowDiversed(rowAnalized.rowId, VisibleEditingTextBox.CaretIndex);
                        }
                    }
                    break;
                case Key.Delete:
                    if (VisibleEditingTextBox.CaretIndex == VisibleEditingTextBox.Text.Length)
                    {
                        foreach (var iRowChangedObserver in coreFile.IRowChangedObservers)
                        {
                            iRowChangedObserver.OnRowConcatenated(rowAnalized.rowId, null);
                        }
                    }
                    break;
                case Key.Back:
                    if (VisibleEditingTextBox.CaretIndex == 0)
                    {
                        foreach (var iRowChangedObserver in coreFile.IRowChangedObservers)
                        {
                            iRowChangedObserver.OnRowConcatenated(null, rowAnalized.rowId);
                        }
                    }
                    break;
            }
        }

        public void OnRowAdded(RowAnalized rowAnalized)
        {
            //test ui
        }

        public void OnRowConcatenated(string? rowIdTop, string? rowIdBottom)
        {
            if (rowIdTop != null)
            {
                if (rowIdTop == rowAnalized.rowId)
                {
                    VisibleEditingTextBox.Text = rowAnalized.visibleEditedContent;
                }
            }
        }

        public void OnRowDiversed(string rowId, int position)
        {
            if (rowId == rowAnalized.rowId)
            {
                VisibleEditingTextBox.Text = rowAnalized.visibleEditedContent;
            }
        }

        public void OnRowDeleted(string rowId)
        {
            //test ui
        }

        public void OnRowMovedNext(string rowId)
        {
            //test ui
        }

        public void OnRowMovedPrev(string testId)
        {
            //test ui
        }

        public void OnRowTypeChanged(string rowId, RowType rowType)
        {
            
        }
    }
}
