using MongoDB.Bson;

namespace labos2.Models
{
    public class Klubovi

    {
        public ObjectId id { get; set; }
        public string ime { get; set; }
        public int osnovan { get; set; }
        public string grad { get; set; }
        public string dvorana { get; set; }
        public int kapacitet { get; set; }
        public string trener { get; set; }
        public string kapetan { get; set; }
        public List<string> igraci { get; set; }
        public string navijaci { get; set; }


    }
}
