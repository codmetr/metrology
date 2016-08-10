namespace KipTM.Model.Checks.Steps
{
    public interface IPausedStep
    {
        bool Pause();
        bool Resume();
    }
}