using BilHealth.Model;
using BilHealth.Model.Dto.Incoming;
using BilHealth.Utility.Enum;

namespace BilHealth.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAppointment(Guid caseId, Guid requestingUserId, AppointmentUpdateDto details);
        Task<bool> CancelAppointment(Guid appointmentId);
        Task SetAppointmentApproval(Guid appointmentId, ApprovalStatus approval);

        Task CreateVisit(Guid appointmentId);
        Task<AppointmentVisit> UpdateVisit(Guid appointmentId, AppointmentVisitUpdateDto details);
    }
}
