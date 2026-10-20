namespace Orders.Common.Exceptions
{
    public class GoodsDontExistException : Exception
    {
        public List<int> Goods { get; private set; }

        public GoodsDontExistException(string mesage, List<int> goods) : base(mesage)
        {
            Goods = goods;
        }
    }
}
