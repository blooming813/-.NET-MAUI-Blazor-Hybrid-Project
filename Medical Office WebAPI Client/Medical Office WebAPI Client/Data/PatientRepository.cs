using Medical_Office_WebAPI_Client.Models;
using Medical_Office_WebAPI_Client.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Medical_Office_WebAPI_Client.Data
{
    public class PatientRepository : IPatientRepository
    {
        private readonly HttpClient client = new HttpClient();
        public PatientRepository()
        {
            client.BaseAddress = Jeeves.DBUri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Patient>> GetPatients()
        {
            HttpResponseMessage response = await client.GetAsync("api/patient");
            if (response.IsSuccessStatusCode)
            {
                List<Patient> Patients = await response.Content.ReadAsAsync<List<Patient>>();
                return Patients;
            }
            else
            {
                throw new Exception("Could not access the list of Patients.");
            }
        }

        public async Task<List<Patient>> GetPatientsByDoctor(int DoctorID)
        {
            HttpResponseMessage response = await client.GetAsync($"api/patient/byDoctor/{DoctorID}");
            if (response.IsSuccessStatusCode)
            {
                List<Patient> Patients = await response.Content.ReadAsAsync<List<Patient>>();
                return Patients;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("Cannot find any Patinets for that Doctor.");
                }
                else
                {
                    throw new Exception("Could not access the list of Patients by Doctor.");
                }
            }
        }

        public async Task<Patient> GetPatient(int ID)
        {
            HttpResponseMessage response = await client.GetAsync($"api/patient/{ID}");
            if (response.IsSuccessStatusCode)
            {
                Patient Patient = await response.Content.ReadAsAsync<Patient>();
                return Patient;
            }
            else
            {
                throw new Exception("Could not access that Patient.");
            }
        }
    }
}
