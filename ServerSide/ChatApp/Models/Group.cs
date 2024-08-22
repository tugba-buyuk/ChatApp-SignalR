namespace ChatApp.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<Clients> Clients { get; set; }= new List<Clients>();
    }
}
