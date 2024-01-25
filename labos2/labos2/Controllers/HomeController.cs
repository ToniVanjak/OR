using Azure;
using CsvHelper;
using labos2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace labos2.Controllers;
[ApiController]
[Route("api/igraci")]
public class HomeController : ControllerBase
{
    private readonly labos2.Data.DbContext _dbContext;
    private List<Igrac> igraci;
    private List<Klubovi> klubovi = new List<Klubovi>();

    public HomeController(labos2.Data.DbContext dbContext)
    {
        _dbContext = dbContext;

        igraci = new List<Igrac>();

        var cursor = _dbContext.Klubovi.Find(Builders<Klubovi>.Filter.Empty);
        foreach (var klub in cursor.ToEnumerable())
        {
            klubovi.Add(klub);

            if (klub.igraci != null)
            {
                foreach (var igrac in klub.igraci)
                {
                    Igrac i = new Igrac();
                    i.Id = igrac.ID.ToString();
                    i.ImeKluba = klub.ime;
                    i.ImeIgraca = igrac.ime;
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
    }

    [HttpGet]
    public IActionResult GetData()
    {
        return Ok(igraci);
    }

    [HttpGet("extract")]
    public IActionResult ExtractedData()
    {
        var extractedData = igraci.Select(igrac => new { Id = igrac.Id, ImeKluba = igrac.ImeKluba, ImeIgraca = igrac.ImeIgraca });
        return Ok(extractedData);
    }



    [HttpGet("{id}")]
    public IActionResult GetIgracByImeIgraca(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of Id");
        }

        var igrac = igraci.FirstOrDefault(i => i.Id == id);


        if (igrac != null)
        {
            List<Igrac> igracList = new List<Igrac> { igrac };
            return WrapperResponse.OkResponse("Fetched Igrac", igracList);
        }

        return WrapperResponse.NotFoundResponse("Igrac with the provided Id doesn't exist HT Premijer Liga");
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

    [HttpPost]
    public async Task<IActionResult> AddIgracToKlub([FromBody] AddIgracToKlubDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Ime) || string.IsNullOrEmpty(dto.Klub))
        {
            return WrapperResponse.BadRequestResponse("Invalid data");
        }

        var klubToUpdate = await _dbContext.Klubovi
            .Find(k => k.ime == dto.Klub)
            .FirstOrDefaultAsync();

        if (klubToUpdate == null)
        {
            return WrapperResponse.NotFoundResponse($"Klub with the provided ImeKluba '{dto.Klub}' doesn't exist in HT Premijer Liga");
        }

        var igrac = new IGRAC
        {
            ID = ObjectId.GenerateNewId(),
            ime = dto.Ime
        };

        var filter = Builders<Klubovi>.Filter.Eq(k => k.ime, dto.Klub);
        var update = Builders<Klubovi>.Update.AddToSet(k => k.igraci, igrac);

        var result = await _dbContext.Klubovi.UpdateOneAsync(filter, update);

        if (result.ModifiedCount > 0)
        {
            return WrapperResponse.CreatedResponse();
        }
        else
        {
            return WrapperResponse.NotFoundResponse($"Klub with the provided ImeKluba '{dto.Klub}' doesn't exist in HT Premijer Liga");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDataAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return WrapperResponse.BadRequestResponse("Invalid type of Id");
        }

        var igrac = igraci.FirstOrDefault(i => i.Id == id);

        if (igrac != null)
        {
            var klubFilter = Builders<Klubovi>.Filter.Eq(k => k.ime, igrac.ImeKluba);
            var update = Builders<Klubovi>.Update.PullFilter(k => k.igraci, i => i.ID == ObjectId.Parse(id));
            var result = await _dbContext.Klubovi.UpdateOneAsync(klubFilter, update);

            if (result.ModifiedCount > 0)
            {
                List<Igrac> igr = new List<Igrac> { igrac };
                return WrapperResponse.OkResponse("Igrac deleted", igr);
            }
            else
            {
                return WrapperResponse.NotFoundResponse("Igrac with the provided Id doesn't exist in the HT Premijer Liga");
            }
        }
        return WrapperResponse.NotFoundResponse("Igrac with the provided Id doesn't exist in the HT Premijer Liga");
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(string id, [FromBody] string newName)
    {
        var igrac = igraci.FirstOrDefault(i => i.Id == id);

        if (igrac == null)
        {
            return WrapperResponse.NotFoundResponse("Igrac with the provided Id doesn't exist in the HT Premijer Liga");
        }

        var klubToUpdate = await _dbContext.Klubovi.Find(k => k.ime == igrac.ImeKluba).FirstOrDefaultAsync();

        var igracToUpdate = klubToUpdate.igraci.FirstOrDefault(i => i.ID == ObjectId.Parse(id));

        if (igracToUpdate == null)
        {
            return WrapperResponse.NotFoundResponse("Igrac with the provided Id doesn't exist in the igraci list of the Klub");
        }

        igracToUpdate.ime = newName;

        var filter = Builders<Klubovi>.Filter.Eq(k => k.ime, klubToUpdate.ime);
        var update = Builders<Klubovi>.Update.Set(k => k.igraci, klubToUpdate.igraci);

        await _dbContext.Klubovi.UpdateOneAsync(filter, update);

        return WrapperResponse.OkPutResponse("Igrac with provided Id updated successfully.");
    }


    [HttpPatch]
    [HttpHead]
    [HttpOptions]
    public IActionResult NotSupported()
    {
        return WrapperResponse.NotImplementedResponse();
    }

    [Route("login")]
    public async Task Login()
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            // Indicate here where Auth0 should redirect the user after a login.
            // Note that the resulting absolute Uri must be added to the
            // **Allowed Callback URLs** settings for the app.
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    [Authorize]
    [Route("logout")]
    public async Task Logout()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
              // Indicate here where Auth0 should redirect the user after a logout.
              // Note that the resulting absolute Uri must be whitelisted in 
              .WithRedirectUri("")
                .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [Authorize]
    [Route("profile")]
    public async Task<IActionResult> Profile()
    {

        return Ok(new UserProfile()
        {
            Name = User?.Identity?.Name,
            Nickname = User?.Claims?.FirstOrDefault(c => c.Type == "nickname")?.Value,
            ProfileImage = User?.Claims?.FirstOrDefault(c => c.Type == "picture")?.Value
        });
    }

    [Route("authenticated")]
    public async Task<IActionResult> Authenticated()
    {
        return Ok(User?.Identity?.IsAuthenticated == true);
    }
}