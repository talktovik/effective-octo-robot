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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using QM.Com.Poco;
using QM.Com.dl;
using QM.Com.exception;
using QM.Com.Utility;
using System.Threading;
using QM.Com.Doc;
using QM.Com.qClass;
using FluentEmail.Smtp;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
//



namespace QM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Because Our Companies have several Addresses.
        private AddressBuffer addressBuffer;


        /// <summary>
        /// Here our Constructor Goes. it Always runs No Matter what happens.
        /// </summary>
        public MainWindow()
        {

            InitializeComponent();
            populateStatesCmb();
            populateCompaniesCmb();
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            setCompanyViewMode();
            setViewAddressMode();
            if (cmbCompanies.Items.Count > 0) cmbCompanies.SelectedIndex = 0;
            if (cmbPersonone.Items.Count > 0) cmbPersonone.SelectedIndex = 0;
            populateQuotations();
            populateTemplates();
            populateBrochure();
            PopulateEmail();
            refrenceNumberManager();
            populateExtraFiles();

        }


        /// <summary>
        /// This will take values from database and populate states in combo box
        /// </summary>
        private void populateStatesCmb()
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetStates();
                if (res.success)
                {
                    List<State> states = (List<State>)res.body;
                    cmbState.ItemsSource = states;
                    cmbState.DisplayMemberPath = "stateName";
                }
                else if (res.isException)
                {
                    throw new DAOException(res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception occured in populating states " + ex.Message);
            }
        }

        /// <summary>
        /// This is a helper method to get state object by stateName
        /// </summary>
        /// <param name="stateName"></param>
        private void selectStateByStateName(string stateName)
        {
            int Selected = -1;
            int count = cmbState.Items.Count;
            for (int i = 0; (i <= (count - 1)); i++)
            {
                cmbState.SelectedIndex = i;

                if (((State)(cmbState.SelectedItem)).stateName == stateName)
                {
                    Selected = i;
                    break;
                }
            }
            txtStateCode.Text = getStateCode();
        }

        /// <summary>
        /// This is helper method to get current selected state in combobox.
        /// </summary>
        /// <returns></returns>
        public State getSelectedState()
        {
            return (State)(cmbState.SelectedItem);
        }

        /// <summary>
        /// It will take companies values from database and populate companies >> combobox
        /// </summary>
        private void populateCompaniesCmb()
        {
            try
            {
                DataLayer dl = new DataLayer();
                Response res = dl.GetAllCompanies();
                if (res.success)
                {
                    List<Company> companies = (List<Company>)res.body;
                    cmbCompanies.ItemsSource = null;
                    // how about we can change in companies pojo, and add and show whatever we want
                    cmbCompanies.ItemsSource = companies;
                    cmbCompanies.DisplayMemberPath = "companyNameToShow";
                    cmbCompanies.SelectionChanged += cmbCompaniesSelectionChanged;
                }
                else if (res.isException)
                {
                    MessageBox.Show("Exception while populating companies : " + res.exception);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception in populate companies cmb : " + exception);
            }
        }

        //****************************Have to do this with several Comboboxes**************

        /// <summary>
        /// It will call every time when selection in combo box changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCompaniesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Response res;
            DataLayer dl = new DataLayer();
            try
            {
                if (cmbCompanies.SelectedIndex == -1)
                {
                    emptyAllAddressFields();
                    gridContacts.ItemsSource = null;
                    return;
                }
                Company company = (Company)cmbCompanies.SelectedItem;
                //PopulateFilePaths();
                setAddressComponent(company.companyId);
                populateContacts(company.companyId);
                populateContactsCmb(company.companyId);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception while selecting company " + exception);
            }
        }

        /// <summary>
        /// It will populate address buffer according to company name
        /// </summary>
        /// <param name="companyId"></param>
        private void setAddressComponent(int companyId)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetAddressByCompanyId(companyId);
            if (res.success)
            {
                List<Address> addresses = (List<Address>)res.body;
                addressBuffer = new AddressBuffer(addresses);
                emptyAllAddressFields();
                if (addressBuffer.GetSize() != 0)
                {
                    Address address = addressBuffer.GetCurrentAddress();
                    FillAddress(address);
                }
                setAddressPanel();
            }
            else if (res.isException)
            {
                MessageBox.Show("Point 2 : " + res.exception);
            }
        }

        /// <summary>
        /// It will populate contacts Data grid according to particular company values from db
        /// </summary>
        /// <param name="companyId"></param>
        private void populateContacts(int companyId)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetContactsByCompanyId(companyId);
            if (res.success)
            {
                List<Contact> contacts = (List<Contact>)res.body;
                gridContacts.ItemsSource = contacts;

                //Because they are fetching extra data from the database.
                if (gridContacts.Columns.Count > 2)
                {
                    gridContacts.Columns[0].Visibility = Visibility.Collapsed;
                    gridContacts.Columns[1].Visibility = Visibility.Collapsed;
                }
                Utility.MakeAllColumnsWidthSame(gridContacts);
                if (gridContacts.Columns.Count >= 3) gridContacts.Columns[3].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                if (contacts.Count > 0) gridContacts.SelectedIndex = 0;
            }
            else if (res.isException)
            {
                MessageBox.Show("Point 3 : " + res.exception);
            }
        }

        /// <summary>
        /// It is helper method which will give current selected company object
        /// </summary>
        /// <returns></returns>
        public Company getCurrentSelectedCompany()
        {
            return (Company)cmbCompanies.SelectedItem;
        }

        /// <summary>
        /// This Function actually controls the visibility of the of the companies buttons 
        /// </summary>
        public void setCompanyAddMode()
        {
            cmbCompanies.Visibility = Visibility.Collapsed;
            txtCompanyName.Visibility = Visibility.Visible;
            txtCompanyName.Text = "";
            btnEditCompany.IsEnabled = false;
            btnDeleteCompany.IsEnabled = false;
            btnCancelCompany.IsEnabled = true;
        }

        /// <summary>
        /// It will make changes in UI according to update Mode
        /// </summary>
        public void setCompanyEditMode()
        {
            cmbCompanies.Visibility = Visibility.Collapsed;
            txtCompanyName.Visibility = Visibility.Visible;
            txtCompanyName.Text = ((Company)cmbCompanies.SelectedItem).companyName;
            btnAddCompany.IsEnabled = false;
            btnDeleteCompany.IsEnabled = false;
            btnCancelCompany.IsEnabled = true;
        }

        /// <summary>
        /// View Mode is default mode, when window start company is in view mode
        /// </summary>
        public void setCompanyViewMode()
        {
            cmbCompanies.Visibility = Visibility.Visible;
            txtCompanyName.Visibility = Visibility.Collapsed;
            btnAddCompany.IsEnabled = true;
            btnEditCompany.IsEnabled = true;
            btnDeleteCompany.IsEnabled = true;
            btnCancelCompany.IsEnabled = false;
            btnAddCompany.Content = "Add";
            btnEditCompany.Content = "Edit";
        }

        /// <summary>
        /// It will trigger when we Add button clicked below company cmb. In this firstly it will set company to add mode and make add button as OK button. Then call dl add function when click on ok button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddCompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            if (btn.Content.ToString() == "Add")
            {
                btn.Content = "OK";
                setCompanyAddMode();
            }
            else
            {

                if (txtCompanyName.Text == "" || txtCompanyName.Text.Length == 0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    setCompanyAddMode();
                    return;
                }
                DataLayer dl = new DataLayer();
                Company company = new Company();
                company.companyName = txtCompanyName.Text;
                Response res = dl.AddNewCompany(company);
                if (res.success)
                {
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    setCompanyViewMode();
                    btn.Content = "Add";
                    //lblCompanyStatus.Content = "Company Added Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// It will trigger when Edit button below company clicked, it will do same process as for add company function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditCompanyDetails_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            Button btn = e.Source as Button;
            if (btn.Content.ToString() == "Edit")
            {
                btn.Content = "OK";
                setCompanyEditMode();
            }
            else
            {
                if (txtCompanyName.Text == "" || txtCompanyName.Text.Length == 0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    return;
                }
                DataLayer dl = new DataLayer();
                Company company = new Company();
                company.companyId = ((Company)cmbCompanies.SelectedItem).companyId;
                company.companyName = txtCompanyName.Text;
                Response res = dl.EditCompany(company);
                if (res.success)
                {
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    setCompanyViewMode();
                    // lblCompanyStatus.Content = "Company updated successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// This Function have all the logic for its visibility!!!!
        /// </summary>
        void setViewAddressMode()
        {
            btnAddAddress.IsEnabled = true;
            btnSaveAddress.IsEnabled = true;
            btnDeleteAddress.IsEnabled = true;
            btnCancelAddress.IsEnabled = false;
            setAddressPanel();
            cmbCompanies.IsEnabled = true;
            btnAddAddress.Content = "Add";
        }
        /// <summary>
        /// again a model which is related to the view mode 
        /// </summary>
        public void setAddAddressMode()
        {
            btnAddAddress.IsEnabled = true;
            btnSaveAddress.IsEnabled = false;
            btnDeleteAddress.IsEnabled = false;
            btnCancelAddress.IsEnabled = true;
            addressBuffer.AddAddress(new Address());
            setAddressPanel();
            while (addressBuffer.canGetNext()) btnNext.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            cmbCompanies.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnAddAddress.Content = "Ok";
        }
        private void btnCancelAddressModification_Click(object sender, RoutedEventArgs e)
        {
            setViewAddressMode();
            addressBuffer.RemoveLast();
            setAddressPanel();
            if (addressBuffer.canGetPrev()) btnPrev.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        /// <summary>
        /// It will firstly set address to addAddressMode, and make button as OK button. When we click onn Ok button, it will call addAddress method in DL, by taking values from different address text fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAddressDetails_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            if (b.Content.ToString() == "Add")
            {
                setAddAddressMode();
            }
            else if (b.Content.ToString() == "Ok")
            {
                if (cmbCompanies.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select company first");
                    return;
                }
                DataLayer dl = new DataLayer();
                Address address = new Address();
                address.address1 = txtAddress1.Text;
                address.address2 = txtAddress2.Text;
                address.address3 = txtAddress3.Text;
                address.city = txtCity.Text;
                //address.GSTNo = txtGSTNo.Text;
                address.phone = txtPhone.Text;
                address.pincode = txtPincode.Text;
                address.state = getSelectedState().stateName;
                address.country = txtCountry.Text;
                if (txtStateCode.Text.Trim() != "") address.stateCode = Int32.Parse(txtStateCode.Text);

                address.companyID = ((Company)cmbCompanies.SelectedItem).companyId;
                Response res = dl.AddAddress(address);
                if (res.success)
                {
                    // address added
                    Thread.Sleep(1000);
                    setAddressComponent(getCurrentSelectedCompany().companyId);
                    setViewAddressMode();
                    populateCompaniesCmb();
                    //lblAddressStatus.Content = "Address Added Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// It will call save address function from DL by taking values from different address fields and also update address buffer details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAddressDetails_Click(object sender, RoutedEventArgs e)
        {
            if (addressBuffer == null)
            {
                MessageBox.Show("please select a address first");
                return;
            }
            Address oldAddress = addressBuffer.GetCurrentAddress();
            Address newAddress = new Address()
            {
                address1 = txtAddress1.Text,
                address2 = txtAddress2.Text,
                address3 = txtAddress3.Text,
                city = txtCity.Text,
                state = getSelectedState().stateName,
                country = txtCountry.Text,
                pincode = txtPincode.Text,
                phone = txtPhone.Text,
                //GSTNo = txtGSTNo.Text
            };
            //stateCode = (txtStateCode.Text == "") ? 0 : Int32.Parse(txtStateCode.Text);
            if (txtStateCode.Text.Trim() != "") newAddress.stateCode = Int32.Parse(txtStateCode.Text);
            newAddress.addressID = oldAddress.addressID;
            DataLayer dl = new DataLayer();
            Response res = dl.EditAddress(newAddress);
            if (res.success)
            {
                // address edited / saved successfully
                Thread.Sleep(1000);
                setAddressComponent(getCurrentSelectedCompany().companyId);
                populateCompaniesCmb();
                // lblAddressStatus.Content = "Address Updated Successfully";
            }
            else if (res.isException)
            {
                MessageBox.Show("Exception in saving address : " + res.exception);
            }
        }

        /// <summary>
        /// It will empty all the address fields.
        /// </summary>
        private void emptyAllAddressFields()
        {
            txtAddress1.Text = "";
            txtAddress2.Text = "";
            txtAddress3.Text = "";
            txtCity.Text = "";
            cmbState.SelectedIndex = -1;
            txtCountry.Text = "";
            txtPincode.Text = "";
            txtPhone.Text = "";
            // txtGSTNo.Text = "";
            txtStateCode.Text = "";
        }

        /// <summary>
        /// It is just a helper method, which will take address object and fill all address fields
        /// </summary>
        /// <param name="address"></param>
        private void FillAddress(Address address)
        {
            txtAddress1.Text = address.address1;
            txtAddress2.Text = address.address2;
            txtAddress3.Text = address.address3;
            txtCity.Text = address.city;
            selectStateByStateName(address.state);
            txtCountry.Text = address.country;
            //txtState.Text = address.state; Changes
            txtPincode.Text = address.pincode;
            txtPhone.Text = address.phone;
            // txtGSTNo.Text = address.GSTNo;
            txtStateCode.Text = getStateCode();
        }

        /// <summary>
        /// It will take prev address value from addressBuffer and call fill address for that address value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnPrevAddress_Click(object sender, RoutedEventArgs e)
        {
            // It will show previous address and change value on txtAddressInfo
            try
            {
                if (addressBuffer != null && addressBuffer.canGetPrev())
                {
                    FillAddress(addressBuffer.GetPreviousAddress());
                }
                setAddressPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will take next address from address buffer and call fill address for the same
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnNextAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (addressBuffer != null && addressBuffer.canGetNext())
                {
                    FillAddress(addressBuffer.GetNextAddress());
                }
                setAddressPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// It will set address panel, which is located between prev and next address button. This panel will show, out of total addresses from address which address you are currently in.
        /// </summary>
        private void setAddressPanel()
        {
            if (addressBuffer == null || addressBuffer.GetSize() == 0)
            {
                txtAddressInfo.Text = "0 / 0";
                btnPrev.IsEnabled = false;
                btnNext.IsEnabled = false;
            }
            else
            {
                txtAddressInfo.Text = addressBuffer.GetCurrentIndex() + " / " + addressBuffer.GetSize();
                if (addressBuffer != null && !addressBuffer.canGetPrev())
                    btnPrev.IsEnabled = false;
                else
                    btnPrev.IsEnabled = true;
                if (addressBuffer != null && !addressBuffer.canGetNext())
                    btnNext.IsEnabled = false;
                else
                    btnNext.IsEnabled = true;
            }
        }


        /// <summary>
        /// This function will automatically fill state, pincode, country and stateCode according to city name typed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCityFocusChanged_Click(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetCityByName(txtCity.Text);
            if (res.success)
            {
                City city = (City)res.body;
                selectStateByStateName(city.state);
                //txtState.Text = city.state;
                txtPincode.Text = city.pincode;
                txtPhone.Text = city.stdCode;
                txtCountry.Text = city.country;
                int stateCode = getSelectedState().stateCode;
                if (stateCode != 0) txtStateCode.Text = getStateCode();
            }
            else if (res.isException)
            {
                //MessageBox.Show("Exception while getting cities " + res.exception);
                // maybe city name does not exist in database
            }
        }

        private string getStateCode(string state)
        {
            string stateCode = "";
            DataLayer dl = new DataLayer();
            Response res = dl.GetStateCodeByName(state);
            if (res.success)
            {
                stateCode = (string)res.body;
            }
            else if (res.isException)
            {
                // state_code not found, maybe state name is not exist in database
            }
            return stateCode;
        }

        /// <summary>
        /// It will redirect to manage cities dailog box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManageCities_Click(object sender, RoutedEventArgs e)
        {
            ManageCities manageCities = new ManageCities();
            manageCities.ShowDialog();
        }

        /// <summary>
        /// It will take values from contact fields and save contact detais by calling function from DL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveContactDetails_Click(object sender, RoutedEventArgs e)
        {
            if (gridContacts.SelectedIndex == -1) return;
            Contact contact = new Contact();
            contact.contactId = ((Contact)gridContacts.SelectedItem).contactId;
            contact.contact_name = txtContactName.Text;
            contact.contact_email = txtContactEmail.Text;
            contact.contact_phone = txtContactPhone.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.EditContact(contact);
            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                //lblContactStatus.Content = "Saving...";
                Thread.Sleep(1000);
                //lblContactStatus.Content = "Saved";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        /// <summary>
        /// It will firstly ask to confirm deleting contact details, then it will delete contact details by calling DL function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteContactDetails_Click(object sender, RoutedEventArgs e)
        {
            if (gridContacts.SelectedIndex == -1) return;
            int contactId = ((Contact)gridContacts.SelectedItem).contactId;

            DataLayer dl = new DataLayer();
            Response res = dl.DeleteContact(contactId);
            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                //lblContactStatus.Content = "Deleting...";
                Thread.Sleep(1000);
                //lblContactStatus.Content = "Deleted";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }

        }

        /// <summary>
        /// It will fill text fields according to selection changed in data grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridContacts_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtSalutation.Text = "";
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                txtContactDesignation.Text = "";
                if (gridContacts.SelectedIndex == -1) return;
                Contact contact = (Contact)gridContacts.SelectedItem;
                txtSalutation.Text = contact.salutation;
                txtContactName.Text = contact.contact_name;
                txtContactEmail.Text = contact.contact_email;
                txtContactPhone.Text = contact.contact_phone;
                txtContactDesignation.Text = contact.designation;


            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception in contact selection changed " + exception.Message);
            }
        }

        /// <summary>
        /// This will run first time when grid contact initialized, and we have to hide some contact from our object, so we use this event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridContacts_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (gridContacts != null && gridContacts.Columns.Count > 0)
            {
                gridContacts.Columns[0].Visibility = Visibility.Collapsed;
                gridContacts.Columns[1].Visibility = Visibility.Collapsed;
                Utility.MakeAllColumnsWidthSame(gridContacts);
                if (gridContacts.Columns.Count >= 5) gridContacts.Columns[5].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            }
        }

        /// <summary>
        /// It will take contact details from text fields and add to contacts table by calling appropriate DL function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            Contact contact = new Contact();
            contact.companyId = ((Company)cmbCompanies.SelectedItem).companyId;
            contact.contact_name = txtContactName.Text;
            contact.contact_email = txtContactEmail.Text;
            contact.contact_phone = txtContactPhone.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.AddContact(contact);
            Trace.WriteLine(res.success);
            Trace.WriteLine(res.isException);
            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                //lblContactStatus.Content = "Adding...";
                Thread.Sleep(1000);
                //lblContactStatus.Content = "Added";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }



        private void gridContacts_AutoGeneratingColumn_1(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }



        /// <summary>
        /// it will take currently selected state value and set stateCode accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbState_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (getSelectedState() == null) return;
            txtStateCode.Text = getStateCode();
        }

        private string getStateCode()
        {
            string stateCode = getSelectedState().stateCode.ToString();
            if (stateCode.Length == 1) stateCode = "0" + stateCode;
            return stateCode;
        }



        private void cmbCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            if (btn.Content.ToString() == "Add")
            {
                btn.Content = "OK";
                setCompanyAddMode();
            }
            else
            {

                if (txtCompanyName.Text == "" || txtCompanyName.Text.Length == 0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    setCompanyAddMode();
                    return;
                }
                DataLayer dl = new DataLayer();
                Company company = new Company();
                company.companyName = txtCompanyName.Text;
                Response res = dl.AddNewCompany(company);
                if (res.success)
                {
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    setCompanyViewMode();
                    btn.Content = "Add";
                    //lblCompanyStatus.Content = "Company Added Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// This is the Refresh Button on the top of the Form which repopulate the companies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            populateCompaniesCmb();
            
        }

        private void btnAddAddress_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            if (b.Content.ToString() == "Add")
            {
                setAddAddressMode();
            }
            else if (b.Content.ToString() == "Ok")
            {
                if (cmbCompanies.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select company first");
                    return;
                }
                DataLayer dl = new DataLayer();
                Address address = new Address();
                address.address1 = txtAddress1.Text;
                address.address2 = txtAddress2.Text;
                address.address3 = txtAddress3.Text;
                address.city = txtCity.Text;
                address.GSTNo = "0";
                address.phone = txtPhone.Text;
                address.pincode = txtPincode.Text;
                address.state = getSelectedState().stateName;
                address.country = txtCountry.Text;
                if (txtStateCode.Text.Trim() != "") address.stateCode = Int32.Parse(txtStateCode.Text);

                address.companyID = ((Company)cmbCompanies.SelectedItem).companyId;
                Response res = dl.AddAddress(address);
                if (res.success)
                {
                    // address added
                    Thread.Sleep(1000);
                    setAddressComponent(getCurrentSelectedCompany().companyId);
                    setViewAddressMode();
                    populateCompaniesCmb();
                    //lblAddressStatus.Content = "Address Added Successfully";
                    MessageBox.Show("Address Added SuccessFully");
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        private void btnSaveAddress_Click(object sender, RoutedEventArgs e)
        {
            if (addressBuffer == null)
            {
                MessageBox.Show("please select a address first");
                return;
            }
            Address oldAddress = addressBuffer.GetCurrentAddress();
            Address newAddress = new Address()
            {
                address1 = txtAddress1.Text,
                address2 = txtAddress2.Text,
                address3 = txtAddress3.Text,
                city = txtCity.Text,
                state = getSelectedState().stateName,
                country = txtCountry.Text,
                pincode = txtPincode.Text,
                phone = txtPhone.Text,
                GSTNo = "0",
            };
            //stateCode = (txtStateCode.Text == "") ? 0 : Int32.Parse(txtStateCode.Text);
            if (txtStateCode.Text.Trim() != "") newAddress.stateCode = Int32.Parse(txtStateCode.Text);
            newAddress.addressID = oldAddress.addressID;
            DataLayer dl = new DataLayer();
            Response res = dl.EditAddress(newAddress);
            if (res.success)
            {
                // address edited / saved successfully
                Thread.Sleep(1000);
                setAddressComponent(getCurrentSelectedCompany().companyId);
                populateCompaniesCmb();
                // lblAddressStatus.Content = "Address Updated Successfully";
            }
            else if (res.isException)
            {
                MessageBox.Show("Exception in saving address : " + res.exception);
            }
        }


        /// <summary>
        /// btn Delete address 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAddress_Click_1(object sender, RoutedEventArgs e)
        {
            City city = new City();
            city.cityName = txtCity.Text;
            city.state = getSelectedState().stateName;
            city.pincode = txtPincode.Text;
            city.stdCode = txtPhone.Text;
            city.country = txtCountry.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.AddCity(city);
            if (res.success)
            {
                // city addded
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void btnCancelAddress_Click(object sender, RoutedEventArgs e)
        {
            setViewAddressMode();
            addressBuffer.RemoveLast();
            setAddressPanel();
            if (addressBuffer.canGetPrev()) btnPrev.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        /// <summary>
        /// Edit the company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditCompany_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            Button btn = e.Source as Button;
            if (btn.Content.ToString() == "Edit")
            {
                btn.Content = "OK";
                setCompanyEditMode();
            }
            else
            {
                if (txtCompanyName.Text == "" || txtCompanyName.Text.Length == 0)
                {
                    MessageBox.Show("Company name cannot be empty");
                    return;
                }
                DataLayer dl = new DataLayer();
                Company company = new Company();
                company.companyId = ((Company)cmbCompanies.SelectedItem).companyId;
                company.companyName = txtCompanyName.Text;
                Response res = dl.EditCompany(company);
                if (res.success)
                {
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    setCompanyViewMode();
                    // lblCompanyStatus.Content = "Company updated successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }

        }

        /// <summary>
        /// Delete any existing company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Confirm Delete : " + ((Company)cmbCompanies.SelectedItem).companyName, "Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DataLayer dl = new DataLayer();
                Response res = dl.DeleteCompany(((Company)cmbCompanies.SelectedItem).companyId);
                if (res.success)
                {
                    //company deleted
                    Thread.Sleep(1000);
                    populateCompaniesCmb();
                    //lblCompanyStatus.Content = "Company Deleted Successfully";
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
        }

        /// <summary>
        /// This would be the Cancel if you cnbaged the mind 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelCompany_Click_1(object sender, RoutedEventArgs e)
        {
            setCompanyViewMode();
        }


        /// <summary>
        /// This is for adding the cities
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddCities_Click(object sender, RoutedEventArgs e)
        {
            City city = new City();
            city.cityName = txtCity.Text;
            city.state = getSelectedState().stateName;
            city.pincode = txtPincode.Text;
            city.stdCode = txtPhone.Text;
            city.country = txtCountry.Text;

            DataLayer dl = new DataLayer();
            Response res = dl.AddCity(city);
            if (res.success)
            {
                // city addded
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void btnManageCities_Click_1(object sender, RoutedEventArgs e)
        {
            ManageCities manageCities = new ManageCities();
            manageCities.ShowDialog();
        }


        /// <summary>
        /// This is the prev Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            // It will show previous address and change value on txtAddressInfo
            try
            {
                if (addressBuffer != null && addressBuffer.canGetPrev())
                {
                    FillAddress(addressBuffer.GetPreviousAddress());
                }
                setAddressPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// This is simply the Next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (addressBuffer != null && addressBuffer.canGetNext())
                {
                    FillAddress(addressBuffer.GetNextAddress());
                }
                setAddressPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// __________________________________________________________________________________________________________BUG 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddContact_Click_1(object sender, RoutedEventArgs e)
        {

        }

        //Now I Have to do something with Reference number
        public int refrenceNumberManager()
        {

            int valueFromthedatabase = 0;
            DataLayer dl = new DataLayer();
            Response res = dl.countTheQuotations();
            if (res.success)
            {
                //if we are in this module that we have to say we have the count data of the table 
                valueFromthedatabase = (int)res.body;
                string sendername = txtSenderName.Text;
                txtrefno.Text = valueFromthedatabase.ToString() + sendername;
            }
            return valueFromthedatabase;
        }




        /// <summary>
        /// This would save the contact if you edit anything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnsaveContact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gridContacts.SelectedIndex == -1) return;
                Contact contact = new Contact();
                contact.contactId = ((Contact)gridContacts.SelectedItem).contactId;
                contact.contact_name = txtContactName.Text;
                contact.contact_email = txtContactEmail.Text;
                contact.contact_phone = txtContactPhone.Text;
                contact.designation = txtContactDesignation.Text;
                contact.salutation = txtSalutation.Text;

                DataLayer dl = new DataLayer();
                Response res = dl.EditContact(contact);
                if (res.success)
                {
                    txtContactName.Text = "";
                    txtContactEmail.Text = "";
                    txtContactPhone.Text = "";
                    txtContactDesignation.Text = "";
                    txtSalutation.Text = "";
                    //lblContactStatus.Content = "Saving...";
                    Thread.Sleep(1000);
                    //lblContactStatus.Content = "Saved";
                    populateContacts(getCurrentSelectedCompany().companyId);
                }
                else if (res.isException)
                {
                    MessageBox.Show(res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// This Would Add the contact of the selected Company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddContact_Click_2(object sender, RoutedEventArgs e)
        {
            if (cmbCompanies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a company first");
                return;
            }
            Contact contact = new Contact();
            contact.companyId = ((Company)cmbCompanies.SelectedItem).companyId;
            contact.contact_name = txtContactName.Text;
            contact.contact_email = txtContactEmail.Text;
            contact.contact_phone = txtContactPhone.Text;
            contact.designation = txtContactDesignation.Text;
            contact.salutation = txtSalutation.Text;
            DataLayer dl = new DataLayer();
            Response res = dl.AddContact(contact);

            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                txtContactDesignation.Text = "";
                txtSalutation.Text = "";
                //lblContactStatus.Content = "Adding...";
                Thread.Sleep(1000);
                //lblContactStatus.Content = "Added";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
            //Write someLogic So that people won't do click the button twice

            populateContactsCmb(contact.companyId);
        }


        /// <summary>
        /// This would Delete the Contact details whatever is selected on the Grid 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteContact_Click(object sender, RoutedEventArgs e)
        {
            if (gridContacts.SelectedIndex == -1) return;
            int contactId = ((Contact)gridContacts.SelectedItem).contactId;

            DataLayer dl = new DataLayer();
            Response res = dl.DeleteContact(contactId);
            if (res.success)
            {
                txtContactName.Text = "";
                txtContactEmail.Text = "";
                txtContactPhone.Text = "";
                txtSalutation.Text = "";
                txtContactDesignation.Text = "";
                //lblContactStatus.Content = "Deleting...";
                Thread.Sleep(1000);
                //lblContactStatus.Content = "Deleted";
                populateContacts(getCurrentSelectedCompany().companyId);
            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void txtCity_LostFocus(object sender, RoutedEventArgs e)
        {
            DataLayer dl = new DataLayer();

            Response res = dl.GetCityByName(txtCity.Text);
            if (res.success)
            {
                City city = (City)res.body;
                selectStateByStateName(city.state);
                //txtState.Text = city.state;
                txtPincode.Text = city.pincode;
                txtPhone.Text = city.stdCode;
                txtCountry.Text = city.country;
                int stateCode = getSelectedState().stateCode;
                if (stateCode != 0) txtStateCode.Text = getStateCode();
            }
            else if (res.isException)
            {
                //MessageBox.Show("Exception while getting cities " + res.exception);
                // maybe city name does not exist in database
            }

        }

        private void btnMakeQuotation_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTemplateName.SelectedIndex == -1)
            {
                MessageBox.Show("Without Template You can't Send a template. SORRY");
                MessageBox.Show("Should I reStart Now ??");
                System.Environment.Exit(0);

            }
            else
            {
                object Filename = (object)getCurrentTemplate();
                object endfile = Endfilemaker();
                string abcd = (string)endFileinPDF();
                string ad = (string)Endfilemaker();

                //Creating the doc file.
                CreateWordDocument(Filename, endfile);
                SaveTheQuotationdetails();
                populateQuotations();
                refrenceNumberManager();
                populateCompaniesCmb();
               
                //populateContacts();
               
            }



        }

        private void populateQuotations()
        {
            DataLayer dl = new DataLayer();
            Response res = dl.GetAllQuotations();
            if (res.success)
            {
                List<Quotation> quotations = (List<Quotation>)res.body;
                gridQuoation.ItemsSource = quotations;
                //gridContacts.DisplayMemberPath = "companyName";
                Utility.MakeAllColumnsWidthSame(gridQuoation);
                //if (gridQuoation.Columns.Count > 2)
                //{
                //    gridQuoation.Columns[1].Visibility = Visibility.Collapsed;
                //    gridQuoation.Columns[2].Visibility = Visibility.Collapsed;
                //    gridQuoation.Columns[0].Visibility = Visibility.Collapsed;

                //}

                if (gridQuoation != null && gridQuoation.Columns.Count > 0)
                {
                    gridQuoation.Columns[4].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[5].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[6].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[7].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[8].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[9].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[10].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[11].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[12].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[13].Visibility = Visibility.Collapsed;
                    gridQuoation.Columns[14].Visibility = Visibility.Collapsed;
                    Utility.MakeAllColumnsWidthSame(gridQuoation);
                    if (gridContacts.Columns.Count >= 5) gridContacts.Columns[5].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                }
            }
            else if (res.isException)
            {
                MessageBox.Show("Error in Popukating the Grid " + res.exception);
            }

        }
        public Contact getPersonone()
        {
            if (cmbPersontwo.SelectedIndex == -1)
            {
                Contact contact = new Contact();
                contact.contactId = 0;
                contact.companyId = 0;
                contact.contact_email = "";
                contact.contact_phone = "";
                contact.designation = "";
                contact.salutation = "";
                contact.contact_name = "";
                return contact;

            }
            else
            {
                return (Contact)cmbPersonone.SelectedItem;
            }
        }

        public Contact getPersontwo()
        {
            if (cmbPersontwo.SelectedIndex == -1)
            {
                Contact contact = new Contact();
                contact.contactId = 0;
                contact.contact_name = "";
                contact.contact_email = "";
                contact.contact_phone = "";
                contact.companyId = 0;
                contact.designation = "";
                contact.salutation = "";
                return contact;
            }
            else
            {
                return (Contact)cmbPersontwo.SelectedItem;
            }
        }


        /// <summary>
        /// This would Help in data Migration Like If We can do something Taking the current context of data there.
        /// </summary>
        /// <returns></returns>
        public Quotation dataMigrationThing()
        {
            DataLayer dl = new DataLayer();

            Quotation quotation = new Quotation();
            quotation.companyId = getCurrentSelectedCompany().companyId;
            quotation.companyName = getCurrentSelectedCompany().companyName;
            quotation.contactNameone = getPersonone().contact_name;
            quotation.firstMail = getPersonone().contact_email;
            quotation.contactNametwo = "";
            quotation.secondMail = "";
            quotation.senderName = txtSenderName.Text;
            quotation.referenceID = txtrefno.Text;
            quotation.templatepath = getCurrentTemplate();
            Response res = dl.MaketheActualQuotation(quotation);
            return quotation;

        }
        public void PopulatetheWordDoc(Quotation quotation)
        {

        }

        public void populateTemplates()
        {
           
            string dir = @"..\..\..\QM\Resources\Templates";
            foreach (string file in System.IO.Directory.GetFiles(dir))
            {
                cmbTemplateName.Items.Add(System.IO.Path.GetFileName(file));
            }

        }
        public string getCurrentTemplate()
        {
            string vari = (string)cmbTemplateName.SelectedItem;
            string fileName = (string)System.IO.Path.GetFileName(vari);
            string trickpart = @"..\..\..\QM\Resources\Templates" + "\\" + fileName;
            //MessageBox.Show(trickpart);
            return trickpart;


        }

        //writing the logic to delete!
        private void btnDeleteQuotation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Quotation quotation = new Quotation();
                quotation = getCurrentSelectedQuotationFromGrid();
                DataLayer dl = new DataLayer();
                Response res = dl.deleteQuotation(quotation);
                if (res.success)
                {
                    MessageBox.Show("Quotation Deleted");
                    setAddAddressMode();

                }
                else if (res.isException)
                {
                    throw new Exception(res.exception);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured which is " + ex.Message);
            }


        }
        private object RevisionEndDileMAkerInPDF() {
            string pathname = @"..\..\..\QM\Resources\RevisedQuotationinPdf";
            string companyname = getCurrentSelectedQuotationFromGrid().companyName;
            string companyName = companyname.Replace(" ", "");
            string companynamepath = pathname +"\\"+ companyName;
            int prev = getCurrentSelectedQuotationFromGrid().revisionID;
            prev = prev + 1;
            string modifier = getCurrentSelectedQuotationFromGrid().referenceID +  "-" + prev.ToString()+" "+ companyname  ; 
            // if the Directory not exist then Create it.
            if (!Directory.Exists(companynamepath))
            {
                Directory.CreateDirectory(companynamepath);
            }
            return (object)(companynamepath + "\\"  + modifier + ".pdf");


        }
        private object RevisionEndFileMaker() {
        string pathname = @"..\..\..\QM\Resources\revisedQuotations\";
            string companyname = getCurrentSelectedQuotationFromGrid().companyName;
            string companyName = companyname.Replace(" ", "");
            string companypath = pathname + companyName;
            string name = getCurrentSelectedQuotationFromGrid().referenceID + getCurrentSelectedQuotationFromGrid().senderName;
            // if the Directory not exist then Create it.
            if (!Directory.Exists(companypath))
            {
                Directory.CreateDirectory(companypath);
            }
            return companypath + "\\" + name +  ".docx";
        }
        private object Endfilemaker()
        {
            string pathname = @"..\..\..\QM\Resources\Quotations";
            string companyname = getCurrentSelectedCompany().companyName;
            string refnumber = txtrefno.Text;
            string sender = txtSenderName.Text;
            string modifier = refnumber + sender;
            return (object)(pathname + "\\" + modifier + companyname + ".docx");


        }
        private object endFileinPDF()
        {
            string pathname = @"..\..\..\QM\Resources\QuotationsinPDF\";
            string companyname = getCurrentSelectedCompany().companyName;
            string companyName = companyname.Replace(" ", "");
            string refrenceNUmber = txtrefno.Text;
            string sender = txtSenderName.Text;
            string companynamepath = pathname + companyName;
            // if the Directory not exist then Create it.
            if (!Directory.Exists(companynamepath))
            {
                Directory.CreateDirectory(companynamepath);
            }
            return (object)(pathname + companyName + "\\" + refrenceNUmber + " "+ companyname  + ".pdf");
        }

        private void CreateWordDocument(object filename, object SaveAs)
        {
            string modified = txtrefno.Text + txtSenderName.Text;
            Word.Application wordApp = new Word.Application();
            object missing = Missing.Value;
            Word.Document myWordDoc = null;

            if (File.Exists((string)filename))
            {
                object readOnly = false;
                object isVisible = false;
                wordApp.Visible = false;

                myWordDoc = wordApp.Documents.Open(ref filename, ref missing, ref readOnly,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing, ref missing);
                myWordDoc.Activate();

                
                //find and replace
                this.FindAndReplace(wordApp, "<COMPANYNAME>", getCurrentSelectedCompany().companyName);
                this.FindAndReplace(wordApp, "<ADDRESS1>", txtAddress1.Text);
                this.FindAndReplace(wordApp, "<ADDRESS2>", txtAddress2.Text);
                this.FindAndReplace(wordApp, "<ADDRESS3>", txtAddress3.Text);
                this.FindAndReplace(wordApp, "<PHONE NUMBER>", txtPhone.Text);
                this.FindAndReplace(wordApp, "<REFRENCENUMBER>", txtrefno.Text);
                this.FindAndReplace(wordApp, "<DATE>", txtDate.Text);
                //this.FindAndReplace(wordApp, "SENDERNAME", txtSenderName.Text);
                this.FindAndReplace(wordApp, "<YOURENQUIRY>", txtYourEnquiry.Text);
                this.FindAndReplace(wordApp, "<PERSON1>", getPersonone().contact_name);
                this.FindAndReplace(wordApp, "<PERSON2>", getPersontwo().contact_name);
            }
            else
            {
                MessageBox.Show("File not Found!");
            }

            //Save as means the filename(string)
            myWordDoc.SaveAs2(ref SaveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing);

            myWordDoc.Close();
            wordApp.Quit();
            //MessageBox.Show("File Created!");
        }
        private void FindAndReplace(Word.Application wordApp, object ToFindText, object replaceWithText)
        {
            object matchCase = true;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundLike = false;
            object nmatchAllforms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiactitics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;

            wordApp.Selection.Find.Execute(ref ToFindText,
                ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundLike,
                ref nmatchAllforms, ref forward,
                ref wrap, ref format, ref replaceWithText,
                ref replace, ref matchKashida,
                ref matchDiactitics, ref matchAlefHamza,
                ref matchControl);
        }
        public void populatethePage(String locationofFile)
        {
            var wordapp = default(Microsoft.Office.Interop.Word.Application);
            Microsoft.Office.Interop.Word.Document worddoc;
            try
            {
                if (!string.IsNullOrEmpty(locationofFile))
                {
                    wordapp = new Microsoft.Office.Interop.Word.Application();
                    wordapp.Visible = true;
                    object argFileName = locationofFile;
                    worddoc = wordapp.Documents.Open(ref argFileName);
                    wordapp.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMaximize;
                    WindowState = WindowState.Minimized;

                }
                else
                {
                    MessageBox.Show("Please Select a Template");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("There is Flaw in Opening the Module" + ex.Message);
                if (wordapp is object)
                {
                    wordapp.Quit();
                }
            }

            wordapp = null;
            worddoc = null;
            //wordapp.Quit();


        }
        public void checkandSave()
        {
            DataLayer dl = new DataLayer();
            dl.docToPdf((string)Endfilemaker(), (string)endFileinPDF());
            Quotation quotation = new Quotation();
            quotation.referenceID = txtrefno.Text;
            quotation.senderName = txtSenderName.Text;
            quotation.companyName = getCurrentSelectedCompany().companyName;
            quotation.dateTime = txtDate.Text;
            quotation.yourEnquiry = txtYourEnquiry.Text;
            quotation.contactNameone = getPersonone().contact_name;
            quotation.contactNametwo = getPersontwo().contact_name;
            quotation.firstMail = getPersontwo().contact_email;
            quotation.secondMail = getPersontwo().contact_email;
            quotation.templatepath = getCurrentTemplate();
            quotation.wordFileLocation = (string)Endfilemaker();
            quotation.pdfFileLocation = (string)endFileinPDF();
            Response res = dl.saveQuotationinDB(quotation);
            if (res.success)
            {
                Thread.Sleep(1000);
                setAddAddressMode();
                MessageBox.Show("Quotation Saved");

            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }
    


    

        public void SaveTheQuotationdetails()
        {
            //All the Quotation Details which we might need in the future....
            DataLayer dl = new DataLayer();
            dl.docToPdf((string)Endfilemaker(), (string)endFileinPDF());
            Quotation quotation = new Quotation();
            quotation.referenceID = txtrefno.Text;
            quotation.senderName = txtSenderName.Text;
            quotation.companyName = getCurrentSelectedCompany().companyName;
            quotation.companyId = getCurrentSelectedCompany().companyId;
            quotation.dateTime = txtDate.Text;
            quotation.yourEnquiry = txtYourEnquiry.Text;
            quotation.contactNameone = getPersonone().contact_name;
            quotation.contactNametwo = getPersontwo().contact_name;
            quotation.firstMail = getPersontwo().contact_email;
            quotation.secondMail =getPersontwo().contact_email;
            quotation.templatepath = getCurrentTemplate();
            quotation.wordFileLocation = (string)Endfilemaker();
            quotation.pdfFileLocation = (string)endFileinPDF();
            Response res = dl.saveQuotationinDB(quotation);
            if (res.success)
            {
                Thread.Sleep(1000);
               setAddAddressMode();
                MessageBox.Show("Quotation Saved");

            }
            else if (res.isException)
            {
                MessageBox.Show(res.exception);
            }
        }

        private void btnOPenQuatationTemplate_Click(object sender, RoutedEventArgs e)
        {
            // OPen the Template here and save them directly to the Template folder.

            string Templatepath = getCurrentTemplate();
            populatethePage(Templatepath);


        }

        //From here I will Try to make some module for the Quotation manager which will deal with the other part of the software.

        // First I will Try to populate the Contacts of the company in the dropdown! [combobox]


        /// <summary>
        /// This method will Populate the Contacts in the Dropbox
        /// </summary>
        private void populateContactsCmb(int comId) {
            try 
            {
                DataLayer dl = new DataLayer();
                //Company company = new Company();
                //company = (Company)cmbCompanies.SelectedItem;
                Response res = dl.GetContactsByCompanyId(comId);
                if (res.success)
                {

                    if (cmbPersonone.Items.Count > 0) cmbPersonone.SelectedIndex = 0;
                    List<Contact> contacts = (List<Contact>)res.body;
                    cmbPersonone.ItemsSource = contacts;
                    cmbPersonone.DisplayMemberPath = "contact_name";

                    if (cmbPersontwo.Items.Count > 2) cmbPersontwo.SelectedIndex = 1;
                    cmbPersontwo.ItemsSource = contacts;  // Why don't we have the second Contact automatically 
                    cmbPersontwo.DisplayMemberPath = "contact_name";

                }
                else if (res.isException) {
                    throw new Exception(res.exception);


                }
            } catch( Exception ex )
            {
                MessageBox.Show("Error Occured which is " + ex.Message);
            }
        }

        //   [+]----------------------------------------------------EMail Module----------------------------------------------------------------------
        /// <summary>
        /// Email Module For sending the EMails 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ////get the quotation.
            //Quotation quotation = new Quotation();
            //quotation = getCurrentSelectedQuotationFromGrid();
            //string First = quotation.firstMail;
            //string second = quotation.secondMail;
            //MessageBox.Show("The mail We have to send " + First);
            //var url = "mailto:"+ First+"?subject=Test&body=Hello";
            //System.Diagnostics.Process.Start(url);


            //Trail 1st
            Quotation quotation = new Quotation();
            List<Contact> contacts = new List<Contact>();
            quotation = getCurrentSelectedQuotationFromGrid();
            //==================================================writing logic to get all the mail of Contacts =================
            DataLayer dl = new DataLayer();
             int totalContacts = 0;
            Response res = dl.GetContactsByCompanyId(quotation.companyId);
            if (res.success) 
            {
                 contacts = (List<Contact>)res.body;
                totalContacts = contacts.Count;

            }
            //===================================================================================================================
            // open mail and attach file
            var outlookapp = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem mail = (Microsoft.Office.Interop.Outlook.MailItem)outlookapp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            //This is for attachments 
            //__________________________
            //attaching the template
            if (chkquote.IsChecked == true)
            {
                mail.Attachments.Add(quotation.pdfFileLocation);
            }
            if (chkExtraFiles.IsChecked == true) 
            {
                mail.Attachments.Add(getcurrentExtrafile());
            }
            //__________________________
            attachbrochure(ref mail); //Ye ho gaya.
            string email = "";
            if (totalContacts > 0) {
                for (int i = 0; i < totalContacts; i++) 
                {
                    email = email + contacts[i].contact_email + ";";
                }
            
            }

            for (int i = 0; i < totalContacts; i++)
                mail.To = email;
            mail.Display();
            mail.Subject = "Offer for Rapid-I Precision Measuring System";
            attachmailbody(ref mail);
            outlookapp = null;
            mail = null;
        }
        //Now I have to check the data so that it could be the logical system according to that.


        //Now first We have to select which quotation you are talking about 


        public void attachmailbody(ref Microsoft.Office.Interop.Outlook.MailItem mail) 
        {
            string mailfile = getCurrentEmail();

            //if ((mailfile ?? "") == (Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\QM" + @"\emails" + @"\Rapid I Status Reminder Mail Template.doc" ?? ""))
                //mailfile = fillRemindertemplate(mailfile);
            var word = new Microsoft.Office.Interop.Word.Application();

            // -----------------------Set all the variables in the word file.------------------------

            object argFileName = mailfile;
            var doc = word.Documents.Open(ref argFileName);
            letSleep();
            doc.Content.Select();
            letSleep();
            doc.Content.Copy();
            letSleep();
            word.Quit();
            letSleep();
            var oInsp = mail.GetInspector;
            letSleep();
            Microsoft.Office.Interop.Word.Document objDoc;
            letSleep();
            objDoc = (Microsoft.Office.Interop.Word.Document)oInsp.WordEditor;
            letSleep();
            objDoc.Content.Paste();

        }

        public void letSleep()
        {
            System.Threading.Thread.Sleep(200);
        }
        public void attachbrochure(ref Microsoft.Office.Interop.Outlook.MailItem mail)
        {
            if (chkbroc.IsChecked == true)
            {
                string bfile = getCurrentBrochure();
                if (!string.IsNullOrEmpty(bfile))
                    mail.Attachments.Add(bfile);
            }
        }
        private Quotation getCurrentSelectedQuotationFromGrid()
        {
            Quotation quotation = (Quotation)gridQuoation.SelectedItem;
            return quotation;

        }

        /// <summary>
        /// This Button is responsible for the Revision of the Module
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (gridQuoation.SelectedIndex == -1)
                {
                    MessageBox.Show("Please Select a Quotation from the grid");

                }


                DataLayer dl = new DataLayer();
                Quotation quotation = getCurrentSelectedQuotationFromGrid();

                int currentRevisionNumber = quotation.revisionID;
                //Since use pressed this button thus we have to increase this !
                currentRevisionNumber = currentRevisionNumber + 1;

                Quotation q = new Quotation();
                q.companyId = quotation.companyId;
                q.companyName = quotation.companyName;
                q.contactNameone = quotation.contactNameone;
                q.contactNametwo = quotation.contactNametwo;
                q.dateTime = DateTime.Now.ToString("dd/MM/yyyy");
                q.firstMail = quotation.firstMail;
                q.secondMail = quotation.secondMail;
                q.senderName = quotation.senderName;
                q.templatepath = quotation.templatepath;
                q.pdfFileLocation = (string)RevisionEndDileMAkerInPDF();
                q.wordFileLocation = quotation.wordFileLocation;
                q.yourEnquiry = quotation.yourEnquiry;
                q.quotationID = currentRevisionNumber;
                q.revisionID = currentRevisionNumber;
                q.referenceID = quotation.referenceID;
                DataLayer dla = new DataLayer();

                Response res = dla.saveQuotationinDB(q);
                if (res.success)
                {
                    MessageBox.Show("Item Saved in DataBase So...");
                }
                if (res.isException)
                {
                    throw new Exception(res.exception);
                }
                CreateWordDocument(quotation.wordFileLocation, quotation.wordFileLocation ,q);
                populatethePage(quotation.wordFileLocation);
                MessageBox.Show("Whatever You save would be in Pdf Form");
                dl.docToPdf(quotation.wordFileLocation, (string)RevisionEndDileMAkerInPDF());

            }
            catch (Exception ex ) {
                MessageBox.Show("Error Occured which is " + ex.Message);
            }
            populateQuotations();
        }

        //The Experiment

        private void CreateWordDocument(object filename, object SaveAs, Quotation quotation)
        {
            string modified = txtrefno.Text + txtSenderName.Text;
            Word.Application wordApp = new Word.Application();
            object missing = Missing.Value;
            Word.Document myWordDoc = null;

            if (File.Exists((string)filename))
            {
                object readOnly = false;
                object isVisible = false;
                wordApp.Visible = false;

                myWordDoc = wordApp.Documents.Open(ref filename, ref missing, ref readOnly,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing,
                                        ref missing, ref missing, ref missing, ref missing);
                myWordDoc.Activate();

                string okay = quotation.referenceID + "-" + quotation.revisionID;
                string what = quotation.referenceID ;

                //find and replace
                this.FindAndReplace(wordApp, what, okay);
                //this.FindAndReplace(wordApp, "<ADDRESS1>", txtAddress1.Text);
                //this.FindAndReplace(wordApp, "<ADDRESS2>", txtAddress2.Text);
                //this.FindAndReplace(wordApp, "<ADDRESS3>", txtAddress3.Text);
                //this.FindAndReplace(wordApp, "<PHONE NUMBER>", txtPhone.Text);
                //this.FindAndReplace(wordApp, "<REFRENCENUMBER>", txtrefno.Text);
                //this.FindAndReplace(wordApp, "<DATE>", txtDate.Text);
                ////this.FindAndReplace(wordApp, "SENDERNAME", txtSenderName.Text);
                //this.FindAndReplace(wordApp, "<YOURENQUIRY>", txtYourEnquiry.Text);
                //this.FindAndReplace(wordApp, "<PERSON1>", getPersonone().contact_name);
                //this.FindAndReplace(wordApp, "<PERSON2>", getPersontwo().contact_name);
            }
            else
            {
                MessageBox.Show("File not Found!");
            }

            //Save as means the filename(string)
            myWordDoc.SaveAs2(ref SaveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing);

            myWordDoc.Close();
            wordApp.Quit();
            //MessageBox.Show("File Created!");
        }


        public void populateBrochure() 
        {
            string dir = @"..\..\..\QM\Resources\Brochures";
            foreach (string file in System.IO.Directory.GetFiles(dir))
            {
                cmbBro.Items.Add(System.IO.Path.GetFileName(file));
            }
        }

        public void populateExtraFiles() 
        {
            string dir = @"..\..\..\QM\Resources\ExtraFiles";
            foreach (string file in System.IO.Directory.GetFiles(dir))
            {
                cmbExtra.Items.Add(System.IO.Path.GetFileName(file));
            }

        }
        public void PopulateEmail() 
        {
            string dir = @"..\..\..\QM\Resources\Emails";
            foreach (string file in System.IO.Directory.GetFiles(dir))
            {
                cmbEmail.Items.Add(System.IO.Path.GetFileName(file));
            }
        }

        //Getting the data from diffent templates.
        public string getCurrentBrochure()
        {
            string vari = (string)cmbBro.SelectedItem;
            string fileName = (string)System.IO.Path.GetFileName(vari);
            string trickpart = @"..\..\..\QM\Resources\Brochures" + "\\" + fileName;
            //MessageBox.Show(trickpart);
            return trickpart;


        }
        public string getcurrentExtrafile() 
        {
            string vari = (string)cmbExtra.SelectedItem;
            string fileName = (string)System.IO.Path.GetFileName(vari);
            string trickpart = @"..\..\..\QM\Resources\ExtraFiles" + "\\" + fileName;
            //MessageBox.Show(trickpart);
            return trickpart;
        }
        public string getCurrentEmail()
        {
            string vari = (string)cmbEmail.SelectedItem;
            string fileName = (string)System.IO.Path.GetFileName(vari);
            string trickpart = @"..\..\..\QM\Resources\Emails" + "\\" + fileName;
            //MessageBox.Show(trickpart);
            return trickpart;


        }

        private void gridQuoation_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            

        }

        private void gridQuoation_Loaded(object sender, RoutedEventArgs e)
        {
            if (gridQuoation != null && gridQuoation.Columns.Count > 0)
            {
                gridQuoation.Columns[4].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[5].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[6].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[7].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[8].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[9].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[10].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[11].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[12].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[13].Visibility = Visibility.Collapsed;
                gridQuoation.Columns[14].Visibility = Visibility.Collapsed;
                Utility.MakeAllColumnsWidthSame(gridQuoation);
                if (gridContacts.Columns.Count >= 5) gridContacts.Columns[5].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            //object Filename = (object)getCurrentTemplate();
            //object endfile = Endfilemaker();
            //string abcd = (string)endFileinPDF();
            //string ad = (string)Endfilemaker();

            ////Creating the doc file.
            //CreateWordDocument(Filename, endfile);


           // populatethePage((string)Endfilemaker());
            //MessageBox.Show("Please Close the word File.");
            //letSleep();
            //SaveTheQuotationdetails();



            //theNewLogic
            Quotation quotation = new Quotation();
            quotation = getCurrentSelectedQuotationFromGrid();

            object filename = (object)quotation.templatepath;
            object endfile = (object)quotation.wordFileLocation;
            
            
            //CreateWordDocument(filename, endfile);
            populatethePage(quotation.wordFileLocation);
            DataLayer dl = new DataLayer();
            MessageBox.Show("Please Close the Word Application");
            dl.docToPdf(quotation.wordFileLocation, quotation.pdfFileLocation);
            MessageBox.Show("Updated");




        }
    }
}
