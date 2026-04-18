namespace RepairCircle.ViewModels.Feedbacks;

public class FeedbackFormViewModel
{
    public string RepairRequestTitle { get; set; } = string.Empty;
    public FeedbackFormInputModel Input { get; set; } = new();
}
