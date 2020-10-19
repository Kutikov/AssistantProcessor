using System.Windows.Controls;
using System.Windows.Media;
using AssistantProcessor.Enums;
using AssistantProcessor.Interfaces;
using AssistantProcessor.Models;

namespace AssistantProcessor.UI
{
    /// <summary>
    /// Логика взаимодействия для NativeRowUI.xaml
    /// </summary>
    public partial class NativeRowUI : UserControl, IRowChangedObserver
    {
        private readonly RowNative rowNative;
        private readonly CoreFile coreFile;

        public NativeRowUI(RowNative rowNative, CoreFile coreFile)
        {
            InitializeComponent();
            this.rowNative = rowNative;
            this.coreFile = coreFile;
            RowNumberText.Text = (rowNative.rowNumber + 1).ToString();
            NativeText.Text = this.rowNative.content;
            coreFile.IRowChangedObservers.Add(this);
            Root.Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString("#003300"));
        }

        ~NativeRowUI()
        {
            coreFile.IRowChangedObservers.Remove(this);
        }

        public void OnRowAdded(RowAnalized rowAnalized)
        {

        }

        private void UpdateColors()
        {
            Root.Background = rowNative.included ? new SolidColorBrush((Color) ColorConverter.ConvertFromString("#003300")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5f2222"));
        }

        public void OnRowConcatenated(string? rowIdTop, string? rowIdBottom)
        {
            UpdateColors();
        }

        public void OnRowDiversed(string rowId, int position)
        {

        }

        public void OnRowDeleted(string rowId)
        {
            UpdateColors();
        }

        public void OnRowMovedNext(string rowId)
        {

        }

        public void OnRowMovedPrev(string testId)
        {
        }

        public void OnRowTypeChanged(string rowId, RowType rowType)
        {
        }
    }
}
