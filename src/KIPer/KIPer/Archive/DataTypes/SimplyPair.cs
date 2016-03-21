namespace KipTM.Archive.DataTypes
{
    //class SimplyPair
    //{
    //    public object Item1 { get; set; }
    //    public object Item2 { get; set; }
    //}

    class SimplyPair<T1,T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }
}
