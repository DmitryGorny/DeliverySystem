namespace Catalog.Common.Exceptions
{
    public class GoodsDontExistException : Exception
    {
        public List<int> GoodsIds { get; private set; }
        public GoodsDontExistException(string message, List<int> goodsIds) : base(message) 
        {
            GoodsIds = goodsIds;
        }
    }
}
