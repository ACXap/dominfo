
namespace dominfo
{
    public class ManagementCompany
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Inn { get; set; }
        public int CountHouse { get; set; }

        public override string ToString()
        {
            return $"{Name};{Url};{Inn};{CountHouse}";
        }
    }
}