using Medical_Office_WebAPI_Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medical_Office_WebAPI_Client.Data
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetDoctors();
        Task<Doctor> GetDoctor(int ID);
    }
}
