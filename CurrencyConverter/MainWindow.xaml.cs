using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Data;
using System.Text.RegularExpressions;
using System.Net.Http;
using Newtonsoft.Json;

namespace CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Root val = new Root();

        public class Root
        {
            public Rate rates { get; set; }
            public long timestamp;
            public string license;
        }

        public class Rate
        {
            public double USD { get; set; }
            public double EUR { get; set; }
            public double GBP { get; set; }
            public double CHF { get; set; }
            public double CAD { get; set; }
            public double JPY { get; set; }
            public double AUD { get; set; }
            public double TRY { get; set; }
        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var ResponceString = await response.Content.ReadAsStringAsync();
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponceString);

                        MessageBox.Show("Timestamp: " + ResponseObject.timestamp, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        return ResponseObject;
                    }
                    return myRoot;
                }
            }
            catch (Exception)
            {
                return myRoot;
            }
        }

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=2fc053b949af443b84bdcc5c0a816c76");

            // BindCurrency method used to bind currency name with the value in the Combobox
            BindCurrency();
        }

        public MainWindow()
        {
            InitializeComponent();

            // ClearControls method used to clear all values from window.
            ClearControls();

            // Requests data using an API
            GetValue();
        }

        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();

            // Add display and value column in DataTable
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Rate");

            // Add rows in DataTable with text and value
            dtCurrency.Rows.Add("--Select--", 0);
            dtCurrency.Rows.Add("USD", val.rates.USD);
            dtCurrency.Rows.Add("EUR", val.rates.EUR);
            dtCurrency.Rows.Add("GBP", val.rates.GBP);
            dtCurrency.Rows.Add("AUD", val.rates.AUD);
            dtCurrency.Rows.Add("JPY", val.rates.JPY);
            dtCurrency.Rows.Add("CHF", val.rates.CHF);
            dtCurrency.Rows.Add("TRY", val.rates.TRY);

            // Datatable data assign From currency Combobox.
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            // DisplayMemberPath propertyt is used to display data in 
            cmbFromCurrency.DisplayMemberPath = "Text";
            // SelectedValuePath is used to set a value to data in Combobox
            cmbFromCurrency.SelectedValuePath = "Rate";
            // SelectedIndex property is used to bind Combobox it's default selected item as first rown in datatable
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Rate";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            // Check amount textbox is Null or blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }

            // Check FROM cmb is selected or not
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a currency you want to convert from", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on From cmb
                cmbFromCurrency.Focus();
                return;
            }

            // Check To cmb is selected or not
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a currency you want to convert to", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on To cmb
                cmbToCurrency.Focus();
                return;
            }

            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
    }
}
