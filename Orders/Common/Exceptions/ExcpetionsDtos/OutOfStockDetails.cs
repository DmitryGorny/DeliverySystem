namespace Orders.Common.Exceptions.ExcpetionsDtos
{
    public class OutOfStockDetails
    {
        public int GoodId { get; set; }
        public int Requested { get; set; }
        public int Available { get; set; }
    }
}
