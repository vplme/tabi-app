using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using McMaster.Extensions.CommandLineUtils;
using Tabi.DataObjects;
using Tabi.Logic;

namespace Tabi.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();
            var optionSubject = app.Option("-s|--subject <SUBJECT>", "The subject", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var subject = optionSubject.HasValue()
                    ? optionSubject.Value()
                    : "world";

                ReadCsv();

                return 0;
            });

            return app.Execute(args);
        }

        private static void ReadCsv()
        {

            IEnumerable<ResolvedStop> stops = null;
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("data.csv"))
                {
                    var csv = new CsvReader(sr);
                    csv.Configuration.RegisterClassMap<PositionEntryMap>();
                    var records = csv.GetRecords<PositionEntry>().ToList();

                    StopResolver stopResolver = new StopResolver(TimeSpan.FromMinutes(5), 100, 400, 50, 200);
                    stops = stopResolver.ResolveStops(records);
                    Console.WriteLine($"Stops found: {stops.Count()}");
                }

                using (StreamWriter textWriter = new StreamWriter("export.csv"))
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    csvWriter.Configuration.RegisterClassMap<ResolvedStopMap>();
                    csvWriter.Configuration.QuoteAllFields = true;

                    csvWriter.WriteRecords(stops);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
