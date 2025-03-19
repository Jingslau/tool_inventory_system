using System.Windows.Controls;
using waerp_toolpilot.sql;

namespace waerp_toolpilot.application.OrderSystem.StorageOverview
{
    /// <summary>
    /// Interaction logic for StorageOverviewView.xaml
    /// </summary>
    public partial class StorageOverviewView : UserControl
    {
        public StorageOverviewView()
        {
            InitializeComponent();
            CurrentStock.Text = OrderItemOverviewQueries.GetCurrentStock().ToString();
            CurrentRent.Text = OrderItemOverviewQueries.GetCurrentRent().ToString();
            CurrentNew.Text = OrderItemOverviewQueries.GetCurrentNew().ToString();
        }
    }
}
