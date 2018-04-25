using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace zaliczenie
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("test");
            var collection = database.GetCollection<Incident>("head");

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "daysofweek":
                        DaysOfWeek(collection);
                        break;
                    case "index":
                        Index(collection);
                        break;
                    case "justice":
                        Justice(collection, args.Last());
                        break;
                    default:
                        Console.Write("Wrong argument");
                        break;
                }
            }
        }

        public static void Justice(IMongoCollection<Incident> collection, string co)
        {
            var filter = new BsonDocument { { "Address", new BsonDocument { { "$regex", "BRYANT" }, { "$options", "i" } } } };
            var text = collection.Find(filter).Limit(1000).ToList();
            Console.WriteLine("Docs saved to justice.json");
            CreateFile("justice.json", JsonConvert.SerializeObject(text));
        }

        public static void Index(IMongoCollection<Incident> collection)
        {
            collection.Indexes.DropAll();
            Console.Write("Bez indeksu ");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            collection.Find(x => x.IncidntNum > 180263596)
                      .Sort(Builders<Incident>.Sort.Descending("IncidntNum"))
                      .ToList();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + "ms");

            collection.Indexes.DropAll();

            Console.WriteLine("Z indeksem ");
            collection.Indexes.CreateOne(Builders<Incident>.IndexKeys.Descending(_ => _.IncidntNum));
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            collection.Find(x => x.IncidntNum > 180263596)
                      .Sort(Builders<Incident>.Sort.Ascending("IncidntNum")).ToList();
            watch2.Stop();
            Console.WriteLine(watch2.ElapsedMilliseconds + "ms");

            collection.Indexes.DropAll();
        }

        public static void DaysOfWeek(IMongoCollection<Incident> collection)
        {
            var group = new BsonDocument
            {
                {"_id", "$DayOfWeek"},
                {"count", new BsonDocument {{"$sum", 1}}}
            };

            var DaysOfWeek = collection.Aggregate().Group(group).As<DaysOfWeek>().ToList();

            DrawPieChart(DaysOfWeek);
            CreateFile("daysofweek.md", BuildResultContent(DaysOfWeek));
        }

        public static void DrawPieChart(List<DaysOfWeek> data)
        {
            using (var chart1 = new Chart())
            {
                chart1.ChartAreas.Add(new ChartArea());

                chart1.Legends.Add("Legenda");
                chart1.Legends[0].LegendStyle = LegendStyle.Table;
                chart1.Legends[0].Docking = Docking.Bottom;
                chart1.Legends[0].Alignment = StringAlignment.Center;
                chart1.Legends[0].Title = "DaysOfWeek";
                chart1.Legends[0].BorderColor = Color.Black;

                string seriesname = "MySeriesName";
                chart1.Series.Add(seriesname);

                chart1.Series[seriesname].ChartType = SeriesChartType.Pie;

                foreach (var item in data)
                {
                    chart1.Series[seriesname].Points.AddXY(item._id, item.count);
                }

                chart1.SaveImage("chart.png", ChartImageFormat.Jpeg);
            }
        }

        public static void CreateFile(string name, string content)
        {
            var path = System.AppDomain.CurrentDomain.BaseDirectory + name;
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Create(path).Dispose();

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.Write(content);
            }
        }

        public static string BuildResultContent(List<DaysOfWeek> values)
        {
            string header = "| Day | Count | Percentage |\n";
            string line = "| ---- | ---- | ---- |\n";

            string result = header + line;
            int sum = 0;

            foreach (var item in values)
            {
                sum += item.count;
            }

            for (int i = 0; i < values.Count; i++)
            {
                decimal percentage = ((decimal)values[i].count / (decimal)sum) * 100m;
                result += $"| {values[i]._id} | {values[i].count} | {Math.Round(percentage, 2)}% |\n";
            }

            return result;
        }
    }
}
