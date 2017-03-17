using System;

namespace MarkerService.Filler
{
    public interface IFillerFactory<T>
    {
        T FillMarker<Ttarget>(object Key, Ttarget result);
        T FillMarker(Type ttarget, object Key, object item);
    }
}