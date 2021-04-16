namespace CarService
{
    public class ServicesInfo
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public double Cost { get; set; }
        public double CostWithDiscount { get; set; }
        public int DurationInMinutes { get; set; }
        public double Discount { get; set; }
        public string MainImagePath { get; set; }
        public ServicesInfo(int iD, string title, double cost, int durationInSeconds, double discount, string mainImagePath)
        {
            ID = iD;
            Title = title;
            Cost = cost;
            CostWithDiscount = cost - cost * discount / 100;
            DurationInMinutes = durationInSeconds / 60;
            Discount = discount;
            MainImagePath = mainImagePath;
        }
    }
}
