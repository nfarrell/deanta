
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deanta.Cosmos.DataLayer.EfClassesSql;
using Newtonsoft.Json;

namespace Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete
{
    public static class TodoJsonLoader
    {
        private const decimal DefaultTodoPrice = 40;    //Any todo without a price is set to this value

        public static IEnumerable<Todo> LoadTodos(string fileDir, string fileSearchString)
        {
            var filePath = GetJsonFilePath(fileDir, fileSearchString);
            var jsonDecoded = JsonConvert.DeserializeObject<ICollection<TodoInfoJson>>(File.ReadAllText(filePath));

            var authorDict = new Dictionary<string,Owner>();
            foreach (var todoInfoJson in jsonDecoded)
            {
                foreach (var author in todoInfoJson.Owners)
                {
                    if (!authorDict.ContainsKey(author))
                        authorDict[author] = new Owner{ Name = author};
                }
            }

            return jsonDecoded.Select(x => CreateTodoWithRefs(x, authorDict));
        }
        //--------------------------------------------------------------
        //private methods
        private static Todo CreateTodoWithRefs(TodoInfoJson todoInfoJson, Dictionary<string, Owner> authorDict)
        {
            var owners = todoInfoJson.Owners.Select(x => authorDict[x]).ToList();
            var todo = Todo.CreateTodo(todoInfoJson.Title, owners).Result;

            return todo;
        }

        /// <summary>
        /// This create the right number of NumStars that add up to the average rating
        /// </summary>
        /// <param name="averageRating"></param>
        /// <param name="ratingsCount"></param>
        /// <returns></returns>
        private static List<int> CalculateCommentsToMatch(double averageRating, int ratingsCount)
        {
            var numStars = new List<int>();
            var currentAve = averageRating;
            for (var i = 0; i < ratingsCount; i++)
            {
                numStars.Add( (int)( currentAve > averageRating ? Math.Truncate(averageRating) : Math.Ceiling(averageRating)));
                currentAve = numStars.Average();
            }
            return numStars;
        }

        private static DateTime DecodeCreatedDate(string createdDate)
        {
            var split = createdDate.Split('-');
            return split.Length switch
            {
                1 => new DateTime(int.Parse(split[0]), 1, 1),
                2 => new DateTime(int.Parse(split[0]), int.Parse(split[1]), 1),
                3 => new DateTime(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2])),
                _ => throw new InvalidOperationException(
                    $"The json createdDate failed to decode: string was {createdDate}")
            };
        }

        private static string GetJsonFilePath(string fileDir, string searchPattern)
        {
            var fileList = Directory.GetFiles(fileDir, searchPattern);

            if (fileList.Length == 0)
                throw new FileNotFoundException($"Could not find a file with the search name of {searchPattern} in directory {fileDir}");

            //If there are many then we take the most recent
            return fileList.ToList().OrderBy(x => x).Last();
        }
    }
}