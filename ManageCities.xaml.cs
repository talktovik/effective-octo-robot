using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QM.Com.dl;
using QM.Com.Poco;
using QM.Com.Utility;
using System.ComponentModel;

namespace QM
{
    /// <summary>
    /// Interaction logic for ManageCities.xaml
    /// </summary>
    public partial class ManageCities : Window
    {
        public ManageCities()
        {
            InitializeComponent();
            populateCities();
           
        }

        private void populateCities()
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetAllCities();
            if (res.success)
            {
                List<City> cities = (List<City>)res.body;
                gridCities.ItemsSource = cities;
                Utility.MakeAllColumnsWidthSame(gridCities);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void gridCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                City city = (City)gridCities.SelectedItem;
                if (city == null) return;
                txtCity.Text = city.cityName;
                txtState.Text = city.state;
                txtCountry.Text = city.country;
                txtPincode.Text = city.pincode;
                txtStdCode.Text = city.stdCode;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }



        private void btnSaveCity_Click(object sender, RoutedEventArgs e)
        {
            if ( gridCities.SelectedIndex == -1 ) return;
            DataLayer dl = new DataLayer();
            City city = new City();
            city.cityName = txtCity.Text;
            city.pincode = txtPincode.Text;
            city.state = txtState.Text;
            city.stdCode = txtStdCode.Text;
            city.country = txtCountry.Text;
            Response res = dl.EditCity(city);
            if (res.success)
            {
                txtCity.Text = "";
                txtCountry.Text = "";
                txtPincode.Text = "";
                txtState.Text = "";
                txtStdCode.Text = "";
                System.Threading.Thread.Sleep(1000);
                populateCities();
            }
            else if(res.isException) {
                MessageBox.Show(res.exception);
            }


        }

        private void btnDeleteCity_Click(object sender, RoutedEventArgs e)
        {
            if (gridCities.SelectedIndex == -1) {
                return;
            }
            DataLayer dl = new DataLayer();
            Response res = dl.DeleteCity(txtCity.Text);
            if (res.success)
            {
                txtCity.Text = "";
                txtPincode.Text = "";
                txtState.Text = "";
                txtStdCode.Text = "";
                txtCountry.Text = "";
                System.Threading.Thread.Sleep(1000);
                populateCities();
            }
            else if (res.isException) {
                MessageBox.Show(res.exception);
            }

        }

        private void gridCities_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }

        private void gridCities_Loaded(object sender, RoutedEventArgs e)
        {
            Utility.MakeAllColumnsWidthSame(gridCities);
        }
    }
}
