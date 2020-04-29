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

        readonly String[] Words = { "the", "of", "and", "a", "to", "in", "is", "you", "that", "it", "he", "was", "for", "on", "are", "as", "with", "his", "they", "I", "at", "be", "this", "have", "from", "or", "one", "had", "by", "word", "but", "not", "what", "all", "were", "we", "when", "your", "can", "said", "there", "use", "an", "each", "which", "she", "do", "how", "their", "if", "will", "up", "other", "about", "out", "many", "then", "them", "these", "so", "some", "her", "would", "make", "like", "him", "into", "time", "has", "look", "two", "more", "write", "go", "see", "number", "no", "way", "could", "people", "my", "than", "first", "water", "been", "call", "who", "oil", "its", "now", "find", "long", "down", "day", "did", "get", "come", "made", "may", "part" };

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

            foreach (var arrayWord in Words)
            {
                var word = arrayWord.ToLower();
                var passedWord = w.ToLower();

                // Check to see what word is the longest
                var longestWord = passedWord;
                if (word.Length > passedWord.Length)
                {
                    longestWord = word;
                }

                var distance = CheckWordIsWithinDistance(passedWord, word);
                if (Math.Floor(Convert.ToDouble(longestWord.Length) / 2) >= distance)
                {
                    wordList.Add(new WordDistance
                    {
                        Word = word,
                        Distance = distance
                    });
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
