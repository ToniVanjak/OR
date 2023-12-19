using Azure;
using CsvHelper;
using labos2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;


namespace labos2.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly labos2.Data.DbContext _dbContext;
    private List<Igrac> igraci;

    public HomeController(labos2.Data.DbContext dbContext)
    {
        _dbContext = dbContext;

        igraci = new List<Igrac>();

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
    }

    [HttpGet]
    public IActionResult GetData()
    {
        return Ok(igraci);
    }

    [HttpGet("{imeIgraca}")]
    public IActionResult GetIgracByImeIgraca(string imeIgraca)
    {
        if (string.IsNullOrEmpty(imeIgraca))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of Igrac");
        }

        var igrac = igraci.FirstOrDefault(i => i.ImeIgraca == imeIgraca);


        if (igrac != null)
        {
            List<Igrac> igracList = new List<Igrac> { igrac };
            return WrapperResponse.OkResponse("Fetched Igrac", igracList);
        }

        return WrapperResponse.NotFoundResponse("Igrac with the provided ImeIgraca doesn't exist HT Premijer Liga");
    }


    [HttpGet("imeKluba/{imeKluba}")]
    public IActionResult GetIgraciByKlub(string imeKluba)
    {
        if (string.IsNullOrEmpty(imeKluba))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of ImeKluba");
        }

        var igracList = igraci.Where(i => i.ImeKluba == imeKluba).ToList();

        if (igracList.Any())
        {
            return WrapperResponse.OkResponse("Fetched Igraci", igracList);
        }

        return WrapperResponse.NotFoundResponse("Klub with the provided ImeKluba doesn't exist in HT Premijer Liga");
    }

    [HttpGet("imeGrada/{imeGrada}")]
    public IActionResult GetIgraciByGrad(string imeGrada)
    {
        if (string.IsNullOrEmpty(imeGrada))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of ImeGrada");
        }

        var igracList = igraci.Where(i => i.Grad == imeGrada).ToList();

        if (igracList.Any())
        {
            return WrapperResponse.OkResponse("Fetched Igraci", igracList);
        }

        return WrapperResponse.NotFoundResponse("Grad with the provided ImeGrada doesn't exist in HT Premijer Liga");
    }

    [HttpGet("osnovan/{osnovan}")]
    public IActionResult GetIgraciByGodinaOsnutka(string osnovan)
    {
        if (!int.TryParse(osnovan, out int godinaOsnutka))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of GodinaOsnutka");
        }

        var igracList = igraci.Where(i => i.Osnovan == godinaOsnutka).ToList();

        if (igracList.Any())
        {
            return WrapperResponse.OkResponse("Fetched Igraci", igracList);
        }

        return WrapperResponse.NotFoundResponse("Igraci with the provided GodinaOsnutka doesn't exist HT Premijer Liga");
    }

    [HttpPost("{imeKluba}/add")]
    public async Task<IActionResult> AddIgracToKlub(string imeKluba, [FromBody] string igrac)
    {
        if (string.IsNullOrEmpty(imeKluba))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of ImeKluba");
        }

        var klubToUpdate = _dbContext.Klubovi.Find(k => k.ime == imeKluba).FirstOrDefault();
        if (klubToUpdate == null)
        {
            return WrapperResponse.NotFoundResponse("Klub with the provided ImeKluba doesn't exist HT Premijer Liga");
        }

        var filter = Builders<Klubovi>.Filter.Eq(k => k.ime, imeKluba);
        var update = Builders<Klubovi>.Update.AddToSet(k => k.igraci, igrac);

        var result = await _dbContext.Klubovi.UpdateOneAsync(filter, update);

        if (result.ModifiedCount > 0)
        {
            var klub = _dbContext.Klubovi.Find(k => k.ime == imeKluba).FirstOrDefault();
            Klubovi k = klub;
            return WrapperResponse.CreatedResponse();
        }
        else
        {
            return WrapperResponse.NotFoundResponse("Klub with the provided ImeKluba doesn't exist HT Premijer Liga");
        }
    }


    [HttpDelete("{imeIgraca}")]
    public async Task<IActionResult> DeleteDataAsync(string imeIgraca)
    {
        if (string.IsNullOrEmpty(imeIgraca))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of Igrac");
        }

        var igrac = igraci.FirstOrDefault(i => i.ImeIgraca == imeIgraca);

        if (igrac != null) { 

            var filter = Builders<Klubovi>.Filter.Eq(k => k.ime, igrac.ImeKluba) & Builders<Klubovi>.Filter.AnyEq(k => k.igraci, imeIgraca);
            var update = Builders<Klubovi>.Update.Pull(k => k.igraci, imeIgraca);

            var result = await _dbContext.Klubovi.UpdateOneAsync(filter, update);
            List<Igrac> igr = new List<Igrac> { igrac };
            return WrapperResponse.OkResponse("Igrac deleted", igr);

        }   
        else
        {
            return WrapperResponse.NotFoundResponse("Igrac with the provided ImeIgraca doesn't exist in the HT Premijer Liga");
        }
   
         
    }

    [HttpPut("{imeKluba}/{type}/edit")]
    public async Task<IActionResult> Edit(string imeKluba, string type, [FromBody] string edit)
    {
        var klubToUpdate = await _dbContext.Klubovi.Find(k => k.ime == imeKluba).FirstOrDefaultAsync();

        if (klubToUpdate == null)
        {
            return WrapperResponse.NotFoundResponse("Klub with the provided ImeKluba doesn't exist in the HT Premijer Liga");
        }

        var property = klubToUpdate.GetType().GetProperty(type);

        if (property == null)
        {
            return WrapperResponse.BadRequestResponse("Klubovi do not contain given attribute.");
        }

        property.SetValue(klubToUpdate, edit);

        await _dbContext.Klubovi.ReplaceOneAsync(k => k.ime == imeKluba, klubToUpdate);

        var klub = await _dbContext.Klubovi.Find(k => k.ime == imeKluba).FirstOrDefaultAsync();
        return WrapperResponse.OkPutResponse($"{type} updated successfully");

    }


    [HttpPatch]
    [HttpHead]
    [HttpOptions]
    public IActionResult NotSupported()
    {
        return WrapperResponse.NotImplementedResponse();
    }
}