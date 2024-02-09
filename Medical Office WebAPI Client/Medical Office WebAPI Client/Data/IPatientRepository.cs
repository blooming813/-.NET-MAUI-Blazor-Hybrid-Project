using Medical_Office_WebAPI_Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medical_Office_WebAPI_Client.Data
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetPatients();
        Task<Patient> GetPatient(int ID);
        Task<List<Patient>> GetPatientsByDoctor(int DoctorID);
    }
}
