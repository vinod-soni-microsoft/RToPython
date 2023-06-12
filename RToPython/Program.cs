using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace RToPython
{
    public class Choice
    {
        public string text { get; set; }
        public int index { get; set; }
        public string finish_reason { get; set; }
        public object logprobs { get; set; }
    }

    public class Root
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }
    }

    public class Usage
    {
        public int completion_tokens { get; set; }
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    class Program
    {
        static string sourceLanguage = "R";
        static string targetLanguage = "Python";
        static void Main(string[] args)
        {
            try
            {

                //Read from a file
                string something = File.ReadAllText("C:\\Users\\vinodsoni\\demo\\sourceLanguage.txt");

                string openaiResponse =  ConvertMyCode(something).Result;

                //Write to a file
                using (StreamWriter writer = new StreamWriter("C:\\Users\\vinodsoni\\demo\\targetLanguage.txt"))
                {
                    writer.WriteLine(openaiResponse);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
            }
        }

        static async Task<string> ConvertMyCode(string prompt1)
        {
            prompt1 = "Covert this "+sourceLanguage+" language code into "+targetLanguage+" '" + prompt1 + "'";
            HttpClient _httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(new
            {
                prompt = prompt1,
                temperature = 0.9,
                max_tokens = 3670,
                model = "text-davinci-003"
            });

            _httpClient = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Add("<header>", "<key>");

            var response = await _httpClient.PostAsync("<azureopenaiurl>", content);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            Root result = JsonConvert.DeserializeObject<Root>(responseJson);
            return result.choices[0].text;
            //JObject result = JsonConvert.DeserializeObject<dynamic>(responseJson);
            //result.TryGetValue("choices", out JToken choicesToken);  
            //return choicesToken[0]["text"].Value<string>();


        }
    }


}