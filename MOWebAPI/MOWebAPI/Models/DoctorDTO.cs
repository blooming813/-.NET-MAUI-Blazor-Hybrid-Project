using Microsoft.AspNetCore.Mvc;

namespace MOWebAPI.Models
{
    [ModelMetadataType(typeof(DoctorMetaData))]
    public class DoctorDTO
    {
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public Byte[] RowVersion { get; set; }

        public ICollection<PatientDTO> Patients { get; set; }
    }
}
