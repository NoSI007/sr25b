using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;
namespace SR25b
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private delegate void u4progbar(double zt);

        zsrDataSet foo = null;
        /// <summary>
        /// store running comparison Data for later
        /// re-organization.
        /// </summary>
        SR25b.zsrDataSet.NUT_DATADataTable Tempres = new zsrDataSet.NUT_DATADataTable();

        DataTable TheResTab = new DataTable("TheResultsTable");
        DataTable PanRes = new DataTable("PanelResultsTable");

        SR25b.zsrDataSetTableAdapters.FD_GROUPTableAdapter fgda = new zsrDataSetTableAdapters.FD_GROUPTableAdapter();
        SR25b.zsrDataSetTableAdapters.FOOD_DESTableAdapter fdda = new zsrDataSetTableAdapters.FOOD_DESTableAdapter();
        SR25b.zsrDataSetTableAdapters.NUTR_DEFTableAdapter ndda = new zsrDataSetTableAdapters.NUTR_DEFTableAdapter();
        SR25b.zsrDataSetTableAdapters.NUT_DATATableAdapter ddda = new zsrDataSetTableAdapters.NUT_DATATableAdapter();

        CollectionViewSource Gcvs = null;
        CollectionViewSource Fcvs = null;
        CollectionViewSource Dcvs = null;
        //CollectionViewSource Data = null;
        BackgroundWorker bgw = new BackgroundWorker();
        public delegate void updateprog();
        private int decp = 0;// decimal points for Paneled results.

        public class footag
        {
            public footag(string ndb,string d)
            {
                NDB_No = ndb;
                Desc = d;
            }
            public string NDB_No;
            public string Desc;
        }

        public HomePage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(HomePage_Loaded);
        }


        /// <summary>
        /// Make The DataTable for The results Datagrid.
        /// </summary>
        private void MakeResTab()
        {

            TheResTab.Columns.Clear();

            DataColumn ndc = new DataColumn("Selected Foods", typeof(string));
            ndc.Caption = "000";
            TheResTab.Columns.Add(ndc);// Food column Header.......
            foreach (SR25b.zsrDataSet.NUTR_DEFRow nd in foo.NUTR_DEF)
            {
                if (nd.IsSelected == true)
                {
                    //colname = string.Format("{0}-({1}).",nd.NutrDesc,nd.Units);
                    ndc = new DataColumn(nd.Nutr_No, typeof(float));
                    ndc.Caption = nd.Nutr_No;// save the nutr_no key!!
                    TheResTab.Columns.Add(ndc);
                }
            }
            z_res.ItemsSource = TheResTab.DefaultView;






        }

        /// <summary>
        /// Update change to definitions IsSelected.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NUTR_DEF_NUTR_DEFRowChanged(object sender, zsrDataSet.NUTR_DEFRowChangeEvent e)
        {
            string loge = string.Format("Nutrient {0}, {1},{2} updated.", e.Row.Nutr_No, e.Row.NutrDesc, e.Row.IsSelected);
            z_msg.Content = loge;
            ndda.UpdateQuery(e.Row.IsSelected, e.Row.Nutr_No);
            UpdateCountLabel();
        }

        private void UpdateCountLabel()
        {
            int selcount = 0;

            for (int i = 0; i < 146; i++)
            {
                if (foo.NUTR_DEF[i].IsSelected == true)
                    selcount++;
            }
            z_nut_count.Content = string.Format(" {0}/146 ", selcount);
        }

        void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            foo = (SR25b.zsrDataSet)this.FindResource("zsrDataSet");
            Browse();
            Nutdf();
            foo.NUTR_DEF.NUTR_DEFRowChanged += new zsrDataSet.NUTR_DEFRowChangeEventHandler(NUTR_DEF_NUTR_DEFRowChanged);
            MakeResTab();
            MakePanResTab();
            UpdateCountLabel();
            ResGrid.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(ResGrid_AutoGeneratingColumn);
        }

        void ResGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string strfmt = null;
            e.Column.IsReadOnly = true;
            if (e.Column.Header.ToString().StartsWith("Values") == true)
            {
                DataGridTextColumn dgc = e.Column as DataGridTextColumn;
                dgc.CellStyle = (Style)this.FindResource("Cells");
                strfmt = StrFormat(decp);
                dgc.Binding.StringFormat = strfmt;
                //e.Column.Width = 50;
            }
            else
            {
                //e.Column.Width = 600;
            }
        }

        private void MakePanResTab()
        {
            DataColumn c1 = new DataColumn("Values", typeof(string));
            DataColumn c2 = new DataColumn("Foods", typeof(string));
            PanRes.Columns.Add(c1);
            PanRes.Columns.Add(c2);
        }

        /// <summary>
        /// Browsing all groups/foods.
        /// </summary>
        private void Browse()
        {
            Group();
            Foods();
        }
        /// <summary>
        /// Fill The NUTR_DEF Datatable form ACCESS DB
        /// </summary>
        private void Nutdf()
        {
            ndda.Fill(foo.NUTR_DEF);
            Dcvs = (CollectionViewSource)(this.FindResource("nUTR_DEFViewSource"));
            Dcvs.View.MoveCurrentToFirst();
        }
        /// <summary>
        /// Fill The FOOD_DES Datatable form ACCESS DB
        /// </summary>
        private void Foods()
        {
            fdda.Fill(foo.FOOD_DES);
            Foo2Vsrc();
            ShowFoodHeader();

        }
        /// <summary>
        /// link table to CollectionViewSource
        /// </summary>
        private void Foo2Vsrc()
        {
            Fcvs = (CollectionViewSource)(this.FindResource("fD_GROUPFOOD_DESViewSource"));
            if (Fcvs != null && Fcvs.View != null)
                Fcvs.View.MoveCurrentToFirst();
            
           

        }
        /// <summary>
        /// Fill The FD_GROUPS Datatable form ACCESS DB
        /// </summary>
        private void Group()
        {
            fgda.Fill(foo.FD_GROUP);
            G2Vsrc();
        }
        /// <summary>
        /// Link up the with CollectionViewSource
        /// </summary>
        private void G2Vsrc()
        {
            Gcvs = (CollectionViewSource)(this.FindResource("fD_GROUPViewSource"));
            if (Gcvs != null && Gcvs.View != null)
                Gcvs.View.MoveCurrentToFirst();
        }
        /// <summary>
        /// Wild Card search.. its %keyword%
        /// </summary>
        private void Search()
        {

            if (string.IsNullOrWhiteSpace(Wild.Text) == true)
            {
                Browse();
                return;
            }

            string wcard = string.Format("%{0}%", Wild.Text);


            if (fgda.ClearBeforeFill == false)
            {
                foo.FD_GROUP.Rows.Clear();
                foo.FOOD_DES.Rows.Clear();
            }

            fgda.Search(foo.FD_GROUP, wcard);
            G2Vsrc();

            fdda.Search(foo.FOOD_DES, wcard);
            Foo2Vsrc();
        }
        /// <summary>
        /// store the selected food in the cmp listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void z_add_Click(object sender, RoutedEventArgs e)
        {
            AddItem2cmp();
        }
        /// <summary>
        /// remove an item from the cmp ListBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void z_del_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedItem();
        }

        private void RemoveSelectedItem()
        {
            if (cmp.SelectedItem == null)
                return;
            //if (TheResTab.Rows.Count <= 0)
            //    return;
            ListBoxItem lbi = cmp.SelectedItem as ListBoxItem;
            footag  sr = (footag)lbi.Tag;
            cmp.Items.Remove(lbi);
            //Remove Item from Tempres
            TempresRem(sr);
            // delete  from results table.
            for (int i = 0; i < TheResTab.Rows.Count; i++)
            {
                if (sr.Desc.Equals(TheResTab.Rows[i][0].ToString()) == true)
                {
                    TheResTab.Rows[i].Delete();
                }

            }
            CmpListHasItems();



        }
        /// <summary>
        /// Clear cmp and all TheResTab 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void z_clear_Click(object sender, RoutedEventArgs e)
        {
            ClearWorkspace();
        }

        private void ClearWorkspace()
        {
            cmp.Items.Clear();
            TheResTab.Rows.Clear();
            PanRes.Rows.Clear();
            CmpListHasItems();
        }
        /// <summary>
        /// Load all data from ACCES for the selected foods
        /// and display in datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void z_comp_Click(object sender, RoutedEventArgs e)
        {
            Compare();
        }


        private void Compare()
        {

            if (cmp.Items.Count <= 0)
                return;
            z_resLab.Content = string.Format("Comparing {0} foods", cmp.Items.Count);
            //Delegate for the progressbar dispatcher.
            u4progbar updateprog = new u4progbar(UpdateProBar);
            // the progress steps from the iterations number.
            double pv = (double)((double)100.0 / (double)cmp.Items.Count);

            ShowProg();

            footag  fr = null;
            TheResTab.Rows.Clear();

            for (int i = 0; i < cmp.Items.Count; i++)
            {
                ListBoxItem lbi = (ListBoxItem)cmp.Items[i];
                z_prog.Dispatcher.Invoke(DispatcherPriority.Render, updateprog, pv);

                fr = lbi.Tag as footag ;

                AddFoodRow(fr);
            }

            HideProg();
            ActionBtnsHide(true);

        }
        /// <summary>
        /// Hide progressbar 
        /// and show result tab and compare button
        /// </summary>
        private void HideProg()
        {

            z_p2.Visibility = System.Windows.Visibility.Visible;
            z_comp.Visibility = System.Windows.Visibility.Visible;
            z_prog.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// shows progress bar and hide
        /// results tab and compare button.
        /// </summary>
        private void ShowProg()
        {
            z_prog.IsIndeterminate = false;
            z_prog.Visibility = System.Windows.Visibility.Visible;
            z_p2.Visibility = System.Windows.Visibility.Hidden;
            z_comp.Visibility = System.Windows.Visibility.Hidden;
        }

        private void UpdateProBar(double pv)
        {
            z_prog.Value += pv;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void z_go_Click(object sender, RoutedEventArgs e)
        {
            //HACK: Way does it Crash!!!!!!!!!! when mixed with Browse.!!!
            Search();
        }


        /// <summary>
        /// Make a new table and loose all datagrid data.
        /// need to rebuild table::::HACK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void z_updatedef_Click(object sender, RoutedEventArgs e)
        {
            MakeResTab();
        }

        private void z_res_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.IsReadOnly = true;
            if (e.Column.Header.ToString().StartsWith("Selected") == true)
            {
                e.Column.HeaderStyle = (Style)this.FindResource("Column1HeaderStyle");
                DataGridTextColumn dgc = e.Column as DataGridTextColumn;

                dgc.CellStyle = (Style)this.FindResource("Col0");
            }
            else
            {
                e.Column.HeaderStyle = (Style)this.FindResource("NutrientColumns");

                string nutno = e.Column.Header.ToString();
                string colname = null;
                int dec = 0;
                string strfmt = null;

                foreach (SR25b.zsrDataSet.NUTR_DEFRow df in foo.NUTR_DEF)
                {
                    if (df.Nutr_No.Equals(nutno) == true)
                    {
                        colname = string.Format("{0}-({1})", df.NutrDesc, df.Units);
                        dec = df.Decimal;
                        break;
                    }
                }

                e.Column.Header = colname;
                DataGridTextColumn dgc = e.Column as DataGridTextColumn;

                dgc.CellStyle = (Style)this.FindResource("Cells");

                strfmt = StrFormat(dec);
                dgc.Binding.StringFormat = strfmt;
            }
        }

        private string StrFormat(int dec)
        {
            string strfmt = null;

            switch (dec)
            {
                case 1:
                    strfmt = "{0,14:F1}";
                    break;
                case 2:
                    strfmt = "{0,14:F2}";
                    break;
                case 3:
                    strfmt = "{0,14:F3}";
                    break;
                default:
                    strfmt = "{0,14:F0}";
                    break;

            }
            return strfmt;
        }

        private string Findef(string p)
        {
            string colname = null;
            int dec = 0;
            foreach (SR25b.zsrDataSet.NUTR_DEFRow df in foo.NUTR_DEF)
            {
                if (df.Nutr_No.Equals(p) == true)
                {
                    colname = string.Format("{0}-({1})", df.NutrDesc, df.Units);
                    dec = df.Decimal;
                    return colname;
                }
            }
            return null;
        }

        private void AddItem2cmp()
        {
            if (FooList.SelectedItem == null)
                return;
            DataRowView drv = FooList.SelectedItem as DataRowView;
            if (drv == null)
                return;
            SR25b.zsrDataSet.FOOD_DESRow fr = (SR25b.zsrDataSet.FOOD_DESRow)drv.Row;
            
            Add2cmpList(fr);
            CmpListHasItems();
        }

        private void Add2cmpList(zsrDataSet.FOOD_DESRow fr)
        {
            footag ft;
            foreach (ListBoxItem li in cmp.Items)
            {
                ft = li.Tag as footag;

                if (ft.NDB_No.Equals(fr.NDB_No) == true)
                {
                    MessageBox.Show("Items already in comparison table");
                    return;
                }
            }
            ListBoxItem lbi = new ListBoxItem();
            
            
            lbi.Tag = new footag(fr.NDB_No,fr.Desc);
            lbi.Content = fr.Desc;
            cmp.Items.Add(lbi);
            TempresAdd(fr);
        }
        
      

        private void TempresAdd(zsrDataSet.FOOD_DESRow fr)
        {
            ddda.ClearBeforeFill = false;
            ddda.Nutrient4(Tempres, fr.NDB_No);
            ddda.ClearBeforeFill = true;

        }

        private void TempresRem(footag  fr)
        {
            zsrDataSet.NUT_DATARow dr = null;
            string sel = string.Format("NDB_NO = {0}", fr.NDB_No);
            DataRow[] sfdr = Tempres.Select(sel);
            foreach (DataRow ddr in sfdr)
            {
                dr = (zsrDataSet.NUT_DATARow)ddr;
                Tempres.Rows.Remove(dr);
            }

        }

        private void TempresClear()
        {
            Tempres.Rows.Clear();
        }

        private void AddFoodRow(footag  fr)
        {

            ddda.Nutrient4(foo.NUT_DATA, fr.NDB_No);


            DataRow dr = TheResTab.NewRow();

            float v = 0F;
            dr[0] = fr.Desc;


            for (int i = 1; i < TheResTab.Columns.Count; i++)
            {

                v = Findval(TheResTab.Columns[i].Caption);
                dr[i] = v;

            }

            TheResTab.Rows.Add(dr);

        }
        /// <summary>
        /// get the food value in the  NUT_DATA table 
        /// for the given nutrient 0F if not found.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private float Findval(string p)
        {
            foreach (SR25b.zsrDataSet.NUT_DATARow d in foo.NUT_DATA)
            {
                if (p.Equals(d.Nutr_No) == true)
                    return (float)d.Nutr_Val;
            }
            return 0F;
        }


        private void z_PanelComp(object sender, RoutedEventArgs e)
        {
            PanResults();
        }

        private void PanResults()
        {
            pres.Visibility = System.Windows.Visibility.Hidden;

            ResList.Items.Clear();

            foreach (SR25b.zsrDataSet.NUTR_DEFRow nd in foo.NUTR_DEF.Rows)
            {
                if (nd.IsSelected == false)
                    continue;
                ListBoxItem lb = new ListBoxItem();
                lb.Tag = nd;
                TextBlock t = new TextBlock();
                TextBlock t1 = new TextBlock();

                t.Text = nd.Units;
                t.Width = 50;
                t1.Text = nd.NutrDesc;
                t1.Width = 155;
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Children.Add(t);
                sp.Children.Add(t1);

                lb.Content = sp;
                ResList.Items.Add(lb);
            }
            ResListHead.Content = string.Format("( {0} )/( 146 ) Nutrients", ResList.Items.Count);

            pres.Visibility = System.Windows.Visibility.Visible;
            ActionBtnsHide(true);
        }

        private void ResList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRdg();
        }

        private void LoadRdg()
        {
            ListBoxItem lb = ResList.SelectedItem as ListBoxItem;
            if (lb == null)
                return;
            SR25b.zsrDataSet.NUTR_DEFRow nd = (SR25b.zsrDataSet.NUTR_DEFRow)lb.Tag;
            // could do with cleaning this as string StrFormat take this.decp
            this.decp = nd.Decimal;
            string strfmt = StrFormat(decp);
            string selection = string.Format("Nutr_No = {0}", nd.Nutr_No);
            DataRow[] r4dg = Tempres.Select(selection);
            PanRes.Rows.Clear();
            ResGrid.AutoGenerateColumns = true;
            ResGrid.Columns.Clear();
            ResGrid.ItemsSource = null;

            SR25b.zsrDataSet.NUT_DATARow drow = null;

            foreach (DataRow dr in r4dg)
            {
                drow = (SR25b.zsrDataSet.NUT_DATARow)dr;

                DataRow d0 = PanRes.NewRow();
                //d0[0] = (float)drow.Nutr_Val;
                d0[0] = string.Format(strfmt, drow.Nutr_Val);
                d0[1] = ffoodes(drow.NDB_No);
                PanRes.Rows.Add(d0);
            }
            //z_res.ItemsSource = TheResTab.DefaultView;

            ResGrid.ItemsSource = PanRes.DefaultView;

            ResGridHead.Content = string.Format("{0} Records Found", PanRes.Rows.Count);
            //DataGridTextColumn dgc = ResGrid.Columns[0] as DataGridTextColumn;
            ////dgc.HeaderStringFormat = strfmt;
            //string curfmt = dgc.Binding.StringFormat;

            //dgc.Binding.StringFormat  = strfmt;

            //ResGridHead.Content = string.Format("Decimal pt {0}, prev format is {1}, next format {2}", decp, curfmt, strfmt);

        }



        private string ffoodes(string p)
        {
            footag  drow = null;
            foreach (ListBoxItem lb in cmp.Items)
            {
                drow = (footag)lb.Tag;
                if (drow.NDB_No.Equals(p) == true)
                    return drow.Desc;
            }
            return null;
        }



        private void CmpListHasItems()
        {
            if (cmp.Items.Count > 1)
            {
                z_comp.Visibility = System.Windows.Visibility.Visible;
                z_comp1.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                z_comp.Visibility = System.Windows.Visibility.Collapsed;
                z_comp1.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ActionBtnsHide(bool hide)
        {
            if (hide == true)
            {
                z_add.Visibility = System.Windows.Visibility.Collapsed;
                z_del.Visibility = System.Windows.Visibility.Collapsed;
                z_clear.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                z_add.Visibility = System.Windows.Visibility.Visible;
                z_del.Visibility = System.Windows.Visibility.Visible;
                z_clear.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void z_p2_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void z_new_Click(object sender, RoutedEventArgs e)
        {
            ClearWorkspace();
            ActionBtnsHide(false);
            pres.Visibility = System.Windows.Visibility.Hidden;
            z_p2.Visibility = System.Windows.Visibility.Hidden;

        }

        private void grpList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowFoodHeader();
        }

        private void ShowFoodHeader()
        {
            Desc.Header = string.Format("Food List ( {0} )", FooList.Items.Count);
        }


        /*******************  *******************/


    }
}
