using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using labos2.Models;

namespace labos2.Controllers
{

    [ApiController]
    [Route("api/klubovi")]
    public class KluboviController : ControllerBase
    {

        private readonly labos2.Data.DbContext _dbContext;
        private List<Klub> klubovi = new List<Klub>();

        public KluboviController(labos2.Data.DbContext dbContext)
        {
            _dbContext = dbContext;

            var cursor = _dbContext.Klubovi.Find(Builders<Klubovi>.Filter.Empty);
            foreach (var klub in cursor.ToEnumerable())
            {
                Klub klub1 = new Klub();

                klub1.id = klub.id.ToString();
                klub1.ime = klub.ime;
                klub1.osnovan = klub.osnovan;
                klub1.grad = klub.grad;
                klub1.dvorana = klub.dvorana;
                klub1.kapacitet = klub.kapacitet;
                klub1.trener = klub.trener;
                klub1.kapetan = klub.kapetan;
                klub1.navijaci = klub.navijaci;

                List<string> list = new List<string>();
                if (klub.igraci != null)
                {

                    foreach (var i in klub.igraci)
                    {

                        list.Add(i.ime);
                    }
                    klub1.igraci = list;
                }
                else
                {
                    klub1.igraci = null;
                }
               
                

                klubovi.Add(klub1);

            }
        }

        [HttpGet]
        public IActionResult GetData()
        {
            return Ok(klubovi);
        }

        [HttpGet("extract")]
        public IActionResult ExtractedData()
        {
            var extractedData = klubovi.Select(k => new { Id = k.id, ImeKluba = k.ime, osnovan = k.osnovan, grad = k.grad, dvorana = k.dvorana, kapacitet = k.kapacitet, trener = k.trener, kapetan = k.kapetan, navijaci = k.navijaci  });
            return Ok(extractedData);
        }

        [HttpGet("{ime}")]
        public IActionResult GetKlubByIme(string ime)
        {
            if (string.IsNullOrEmpty(ime))
            {
                return WrapperResponse.BadRequestResponse("Invalid type of ImeKluba");
            }

            var klub = klubovi.FirstOrDefault(i => i.id.ToString() == ime);


            if (klub != null)
            {
                return Ok(klub);
            }

            return WrapperResponse.NotFoundResponse("Klub with the provided ime doesn't exist HT Premijer Liga");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return WrapperResponse.BadRequestResponse("Invalid type of Id");
            }

            var filter = Builders<Klubovi>.Filter.Eq(k => k.id, ObjectId.Parse(id));
            var result = await _dbContext.Klubovi.DeleteOneAsync(filter);

            return Ok();

           
        }



        [HttpPost]
        public async Task<IActionResult> AddKlub([FromBody] AddKlub dto)
        {
            if (dto == null )
            {
                return WrapperResponse.BadRequestResponse("Invalid data");
            }

            var klub = new Klubovi
            {
                id = ObjectId.GenerateNewId(),
                ime = dto.ime,
                osnovan = dto.osnovan,
                grad = dto.grad,
                dvorana = dto.dvorana,
                kapacitet = dto.kapacitet,
                trener = dto.trener,
                kapetan = dto.kapetan,
                navijaci = dto.navijaci
            };


             await _dbContext.Klubovi.InsertOneAsync(klub);
 
             return WrapperResponse.CreatedResponse();
   
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] UpdateKlub updKlub)
        {
            var kl = klubovi.FirstOrDefault(i => i.id.ToString() == id);

            if (kl == null)
            {
                return WrapperResponse.NotFoundResponse("klub with the provided Id doesn't exist in the HT Premijer Liga");
            }

            var klubToUpdate = await _dbContext.Klubovi.Find(k => k.id.ToString() == id).FirstOrDefaultAsync();


            var filter = Builders<Klubovi>.Filter.Eq(k => k.ime, klubToUpdate.ime);
            var update = Builders<Klubovi>.Update
                .Set(k => k.ime, updKlub.ime)
                .Set(k => k.osnovan, updKlub.osnovan)
                .Set(k => k.grad, updKlub.grad)
                .Set(k => k.dvorana, updKlub.dvorana)
                .Set(k => k.kapacitet, updKlub.kapacitet)
                .Set(k => k.trener, updKlub.trener)
                .Set(k => k.kapetan, updKlub.kapetan)
                .Set(k => k.navijaci, updKlub.navijaci);

            await _dbContext.Klubovi.UpdateOneAsync(filter, update);


            return WrapperResponse.OkPutResponse("Klub updated successfully");
        }


    }
}