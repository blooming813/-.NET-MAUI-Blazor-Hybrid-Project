using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOWebAPI.Data;
using MOWebAPI.Models;

namespace MOWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly MedicalOfficeContext _context;

        public DoctorController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: api/Doctor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
        {
            return await _context.Doctors
                .Select(d => new DoctorDTO
                {
                    ID = d.ID,
                    FirstName = d.FirstName,
                    MiddleName = d.MiddleName,
                    LastName = d.LastName,
                    RowVersion = d.RowVersion

                })
                .ToListAsync();
        }

        // GET: api/Doctor/inc - Include the Patients Collection
        [HttpGet("inc")]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctorsInc()
        {
            return await _context.Doctors
                .Include(d => d.Patients)
                .Select(d => new DoctorDTO
                {
                    ID = d.ID,
                    FirstName = d.FirstName,
                    MiddleName = d.MiddleName,
                    LastName = d.LastName,
                    RowVersion = d.RowVersion,
                    Patients = d.Patients.Select(dPatient => new PatientDTO
                    {
                        ID = dPatient.ID,
                        FirstName = dPatient.FirstName,
                        MiddleName = dPatient.MiddleName,
                        LastName = dPatient.LastName,
                        OHIP = dPatient.OHIP,
                        DOB = dPatient.DOB,
                        ExpYrVisits = dPatient.ExpYrVisits,
                        DoctorID = dPatient.DoctorID
                    }).ToList()
                })
                .ToListAsync();
        }

        // GET: api/Doctor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctor(int id)
        {
            var doctorDTO = await _context.Doctors
                .Select(d => new DoctorDTO
                {
                    ID = d.ID,
                    FirstName = d.FirstName,
                    MiddleName = d.MiddleName,
                    LastName = d.LastName,
                    RowVersion = d.RowVersion
                })
                .FirstOrDefaultAsync(p => p.ID == id);

            if (doctorDTO == null)
            {
                return NotFound(new { message = "Error: Doctor not found." });
            }

            return doctorDTO;
        }

        // GET: api/Doctor/inc/5
        [HttpGet("inc/{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctorInc(int id)
        {
            var doctorDTO = await _context.Doctors
                .Include(d => d.Patients)
                .Select(d => new DoctorDTO
                {
                    ID = d.ID,
                    FirstName = d.FirstName,
                    MiddleName = d.MiddleName,
                    LastName = d.LastName,
                    RowVersion = d.RowVersion,
                    Patients = d.Patients.Select(dPatient => new PatientDTO
                    {
                        ID = dPatient.ID,
                        FirstName = dPatient.FirstName,
                        MiddleName = dPatient.MiddleName,
                        LastName = dPatient.LastName,
                        OHIP = dPatient.OHIP,
                        DOB = dPatient.DOB,
                        ExpYrVisits = dPatient.ExpYrVisits,
                        DoctorID = dPatient.DoctorID
                    }).ToList()
                })
                .FirstOrDefaultAsync(p => p.ID == id);

            if (doctorDTO == null)
            {
                return NotFound(new { message = "Error: Doctor not found." });
            }

            return doctorDTO;
        }

        // PUT: api/Doctor/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, DoctorDTO doctorDTO)
        {
            if (id != doctorDTO.ID)
            {
                return BadRequest(new { message = "Error: Incorrect ID for Doctor." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get the record you want to update
            var doctorToUpdate = await _context.Doctors.FindAsync(id);

            //Check that you got it
            if (doctorToUpdate == null)
            {
                return NotFound(new { message = "Error: Doctor record not found." });
            }

            //Wow, we have a chance to check for concurrency even before bothering
            //the database!  Of course, it will get checked again in the database just in case
            //it changes after we pulled the record.  
            //Note using SequenceEqual becuase it is an array after all.
            if (doctorDTO.RowVersion != null)
            {
                if (!doctorToUpdate.RowVersion.SequenceEqual(doctorDTO.RowVersion))
                {
                    return Conflict(new { message = "Concurrency Error: Doctor has been changed by another user.  Back out and try editing the record again." });
                }
            }

            //Update the properties of the entity object from the DTO object
            doctorToUpdate.ID = doctorDTO.ID;
            doctorToUpdate.FirstName = doctorDTO.FirstName;
            doctorToUpdate.MiddleName = doctorDTO.MiddleName;
            doctorToUpdate.LastName = doctorDTO.LastName;
            doctorToUpdate.RowVersion = doctorDTO.RowVersion;

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(doctorToUpdate).Property("RowVersion").OriginalValue = doctorDTO.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Doctor has been Removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Doctor has been updated by another user.  Back out and try editing the record again." });
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
            }
        }

        // POST: api/Doctor
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(DoctorDTO doctorDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Doctor doctor = new Doctor
            {
                FirstName = doctorDTO.FirstName,
                MiddleName = doctorDTO.MiddleName,
                LastName = doctorDTO.LastName
            };

            try
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
                //Assign Database Generated values back into the DTO
                doctorDTO.ID = doctor.ID;
                doctorDTO.RowVersion = doctor.RowVersion;
                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.ID }, doctorDTO);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
            }
        }

        // DELETE: api/Doctor/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Doctor>> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound(new { message = "Delete Error: Doctor has already been removed." });
            }
            try
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    return BadRequest(new { message = "Delete Error: Remember, you cannot delete a Doctor that has patients assigned." });
                }
                else
                {
                    return BadRequest(new { message = "Delete Error: Unable to delete Doctor. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.ID == id);
        }
    }
}
