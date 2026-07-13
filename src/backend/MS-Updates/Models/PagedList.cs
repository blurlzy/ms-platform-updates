namespace MS_Updates.Models
{
    public class PagedList<T> where T : class
    {
        public int Total { get; private set; }
        public IReadOnlyCollection<T> Data { get; private set; }

        // ctor
        public PagedList(int count, IReadOnlyCollection<T> data)
        {
            Total = count;
            Data = data;
        }

    }
}
