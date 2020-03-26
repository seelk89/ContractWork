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
            // Check that w only contains letters
            if (!Regex.IsMatch(w, @"^[a-zA-Z]+$"))
            {
                return BadRequest();
            }

            var distanceMax = w.Length / 2;
            var wordList = new List<string>();

            // Get words from db
            using (SQLiteConnection c = new SQLiteConnection("Data Source=WordList.db;"))
            {
                using (var command = new SQLiteCommand(c))
                {
                    c.Open();

                    command.CommandText = "SELECT * FROM Words";      

                    using (SQLiteDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            wordList.Add(rdr.GetString(1).ToLower());      
                        }
                    }                    
                }
            }

            foreach (var word in wordList)
            {
                if (!w.Equals(word)) 
                {
                    var combinedDistance = CheckWordDistanceByRemove(word);
                }

                var wordDistance = new WordDistance
                {
                    Word = word,
                    Distance = 0
                };
            }

            return Ok(wordList);
        }

        private int CheckWordDistanceByRemove(string word) 
        {
            var letters = word.ToCharArray();
            for (int i = 0; i < letters.Length; i++)
            {

            }

            return 0;
        }

        public class WordDistance
        {
            public string Word { get; set; }
            public int Distance { get; set; }
        }
    }
}
