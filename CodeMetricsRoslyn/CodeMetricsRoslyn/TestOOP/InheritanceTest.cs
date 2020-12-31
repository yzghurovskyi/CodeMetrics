namespace CodeMetricsRoslyn
{
    class C : IC
    {
        public string GetStr => "asd";
        public int GetCount() => 3;

        private int _privateAttr;

        public int PublicInt;
        protected int ProtectedAttr;
    }

    class C1 : C
    {
    }

    class C2 : C
    {
    }

    class C11 : C1
    {
    }

    class C12 : C1
    {
    }

    class C21 : C2
    {
    }

    class C22 : C2
    {
    }

    class C211: C21
    {
        
    }

    interface IC
    {
        string GetStr { get; }
        int GetCount();
    }
}
