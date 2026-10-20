namespace Catalog.Common.Exceptions
{
    public class GoodsDontExistException : Exception
    {
        private readonly List<int> GoodsIds;
        public GoodsDontExistException(string message, List<int> goodsIds) : base(message) 
        {
            GoodsIds = goodsIds;
        }

        public List<int> GetGoods()
        {
            return GoodsIds;
        }
    }
}
