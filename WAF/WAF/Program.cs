using DataAccess;
using libsvm;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WAF
{
    class Program
    {
        private static Dictionary<int, string> _predictionDictionary = new Dictionary<int, string> { { -1, "SqlInjection" }, { 1, "SafeRequest" } };

        static void Main(string[] args)
        {

            DataPreparer data = new DataPreparer();

            var problemBuilder = new TextClassificationProblemBuilder();
            var problem = problemBuilder.CreateProblem(data.RequestText, data.ClassValue, data.Vocabulary.ToList());

            const double C = 0.5;
            var model = new C_SVC(problem, KernelHelper.LinearKernel(), C); // Train is called automatically here
            var accuracy = model.GetCrossValidationAccuracy(100);

            Console.Clear();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Accuracy of the model is {0:P}", accuracy);

            model.Export(string.Format(@"C:\Users\kramek\Desktop\AIC#\model_{0}_accuracy.model", accuracy));

            Console.WriteLine(new string('=', 50));
            Console.WriteLine("The Model is ready. \r\nEnter a request to check:");
            Console.WriteLine(new string('=', 50));

            string userInput;

            do
            {
                userInput = Console.ReadLine(); // SeparateNonAlphanumeric(Console.ReadLine());//
                var newX = TextClassificationProblemBuilder.CreateNode(userInput, data.Vocabulary);
                //var predictedYProb = model.PredictProbabilities(newX);
                var predictedY = model.Predict(newX);

                Console.WriteLine("The prediction is {0}", _predictionDictionary[(int)predictedY]);
                Console.WriteLine(new string('=', 50));

            } while (userInput != "exit");

            Console.WriteLine("");

        }

        private static string SeparateNonAlphanumeric(string line)
        {
            string result = "";

            for (int i = 0; i < line.Length; i++)
            {
                if (Char.IsLetter(line[i]) || Char.IsNumber(line[i]))
                {
                    result += line[i];
                }
                else
                {
                    result += ' ';
                    result += line[i];
                    result += ' ';
                }

            }

            return result;
        }
    }
}
