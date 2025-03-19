using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows.Controls;

namespace waerp_management.application.Administration
{
    /// <summary>
    /// Interaction logic for AdministrationOverviewView.xaml
    /// </summary>
    public partial class AdministrationOverviewView : UserControl
    {
        public AdministrationOverviewView()
        {
            InitializeComponent();

            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            DataContext = this;
        }
        public Func<ChartPoint, string> PointLabel { get; set; }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }
}
