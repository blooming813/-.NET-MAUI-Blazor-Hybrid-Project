using Medical_Office_WebAPI_Client.Data;
using Medical_Office_WebAPI_Client.Models;
using Medical_Office_WebAPI_Client.Utilities;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Medical_Office_WebAPI_Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private readonly IDoctorRepository doctorRepository;
        private readonly IPatientRepository patientRepository;

        public MainPage()
        {
            this.InitializeComponent();
            doctorRepository = new DoctorRepository();
            patientRepository = new PatientRepository();
            FillDropDown();
        }

        private async void FillDropDown()
        {
            //Show Progress
            progRing.IsActive = true;
            progRing.Visibility = Visibility.Visible;

            try
            {
                List<Doctor> doctors = await doctorRepository.GetDoctors();
                //Add the All Option
                doctors.Insert(0, new Doctor { ID = 0, LastName = " - All Doctors" });
                //Bind to the ComboBox
                DoctorCombo.ItemsSource = doctors;
                ShowPatients(null);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message.Contains("connection with the server"))
                {
                    Jeeves.ShowMessage("Error", "No connection with the server.");
                }
                else
                {
                    Jeeves.ShowMessage("Error", "Could not complete operation");
                }
            }
            finally
            {
                progRing.IsActive = false;
                progRing.Visibility = Visibility.Collapsed;
            }
        }

        private void DoctorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Doctor selDoc = (Doctor)DoctorCombo.SelectedItem;
            ShowPatients(selDoc?.ID);
        }

        private async void ShowPatients(int? DoctorID)
        {
            //Show Progress
            progRing.IsActive = true;
            progRing.Visibility = Visibility.Visible;

            try
            {
                List<Patient> patients;
                if (DoctorID.GetValueOrDefault() > 0)
                {
                    patients = await patientRepository.GetPatientsByDoctor(DoctorID.GetValueOrDefault());
                }
                else
                {
                    patients = await patientRepository.GetPatients();
                }
                patientList.ItemsSource = patients;

            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message.Contains("connection with the server"))
                {
                    Jeeves.ShowMessage("Error", "No connection with the server.");
                }
                else
                {
                    Jeeves.ShowMessage("Error", "Could not complete operation");
                }
            }
            finally
            {
                progRing.IsActive = false;
                progRing.Visibility = Visibility.Collapsed;
            }
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            FillDropDown();
        }
    }
}
