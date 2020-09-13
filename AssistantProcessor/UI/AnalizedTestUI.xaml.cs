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
        private readonly TestAnalized testAnalized;
        private readonly List<AnalizedRowUI> analizedRowUis;
        public AnalizedTestUI(CoreFile coreFile, TestAnalized testAnalized)
        {
            InitializeComponent();
            this.coreFile = coreFile;
            this.testAnalized = testAnalized;
            analizedRowUis = new List<AnalizedRowUI>();
            coreFile.IRowChangedObservers.Add(this);
            coreFile.ITestChangedObservers.Add(this);
        }
        ~AnalizedTestUI()
        {
            coreFile.ITestChangedObservers.Remove(this);
            coreFile.IRowChangedObservers.Remove(this);
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

        public void OnRowAdded(RowAnalized rowAnalized)
        {
            throw new NotImplementedException();
        }

        public void OnRowConcatenated(string? rowIdTop, string? rowIdBottom)
        {
            throw new NotImplementedException();
        }

        public void OnRowDiversed(string rowId, int position)
        {
            throw new NotImplementedException();
        }

        public void OnRowDeleted(string rowId)
        {
            throw new NotImplementedException();
        }

        public void OnRowMovedNext(string rowId)
        {
            throw new NotImplementedException();
        }

        public void OnRowMovedPrev(string testId)
        {
            throw new NotImplementedException();
        }

        public void OnRowTypeChanged(string rowId, RowType rowType)
        {
            throw new NotImplementedException();
        }

        public void OnTestAdded(TestAnalized test)
        {
            throw new NotImplementedException();
        }

        public void OnTestAdded(List<string> rowIds)
        {
            throw new NotImplementedException();
        }

        public void OnTestDeleted(TestAnalized test)
        {
            throw new NotImplementedException();
        }

        public void OnTestFormed(string testId)
        {
            throw new NotImplementedException();
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
