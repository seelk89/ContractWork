using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
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

        static int Minimum(int a, int b) => a < b ? a : b;

        static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;

        [HttpGet("{w}")]
        public IActionResult Get(string w)
        {
            // Check that w only contains letters
            if (!Regex.IsMatch(w, @"^[a-zA-Z]+$"))
            {
                return BadRequest();
            }

            var wordList = new List<WordDistance>();

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
                            var passedWord = w.ToLower();

                            // Check to see what word is the longest
                            var longestWord = passedWord;
                            if (dbWord.Length > passedWord.Length)
                            {
                                longestWord = dbWord;
                            }

                            var distance = CheckWordIsWithinDistance(passedWord, dbWord);
                            if (Math.Floor(Convert.ToDouble(longestWord.Length) / 2) >= distance)
                            {
                                wordList.Add(new WordDistance
                                {
                                    Word = dbWord,
                                    Distance = distance
                                });
                            }
                        }
                    }
                }
            }

            if (!wordList.Any())
            {
                return StatusCode(204);
            }

            return Ok(wordList.OrderBy(w => w.Distance).ToList());
        }

        static int CheckWordIsWithinDistance(string passedWord, string dbWord)
        {
            var n = passedWord.Length + 1;
            var m = dbWord.Length + 1;
            var arrayD = new int[n, m];

            for (var i = 0; i < n; i++)
            {
                arrayD[i, 0] = i;
            }

            for (var j = 0; j < m; j++)
            {
                arrayD[0, j] = j;
            }

            for (var i = 1; i < n; i++)
            {
                for (var j = 1; j < m; j++)
                {
                    var cost = passedWord[i - 1] == dbWord[j - 1] ? 0 : 1;

                    arrayD[i, j] = Minimum(arrayD[i - 1, j] + 1, // Delete
                                           arrayD[i, j - 1] + 1, // Insert
                                           arrayD[i - 1, j - 1] + cost); // Change

                    if (i > 1 && j > 1
                       && passedWord[i - 1] == dbWord[j - 2]
                       && passedWord[i - 2] == dbWord[j - 1])
                    {
                        arrayD[i, j] = Minimum(arrayD[i, j],
                        arrayD[i - 2, j - 2] + cost); // Switch
                    }
                }
            }

            return arrayD[n - 1, m - 1];
        }

        public class WordDistance
        {
            public string Word { get; set; }
            public int Distance { get; set; }
        }
    }
}
