using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;
using AssistantProcessor.Models;

namespace AssistantProcessor.UI
{
    public partial class AnalizedTestUI : UserControl, IRowChangedObserver, ITestChangedObserver
    {
        private readonly CoreFile coreFile;
        private TestAnalized? testAnalized;
        private readonly List<AnalizedRowUI> analizedRowUis;

        public AnalizedTestUI(CoreFile coreFile)
        {
            InitializeComponent();
            this.coreFile = coreFile;
            analizedRowUis = new List<AnalizedRowUI>();
            coreFile.IRowChangedObservers.Add(this);
            coreFile.ITestChangedObservers.Add(this);
        }

        ~AnalizedTestUI()
        {
            coreFile.ITestChangedObservers.Remove(this);
            coreFile.IRowChangedObservers.Remove(this);
        }

        public void ReInit(TestAnalized testAnalized)
        {
            this.testAnalized = testAnalized;
            RowUIsRedraw();
            CheckTestValidaty();
        }

        private void NextAnalizedRow_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var iRowChangedObserver in coreFile.IRowChangedObservers)
            {
                iRowChangedObserver.OnRowMovedPrev(testAnalized.testId);
            }
        }

        private void AddComment_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ErrorWarning_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ErrorCaption.Visibility = Visibility.Collapsed;
            WarningCaption.Visibility = Visibility.Collapsed;
            EncodingErrorText.Visibility = Visibility.Collapsed;
            NoCorrectAnswersText.Visibility = Visibility.Collapsed;
            NoTaskText.Visibility = Visibility.Collapsed;
            MultipleAnswersText.Visibility = Visibility.Collapsed;
            NoCommentsText.Visibility = Visibility.Collapsed;
            switch (((Viewbox) sender).Name)
            {
                case "EncodingError":
                    ErrorCaption.Visibility = Visibility.Visible;
                    EncodingErrorText.Visibility = Visibility.Visible;
                    break;
                case "NoTrueAnswers":
                    ErrorCaption.Visibility = Visibility.Visible;
                    NoCorrectAnswersText.Visibility = Visibility.Visible;
                    break;
                case "NoTask":
                    ErrorCaption.Visibility = Visibility.Visible;
                    NoTaskText.Visibility = Visibility.Visible;
                    break;
                case "MultipleCorrectAnswers":
                    WarningCaption.Visibility = Visibility.Visible;
                    MultipleAnswersText.Visibility = Visibility.Visible;
                    break;
                case "NoComments":
                    WarningCaption.Visibility = Visibility.Visible;
                    NoCommentsText.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void CloseButton_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Dialog.Visibility = Visibility.Collapsed;
        }

        private void RowUIsRedraw()
        {
            foreach (var rowUi in analizedRowUis.ToArray())
            {
                RowsHolder.Children.Remove(rowUi);
                analizedRowUis.Remove(rowUi);
            }

            List<string> ids = testAnalized.OrderedConnectedIds(coreFile.rowsIdsOrdered);
            for (int i = 0; i < ids.Count; i++)
            {
                AnalizedRowUI analizedRowUi = new AnalizedRowUI(coreFile.Rows.First(x => x.rowId == ids[i]), coreFile, i, this);
                analizedRowUis.Add(analizedRowUi);
                RowsHolder.Children.Add(analizedRowUi);
            }
        }

        private void CheckTestValidaty()
        {
            bool enableFormButton = true;
            switch (testAnalized.correctAnswers.Count)
            {
                case 0:
                    enableFormButton = false;
                    NoTrueAnswers.Visibility = Visibility.Visible;
                    MultipleCorrectAnswers.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    NoTrueAnswers.Visibility = Visibility.Collapsed;
                    MultipleCorrectAnswers.Visibility = Visibility.Collapsed;
                    break;
                default:
                    NoTrueAnswers.Visibility = Visibility.Collapsed;
                    MultipleCorrectAnswers.Visibility = Visibility.Visible;
                    break;
            }

            if (!CheckWin1251())
            {
                enableFormButton = false;
            }
            if (testAnalized.task.Count == 0)
            {
                enableFormButton = false;
                NoTask.Visibility = Visibility.Visible;
            }
            else
            {
                NoTask.Visibility = Visibility.Collapsed;
            }
            NoComments.Visibility = testAnalized.comments.Count == 0 ? Visibility.Visible : Visibility.Hidden;
            FormTest.IsEnabled = enableFormButton;
        }

        public bool CheckWin1251()
        {
            bool enableFormButton = true;
            EncodingError.Visibility = Visibility.Collapsed;
            if (coreFile.ParseType == ParseType.LINEAR)
            {
                foreach (var analizedRowUi in analizedRowUis)
                {
                    if (!analizedRowUi.CheckWin1251())
                    {
                        enableFormButton = false;
                        EncodingError.Visibility = Visibility.Visible;
                    }
                }
            }
            FormTest.IsEnabled = enableFormButton;
            return enableFormButton;
        }

        public void OnRowAdded(RowAnalized rowAnalized)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnRowConcatenated(string? rowIdTop, string? rowIdBottom)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnRowDiversed(string rowId, int position)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnRowDeleted(string rowId)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnRowMovedNext(string rowId)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnRowMovedPrev(string testId)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnRowTypeChanged(string rowId, RowType rowType)
        {
            CheckTestValidaty();
        }

        public void OnTestAdded(TestAnalized test)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnTestAdded(List<string> rowIds)
        {
            RowUIsRedraw();
            CheckTestValidaty();
        }

        public void OnTestDeleted(TestAnalized test)
        {
            
        }

        public void OnTestFormed(string testId)
        {
            TestAnalized? testAnalizedL = coreFile.GetNextNonFormedTestAnalized(testId);
            if (testAnalizedL != null)
            {
                ReInit(testAnalizedL);
            }
        }

        private void FormTest_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var iTestChangedObserver in coreFile.ITestChangedObservers)
            {
                iTestChangedObserver.OnTestFormed(testAnalized.testId);
            }
        }

        private void CreateNewTest_OnClick(object sender, RoutedEventArgs e)
        {
            List<string> ids = (from analizedRowUi in analizedRowUis where analizedRowUi.CheckBox.IsChecked == true select analizedRowUi.rowId).ToList();
            foreach (var iTestChangedObserver in coreFile.ITestChangedObservers)
            {
                iTestChangedObserver.OnTestAdded(ids);
            }
        }
    }
}
