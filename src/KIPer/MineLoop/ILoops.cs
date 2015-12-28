using System;

namespace MainLoop
{
    public interface ILoops
    {
        void AddLocker(string key, object locker);
        void StartImportantAction(string key, Action<object> action);
        void StartMiddleAction(string key, Action<object> action);
        void StartUnmportantAction(string key, Action<object> action);
    }
}