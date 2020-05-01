using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContractWork.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace ContractWork.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRepository<Word> _repository;
        private static string _ServiceUrl = "http://localhost:7071/api";

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRepository<Word> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        static int Minimum(int a, int b) => a < b ? a : b;

        static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;

        [HttpGet("{w}")]
        public async Task<IActionResult> Get(string w)
        {
            // Check that w only contains letters
            if (!Regex.IsMatch(w, @"^[a-zA-Z]+$"))
            {
                return BadRequest();
            }

            var words = _repository.GetAll();
            var wordList = new List<WordDistance>();

            //A shorter array to test with. Test with word about (or abut if you want it misspelled to get some results)

            var newWords = words.Take<Word>(10);

            foreach (var dbWord in newWords)
            {
                var word = dbWord.word.ToLower();
                var passedWord = w.ToLower();

                // Check to see what word is the longest
                var longestWord = passedWord;
                if (word.Length > passedWord.Length)
                {
                    longestWord = word;
                }

                var distance = await GetWordDistance(passedWord, word);
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

        //Here is the task which calls the azure function. Now it needs to be published, and run in cloud, and we need a way to make it do it async
        private async Task<int> GetWordDistance(string passedWord, string dbWord)
        {
            int result;
            RestClient client = new RestClient();
            client.BaseUrl = new Uri(_ServiceUrl + "/GetDistance");

            var req = new RestRequest();

            req.AddParameter("word", passedWord);
            req.AddParameter("dbWord", dbWord);

            var res = await client.ExecuteAsync(req);

            int.TryParse(res.Content,out result);
            return result;
            
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
