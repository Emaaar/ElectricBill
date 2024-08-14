using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace ElectricBill
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();



            //Get All Persons  
            var personList = await App.SQLiteDb.GetItemsAsync();
            if (personList != null)
            {
                sPersons.ItemsSource = personList;
            }
        }

        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            try
            {

                double prev;
                if (!double.TryParse(txtprev.Text, out prev))
                {
                    await DisplayAlert("Invalid Input", "Please Enter a valid number for Previous Reading!", "OK");
                    return;
                }
                double pres;
                if (!double.TryParse(txtpres.Text, out pres))
                {
                    await DisplayAlert("Required", "Please Enter a valid number for Present Reading!", "OK");
                    return;
                }
                double cr = pres - prev;
                double kwh = 0;
                if (cr < 72)
                {
                    kwh = 6.50;
                }
                else if (cr < 150)
                {
                    kwh = 9.50;
                }
                else if (cr < 350)
                {
                    kwh = 10.50;
                }
                else if (cr < 400)
                {
                    kwh = 12.50;
                }
                else if (cr < 500)
                {
                    kwh = 14.00;
                }
                else
                {
                    kwh = 16.50;
                }
                double ec = kwh * cr;
                double dc = 0;
                double sc = 0;
                object selectedItem = type.SelectedItem;


                if (selectedItem != null)
                {
                    string dd = selectedItem.ToString();
                    if (dd == "H")
                    {
                        dc = 200; 
                        sc = 50;
                    }
                    else if (dd == "B")
                    {
                        dc = 400;
                        sc = 100;
                    }
                    else
                    {
                        await DisplayAlert("Error", "Invalid selection", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Invalid selection", "OK");
                }
                double pa = ec + dc + sc;
                double vat = pa * 0.05;
                double payable = pa + vat;
                Reading data = new Reading()
                {
                    PreviousReading = prev,
                    PresentReading = pres,
                    VAT = vat,
                    PA = pa,
                    AP = payable
                };

                // Add New Person  
                await App.SQLiteDb.SaveItemAsync(data);

                // Clear text boxes

                txtpres.Text = string.Empty;
                txtprev.Text = string.Empty;
                txtvat.Text = "VAT: " + vat;
                txtpa.Text = "Principal Amount: " + pa;
                txtap.Text = "Amount Payable: " + payable;
                await DisplayAlert("Success", "Person added Successfully", "OK");

                //Get All Persons  
                var datalist = await App.SQLiteDb.GetItemsAsync();
                if (datalist != null)
                {
                    sPersons.ItemsSource = datalist;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }



        private async void BtnRead_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtmeter.Text))
            {
                //Get Person  
                var data = await App.SQLiteDb.GetItemAsync(Convert.ToInt32(txtmeter.Text));
                if (data != null)
                {
                    // Create a list to hold the matching record
                    List<Reading> dataList = new List<Reading>();
                    dataList.Add(data);

                    // Set the ItemsSource of the ListView to the list containing the matching record
                    sPersons.ItemsSource = dataList;

                    txtmeter.Text = data.MeterNumber.ToString();
                    txtpres.Text = data.PresentReading.ToString();
                    txtprev.Text = data.PreviousReading.ToString();
                    txtvat.Text = "VAT: " + data.VAT.ToString();
                    txtpa.Text = "Principal Amount: " + data.PA.ToString();
                    txtap.Text = "Amount Payable: " + data.AP.ToString();
                }
                else
                {
                    // Clear the ItemsSource of the ListView if no matching record found
                    sPersons.ItemsSource = null;
                    await DisplayAlert("Not Found", "No record found with the provided Meter Number", "OK");
                }
            }
            else
            {
                await DisplayAlert("Required", "Please Enter Meter Number", "OK");
            }
        }



        private async void BtnUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtmeter.Text))
                {
                    // Parse meter number
                    int meterNumber = Convert.ToInt32(txtmeter.Text);

                    // Parse present and previous readings
                    double presentReading, previousReading;
                    if (!double.TryParse(txtpres.Text, out presentReading) || !double.TryParse(txtprev.Text, out previousReading))
                    {
                        await DisplayAlert("Invalid Input", "Please enter valid numbers for present and previous readings.", "OK");
                        return;
                    }

                    // Calculate consumption and billing
                    double consumption = presentReading - previousReading;
                    double kwh = 0;
                    if (consumption < 72)
                    {
                        kwh = 6.50;
                    } 
                    else if (consumption < 150)
                    {
                        kwh = 9.50;
                    }
                    else if (consumption < 350)
                    {
                        kwh = 10.50;
                    }
                    else if (consumption < 400)
                    {
                        kwh = 12.50;
                    }
                    else if (consumption < 500)
                    {
                        kwh = 14.00;
                    }
                    else
                    {
                        kwh = 16.50;
                    }
                    double electricityCharge = kwh * consumption;

                    // Fetch other charges based on user selection (H or B)
                    double deliveryCharge = 0, serviceCharge = 0;
                    object selectedItem = type.SelectedItem;
                    if (selectedItem != null)
                    {
                        string dd = selectedItem.ToString();
                        if (dd == "H")
                        {
                            deliveryCharge = 200;
                            serviceCharge = 50;
                        }
                        else if (dd == "B")
                        {
                            deliveryCharge = 400;
                            serviceCharge = 100;
                        }
                        else
                        {
                            await DisplayAlert("Error", "Invalid selection", "OK");
                            return;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Invalid selection", "OK");
                        return;
                    }

                    // Calculate total payable
                    double totalAmount = electricityCharge + deliveryCharge + serviceCharge;
                    double vat = totalAmount * 0.05;
                    double payableAmount = totalAmount + vat;

                    // Create a new DataList object with updated values
                    Reading person = new Reading()
                    {
                        MeterNumber = meterNumber,
                        PresentReading = presentReading,
                        PreviousReading = previousReading,
                        VAT = vat,
                        PA = totalAmount,
                        AP = payableAmount
                    };

                    // Update the item in the database
                    await App.SQLiteDb.SaveItemAsync(person);

                    // Clear text boxes
                    txtmeter.Text = string.Empty;
                    txtpres.Text = string.Empty;
                    txtprev.Text = string.Empty;
                    txtvat.Text = "VAT: " + vat;
                    txtpa.Text = "Principal Amount: " + totalAmount;
                    txtap.Text = "Amount Payable: " + payableAmount;
                    await DisplayAlert("Success", "Person Updated Successfully", "OK");

                    // Update the UI with the updated data
                    var personList = await App.SQLiteDb.GetItemsAsync();
                    if (personList != null)
                    {
                        sPersons.ItemsSource = personList;
                    }
                }
                else
                {
                    await DisplayAlert("Required", "Please Enter PersonID", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }


        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
                try
                {
                    if (!string.IsNullOrEmpty(txtmeter.Text))
                    {
                        // Parse meter number
                        int meterNumber = Convert.ToInt32(txtmeter.Text);

                        // Retrieve the item from the database
                        var data = await App.SQLiteDb.GetItemAsync(meterNumber);

                        if (data != null)
                        {
                            // Ask for confirmation before deletion
                            bool answer = await DisplayAlert("Confirmation", "Are you sure you want to delete this record?", "Yes", "No");
                            if (answer)
                            {
                                // Delete the item from the database
                                await App.SQLiteDb.DeleteItemAsync(data);

                                // Clear text boxes
                                txtmeter.Text = string.Empty;
                                txtpres.Text = string.Empty;
                                txtprev.Text = string.Empty;
                                txtvat.Text = string.Empty;
                                txtpa.Text = string.Empty;
                                txtap.Text = string.Empty;

                                // Display success message
                                await DisplayAlert("Success", "Record Deleted Successfully", "OK");

                                // Update the UI with the updated data
                                var personList = await App.SQLiteDb.GetItemsAsync();
                                if (personList != null)
                                {
                                    sPersons.ItemsSource = personList;
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "Record not found", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Required", "Please Enter PersonID", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }
    }