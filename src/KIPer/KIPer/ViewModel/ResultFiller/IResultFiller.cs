namespace KipTM.ViewModel.ResultFiller
{
    public interface IResultFiller
    {
        IParameterResultViewModel GetFillResultMarker<T>(T result);
        IParameterResultViewModel GetFillResultMarker(object result);
    }
}