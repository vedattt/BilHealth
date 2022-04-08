using BilHealth.Utility.Enum;

namespace BilHealth.Model.Dto
{
    public record AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = String.Empty;
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Waiting;
        public bool Attended { get; set; } = false;

        public AppointmentVisitDto? Visit { get; set; }
    }
}
