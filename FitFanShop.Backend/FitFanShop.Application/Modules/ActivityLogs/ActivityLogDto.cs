namespace FitFanShop.Application.Modules.ActivityLogs;

public class ActivityLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
}
