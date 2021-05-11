using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestingCsv.Models;

namespace TestingCsv
{
    public class GenerateCsvData
    {
        public List<CsvData> Generate(int totalRecord)
        {
            List<CsvData> result = new List<CsvData>();
            for (int i = 0; i < totalRecord; i++)
            {
                result.Add(new CsvData
                {
                    ID = Guid.NewGuid(),
                    Content = RandomString(RandomNumber(1000, 2000)),

                });
            }
            return result;
        }

        private Random random = new Random();
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private int RandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
