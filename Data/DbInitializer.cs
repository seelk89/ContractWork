using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContractWork.Data
{
    public class DbInitializer : IDbInitializer
    {
        public void Initialize(Context context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Words.Any())
            {
                return; // DB has been seeded
            }

            // For testing
            List<Word> words = new List<Word>
            {
                new Word { id = 1, word = "able"}
            };

            //context.Words.AddRange(words);
            context.Words.AddRange(GetWordsFromFile("top1000.txt"));
            context.SaveChanges();
        }

        // Reads txt file line by line and adds them to a string list, returning said list
        private List<Word> GetWordsFromFile(string path)
        {
            List<Word> words = new List<Word>();
            string line;

            // Read the contents of the file into a stream
            using (System.IO.StreamReader file = new System.IO.StreamReader(path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    words.Add(new Word {
                        word = line
                    });
                }
            }

            return words;
        }
    }
}
