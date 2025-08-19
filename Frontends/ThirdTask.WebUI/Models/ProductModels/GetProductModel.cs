namespace ThirdTask.WebUI.Models.ProductModels
{
    public class GetProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public bool Status { get; set; }
    }
}
