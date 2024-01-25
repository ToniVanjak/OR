using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace labos2.Models
{
    public class IGRAC
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string ime { get; set; }
    }
    public class Klubovi

    {
        [BsonId]
        public ObjectId id { get; set; }
        public string ime { get; set; }
        public int osnovan { get; set; }
        public string grad { get; set; }
        public string dvorana { get; set; }
        public int kapacitet { get; set; }
        public string trener { get; set; }
        public string kapetan { get; set; }
        public List<IGRAC> igraci { get; set; }
        public string navijaci { get; set; }


    }

    public class Klub

    {
        public string id { get; set; }
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


    public class AddKlub
    {
            [BsonId]
            public ObjectId id { get; set; }
            public string ime { get; set; }
            public int osnovan { get; set; }
            public string grad { get; set; }
            public string dvorana { get; set; }
            public int kapacitet { get; set; }
            public string trener { get; set; }
            public List<IGRAC> igraci { get; set; } = new List<IGRAC>();
            public string kapetan { get; set; }
            public string navijaci { get; set; }

    }

    public class UpdateKlub
    {
        [BsonId]
        public ObjectId id { get; set; }
        public string ime { get; set; }
        public int osnovan { get; set; }
        public string grad { get; set; }
        public string dvorana { get; set; }
        public int kapacitet { get; set; }
        public string trener { get; set; }
        public string kapetan { get; set; }
        public string navijaci { get; set; }

    }
}
