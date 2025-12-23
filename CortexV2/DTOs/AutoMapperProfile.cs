using AutoMapper;
using Cortex.DTOs.Users;
using Cortex.Entities;

namespace Cortex.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Appointment, AppointmentDto>();
            CreateMap<Patient, PatientDto>();
            CreateMap<Staff, StaffDto>();
        }
    }
}
