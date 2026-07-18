namespace MS_Updates.Models
{
    public class PagedList<T> where T : class
    {
        public int Total { get; private set; }
        public IReadOnlyCollection<T> Data { get; private set; }

        // ctor
        public PagedList(int total, IReadOnlyCollection<T> data)
        {
            Total = total;
            Data = data;
        }

    }
}
