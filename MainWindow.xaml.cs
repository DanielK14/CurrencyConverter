using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System;
using System.Configuration;

namespace CurrencyConverter_Static
{
    public partial class MainWindow : Window
    {      
        SqlConnection sqlConnection = new SqlConnection();

        SqlCommand sqlCmd = new SqlCommand();

        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();

        private int currencyId = 0;
        private double fromAmount = 0;
        private double toAmount = 0;
        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            BindCurrency();
            GetData();
        }

        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(Conn);
            sqlConnection.Open(); 
        }

        private void BindCurrency()
        {
            mycon();
            DataTable dtCurrency = new DataTable();

            sqlCmd = new SqlCommand("select Id, CurrencyName from Currency_Master", sqlConnection);

            //CommandType define which type of command we use for write a query
            sqlCmd.CommandType = CommandType.Text;

            //It accepts a parameter that contains the command text of the object's selectCommand property.
            sqlDataAdapter = new SqlDataAdapter(sqlCmd);

            sqlDataAdapter.Fill(dtCurrency);

            DataRow newRow = dtCurrency.NewRow();

            newRow["Id"] = 0;

            newRow["CurrencyName"] = "--SELECT--";

            dtCurrency.Rows.InsertAt(newRow, 0);

            if (dtCurrency != null && dtCurrency.Rows.Count > 0)
            {
                //Assign the datatable data to from currency combobox using ItemSource property.
                cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;

                //Assign the datatable data to to currency combobox using ItemSource property.
                cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            }
            sqlConnection.Close();

            //To display the underlying datasource for cmbFromCurrency
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";

            //To use as the actual value for the items
            cmbFromCurrency.SelectedValuePath = "Id";

            //Show default item in combobox
            cmbFromCurrency.SelectedValue = 0;

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedValue = 0;

            /*
             * 
            dtCurrency.Rows.Add("PokéYen", 0.015);
            dtCurrency.Rows.Add("Imperial Credits", 0.0267);
            */
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double ConvertedValue;
                if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "" || !IsNumber(txtCurrency.Text))
                {  
                    MessageBox.Show("Please enter an amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrency.Focus();
                    return;
                }
                else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbFromCurrency.Focus();
                    return;
                }
                else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbToCurrency.Focus();
                    return;
                }

                if (cmbFromCurrency.Text == cmbToCurrency.Text)
                {
                    ConvertedValue = double.Parse(txtCurrency.Text.Replace(".", ","));
                    lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
                }
                else
                {
                    ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text.Replace(".", ","))) / double.Parse(cmbToCurrency.SelectedValue.ToString());
                    lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            try
            {
                txtCurrency.Text = string.Empty;
                if (cmbFromCurrency.Items.Count > 0)
                {
                    cmbFromCurrency.SelectedIndex = 0;
                }

                if (cmbToCurrency.Items.Count > 0)
                {
                    cmbToCurrency.SelectedIndex = 0;
                }

                lblCurrency.Content = "";
                txtCurrency.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]+,?");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                currencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "" || !IsNumber(txtAmount.Text))
                {
                    MessageBox.Show("Please enter an amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {   if (currencyId > 0)
                    {
                       if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            DataTable dt = new DataTable();

                            sqlCmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", sqlConnection);
                            sqlCmd.CommandType = CommandType.Text;
                            sqlCmd.Parameters.AddWithValue("@Id", currencyId);
                            sqlCmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Replace(",", "."));
                            sqlCmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            sqlCmd.ExecuteNonQuery();
                            sqlConnection.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            sqlCmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", sqlConnection);
                            sqlCmd.CommandType = CommandType.Text;
                            sqlCmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Replace(",", "."));
                            sqlCmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            sqlCmd.ExecuteNonQuery();
                            sqlConnection.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetData()
        {
            //The method is used for connect with database and open database connection    
            mycon();

            //Create Datatable object
            DataTable dt = new DataTable();

            //Write Sql Query for Get data from database table. Query written in double quotes and after comma provide connection    
            sqlCmd = new SqlCommand("SELECT * FROM Currency_Master", sqlConnection);

            //CommandType define Which type of command execute like Text, StoredProcedure, TableDirect.    
            sqlCmd.CommandType = CommandType.Text;

            //It is accept a parameter that contains the command text of the object's SelectCommand property.
            sqlDataAdapter = new SqlDataAdapter(sqlCmd);

            //The DataAdapter serves as a bridge between a DataSet and a data source for retrieving and saving data. The Fill operation then adds the rows to destination DataTable objects in the DataSet    
            sqlDataAdapter.Fill(dt);

            //dt is not null and rows count greater than 0
            if (dt != null && dt.Rows.Count > 0)
            {
                //Assign DataTable data to dgvCurrency using ItemSource property.   
                dgvCurrency.ItemsSource = dt.DefaultView;
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            //Database connection Close
            sqlConnection.Close();
        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;

                DataRowView row_selected = grd.CurrentItem as DataRowView;

                if (row_selected != null)
                {
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            currencyId = Int32.Parse(row_selected["Id"].ToString());

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();

                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";
                            }

                            //DisplayIndex is equal to one in the deleted cell
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                //Show confirmation dialog box
                                if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    mycon();
                                    DataTable dt = new DataTable();

                                    //Execute delete query to delete record from table using Id
                                    sqlCmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", sqlConnection);
                                    sqlCmd.CommandType = CommandType.Text;

                                    //CurrencyId set in @Id parameter and send it in delete statement
                                    sqlCmd.Parameters.AddWithValue("@Id", currencyId);
                                    sqlCmd.ExecuteNonQuery();
                                    sqlConnection.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbFromCurrency.SelectedValue != null && int.Parse(cmbFromCurrency.SelectedValue.ToString()) != 0 && cmbFromCurrency.SelectedIndex != 0)
                {
                    int CurrencyFromId = int.Parse(cmbFromCurrency.SelectedValue.ToString());

                    mycon();
                    DataTable dt = new DataTable();

                    sqlCmd = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyFromId", sqlConnection);
                    sqlCmd.CommandType = CommandType.Text;

                    if (CurrencyFromId != null && CurrencyFromId != 0)
                        sqlCmd.Parameters.AddWithValue("@CurrencyFromId", CurrencyFromId);

                    sqlDataAdapter = new SqlDataAdapter(sqlCmd);

                    sqlDataAdapter.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                        fromAmount = double.Parse(dt.Rows[0]["Amount"].ToString());

                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbToCurrency.SelectedValue != null && int.Parse(cmbToCurrency.SelectedValue.ToString()) != 0 && cmbToCurrency.SelectedIndex != 0)
                {
                    int CurrencyToId = int.Parse(cmbToCurrency.SelectedValue.ToString());

                    mycon();

                    DataTable dt = new DataTable();
                    //Select query for get Amount from database using id
                    sqlCmd = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyToId", sqlConnection);
                    sqlCmd.CommandType = CommandType.Text;

                    if (CurrencyToId != null && CurrencyToId != 0)
                        //CurrencyToId set in @CurrencyToId parameter and send parameter in our query
                        sqlCmd.Parameters.AddWithValue("@CurrencyToId", CurrencyToId);

                    sqlDataAdapter = new SqlDataAdapter(sqlCmd);

                    //Set the data that the query returns in the data table
                    sqlDataAdapter.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                        //Get amount column value from datatable and set amount value in ToAmount variable which is declared globally
                        toAmount = double.Parse(dt.Rows[0]["Amount"].ToString());
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbFromCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //If the user press Tab or Enter key then cmbFromCurrency_SelectionChanged event is executed
            if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
            {
                cmbFromCurrency_SelectionChanged(sender, null);
            }
        }

        private void cmbToCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //If the user press Tab or Enter key then cmbToCurrency_SelectionChanged event is executed
            if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
            {
                cmbToCurrency_SelectionChanged(sender, null);
            }
        }

        private bool IsNumber(string input)
        {
            string testString = input.Replace(".", ",");
            return double.TryParse(testString, out _);
        }
    }
}
