using System;
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
                            var dbWord = rdr.GetString(1).ToLower();

                            if (CheckWordIsWithinDistance(w.ToLower(), dbWord)) 
                            {
                                wordList.Add(dbWord);
                            }                     
                        }
                    }                    
                }
            }

            return Ok(wordList);
        }

        private bool CheckWordIsWithinDistance(string passedWord, string dbWord) 
        {
            var passedLetters = new List<char>();
            passedLetters.AddRange(passedWord);

            var dbLetters = new List<char>();
            dbLetters.AddRange(dbWord);

            // Check to see what word is the longest
            var longestWord = passedLetters;
            if (dbLetters.Count > passedLetters.Count)
            {
                longestWord = dbLetters;
            }

            var i = 0;
            // Check letter in dbWord is not contained in passedWord
            foreach (var c in passedLetters)
            {
                if (!dbLetters.Contains(c)) 
                {
                    i += 1;
                }
            }

            // Returns true if dbWord is within the boundaries of allowed distance
            if (Math.Floor(Convert.ToDouble(longestWord.Count) / 2) >= i) 
            {
                return true;
            }

            return false;
        }

        public class WordDistance
        {
            public string Word { get; set; }
            public int Distance { get; set; }
        }
    }
}
