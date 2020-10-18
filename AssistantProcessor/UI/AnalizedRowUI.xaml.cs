using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;
using AssistantProcessor.Models;

namespace AssistantProcessor.UI
{
    public partial class AnalizedRowUI : UserControl, IRowChangedObserver
    {
        private readonly RowAnalized rowAnalized;
        private readonly AnalizedTestUI analizedTestUi;
        private readonly CoreFile coreFile;
        private readonly bool inited;
        public string rowId;
        public AnalizedRowUI(RowAnalized rowAnalized, CoreFile coreFile, int rowNumber, AnalizedTestUI analizedTestUi)
        {
            InitializeComponent();
            this.rowAnalized = rowAnalized;
            this.coreFile = coreFile;
            this.analizedTestUi = analizedTestUi;
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
            rowId = rowAnalized.rowId;
            RowNumberText.Text = (rowNumber + 1).ToString();
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
                    else
                    {
                        analizedTestUi.CheckWin1251();
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
                    else
                    {
                        analizedTestUi.CheckWin1251();
                    }
                    break;
                default:
                    analizedTestUi.CheckWin1251();
                    break;
            }
        }

        public bool CheckWin1251()
        {
            VisibleEditingTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            Encoding utf8 = Encoding.UTF8;
            byte[] utf8Bytes = utf8.GetBytes(rowAnalized.visibleEditedContent);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            string str = win1251.GetString(win1251Bytes);
            if (str.Length != rowAnalized.visibleEditedContent.Length)
            {
                VisibleEditingTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 50, 50));
                return false;
            }
            return true;
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
