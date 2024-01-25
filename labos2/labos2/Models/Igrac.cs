using labos2.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace labos2.Models
{
    public class Igrac
    {
        public string Id { get; set; }
        public string ImeIgraca { get; set; }
        public string ImeKluba { get; set; }
        public int Osnovan { get; set; }
        public string Grad { get; set; }
        public string Dvorana { get; set; }
        public int Kapacitet { get; set; }
        public string Trener { get; set; }
        public string Kapetan { get; set; }
        public string Navijaci { get; set; }
    }

    public class AddIgracToKlubDto
    {
        public string Ime { get; set; }
        public string Klub { get; set; }
    }
}
