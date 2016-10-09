using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Data;

namespace SR25b
{
    /// <summary>
    /// Interaction logic for UnutList.xaml
    /// </summary>
    public partial class UnutList : UserControl
    {
        SR25b.zsrDataSet foo = null;

        SR25b.zsrDataSetTableAdapters.FOOD_DESTableAdapter fdda = new zsrDataSetTableAdapters.FOOD_DESTableAdapter();
        SR25b.zsrDataSetTableAdapters.NUTR_DEFTableAdapter ndda = new zsrDataSetTableAdapters.NUTR_DEFTableAdapter();
        SR25b.zsrDataSetTableAdapters.NUT_DATATableAdapter ddda = new zsrDataSetTableAdapters.NUT_DATATableAdapter();

        CollectionViewSource Fcvs = null;
        CollectionViewSource Dcvs = null;


        public UnutList()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }

        private void z_all_nuts_Click(object sender, RoutedEventArgs e)
        {
            ShowallNut();
        }

        private void ShowallNut()
        {
            Nutdf();
        }

        private void z_selected_nuts_Click(object sender, RoutedEventArgs e)
        {
            ShowSelNut();
        }

        private void ShowSelNut()
        {
            foo = (SR25b.zsrDataSet)this.FindResource("zsrDataSet");
            //ndda.ClearBeforeFill = true;
            ndda.FillByselected(foo.NUTR_DEF);
            //ndda.ClearBeforeFill = false;
            Dcvs = (CollectionViewSource)(this.FindResource("nUTR_DEFViewSource"));
            Dcvs.View.MoveCurrentToFirst();
            //MessageBox.Show(foo.NUTR_DEF.Rows.Count.ToString());
        }

        private void z_go_Click(object sender, RoutedEventArgs e)
        {
            getfoodlist();
        }

        private void getfoodlist()
        {
            decimal max = 100.0M;
            decimal min = 0.0M;

            if (nUTR_DEFDataGrid.SelectedItem == null)
                return;
            DataRowView drv = nUTR_DEFDataGrid.SelectedItem as DataRowView;
            if (drv == null)
                return;
            SR25b.zsrDataSet.NUTR_DEFRow fr = (SR25b.zsrDataSet.NUTR_DEFRow)drv.Row;

            foo = (SR25b.zsrDataSet)this.FindResource("zsrDataSet");

            decimal.TryParse(z_min.Text, out min);
            if (string.IsNullOrWhiteSpace(z_max.Text) == false)
                decimal.TryParse(z_max.Text, out max);
            // manually set the ItemsSource from the GetDataBy1 and use DefaultView.
            fOOD_DESDataGrid.ItemsSource = fdda.GetDataBy1(fr.Nutr_No, min, max).DefaultView;
            
            z_res.Content = string.Format(" Results, ( {0} Records Found )",fOOD_DESDataGrid.Items.Count);



        }




        private void Foo2Vsrc()
        {
            foo = (SR25b.zsrDataSet)this.FindResource("zsrDataSet");

            Fcvs = (CollectionViewSource)(this.FindResource("fD_GROUPFOOD_DESViewSource"));
            if (Fcvs != null && Fcvs.View != null)
                Fcvs.View.MoveCurrentToFirst();
        }

        /// <summary>
        /// Fill The NUTR_DEF Datatable form ACCESS DB
        /// </summary>
        private void Nutdf()
        {
            foo = (SR25b.zsrDataSet)this.FindResource("zsrDataSet");

            ndda.Fill(foo.NUTR_DEF);
            Dcvs = (CollectionViewSource)(this.FindResource("nUTR_DEFViewSource"));
            Dcvs.View.MoveCurrentToFirst();
        }

        private void nUTR_DEFDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = nUTR_DEFDataGrid.SelectedIndex;
            if (nUTR_DEFDataGrid.SelectedItem == null)
                return;
            DataRowView drv = nUTR_DEFDataGrid.SelectedItem as DataRowView;
            if (drv == null)
                return;
            SR25b.zsrDataSet.NUTR_DEFRow fr = (SR25b.zsrDataSet.NUTR_DEFRow)drv.Row;


        }

        private void z_prtnuts_Click(object sender, RoutedEventArgs e)
        {
            
            PrintVisual(nUTR_DEFDataGrid);
        }

        private  void PrintVisual(Visual prt)
        {
            PrintDialog dialog = new PrintDialog();

            if (dialog.ShowDialog().GetValueOrDefault() == true)
            {
                Size pg = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
                ((UIElement)prt).Measure(pg);

                ((UIElement)prt).Arrange(new Rect(10, 10, pg.Width, pg.Height));
                dialog.PrintVisual(prt, "Printout");
            }
        }

       

        private void z_prtFoods_Click(object sender, RoutedEventArgs e)
        {
            //fOOD_DESDataGrid.
        }





    }
}
