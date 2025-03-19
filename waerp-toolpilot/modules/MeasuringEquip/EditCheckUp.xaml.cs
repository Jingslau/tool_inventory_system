using System.IO;
using System.Windows;
using waerp_toolpilot.models;

namespace waerp_toolpilot.modules.Administration.MeasuringEquip
{
    /// <summary>
    /// Interaction logic for EditCheckUp.xaml
    /// </summary>
    public partial class EditCheckUp : Window
    {
        public EditCheckUp()
        {
            InitializeComponent();
            MeasureEquipName.Text = MeasuringEquipModel.MeasuringEquipID;
            MeasureEquipVendor.Text = MeasuringEquipModel.MeasuringEquipName;
            MeasureEquipQuant.Text = MeasuringEquipModel.MeasuringEquipVendor;
            selectedDate.Text = MeasuringEquipModel.CurrentSelectedHistory_CheckDate.ToString("d");
            PDFFilePath.Text = MeasuringEquipModel.CurrentSelectedHistory_DocPath;
        }

        private void CreateCheckUp_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }

        private void CloseDialog_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void selectPdfDocument(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); openFileDialog.Filter = "PDF Dokument|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                PDFFilePath.Text = Path.GetDirectoryName(openFileDialog.FileName) + "\\" + Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
