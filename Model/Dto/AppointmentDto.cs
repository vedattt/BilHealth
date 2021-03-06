using BilHealth.Utility.Enum;
using NodaTime;

namespace BilHealth.Model.Dto
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public SimpleUserDto RequestingUser { get; set; } = null!;
        public Guid CaseId { get; set; }
        public Instant CreatedAt { get; set; }
        public Instant DateTime { get; set; }
        public string Description { get; set; } = String.Empty;
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Waiting;
        public bool Attended { get; set; } = false;
        public bool Cancelled { get; set; } = false;

        public AppointmentVisitDto? Visit { get; set; }
    }
}
