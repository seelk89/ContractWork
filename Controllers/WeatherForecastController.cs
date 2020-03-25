using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContractWork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{w}")]
        public IActionResult Get(string w)
        {
            var distanceMax = w.Length / 2;

            if (!Regex.IsMatch(w, @"^[a-zA-Z]+$"))
            {
                return BadRequest();
            }

            using (SQLiteConnection c = new SQLiteConnection("Data Source=WordList.db;"))
            {
                using (var command = new SQLiteCommand(c))
                {
                    c.Open();

                    command.CommandText = "SELECT * FROM Words";

                    var wordList = new List<string>();

                    using (SQLiteDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            wordList.Add(rdr.GetString(1).ToLower());      
                        }
                    }

                    return Ok(wordList);
                }
            }
        }



        public class WordDistance
        {
            public string Word { get; set; }
            public int Distance { get; set; }
        }
    }
}
