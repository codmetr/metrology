namespace MarkerService.Filler
{
    public interface IFiller<T>
    {
        T FillMarker<Ttarget>(Ttarget result);
        T FillMarker(object result);
    }
}