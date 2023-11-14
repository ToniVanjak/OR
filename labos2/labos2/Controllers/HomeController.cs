using Azure;
using CsvHelper;
using CsvHelper.Configuration;
using labos2.Data;
using labos2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Writers;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using YourNamespace.Pages.Home;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly labos2.Data.DbContext _dbContext;

    public HomeController(labos2.Data.DbContext dbContext)
    {
        _dbContext = dbContext;
    }

  
    [HttpGet]
    public IActionResult GetData()
    {
        List<Igrac> igraci = new List<Igrac>();
        var cursor = _dbContext.Klubovi.Find(Builders<Klubovi>.Filter.Empty);
        foreach (var klub in cursor.ToEnumerable())
        {
            foreach (var igrac in klub.igraci)
            {

                Igrac i = new Igrac();
                i.ImeKluba = klub.ime;
                i.ImeIgraca = igrac;
                i.Grad = klub.grad;
                i.Dvorana = klub.dvorana;
                i.Navijaci = klub.navijaci;
                i.Kapacitet = klub.kapacitet;
                i.Osnovan = klub.osnovan;
                i.Trener = klub.trener;
                i.Kapetan = klub.kapetan;
                igraci.Add(i);
            }

        }
        return Ok(igraci);
    }

}