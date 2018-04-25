using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace script
{
    class Program
    {
        static void Main(string[] args)
        {
            int iterations = 5;

            var standaloneTimes = new List<double>();
            var replica_defaultTimes = new List<double>();
            var replica1_w1jfTimes = new List<double>();
            var replica1_w1jtTimes = new List<double>();
            var replica1_w2jfTimes = new List<double>();
            var replica1_w2jtTimes = new List<double>();

            Console.WriteLine("standalone");

            for (int i = 0; i < iterations; i++)
            {
                var number = i + 1;
                Console.Write("Executing iteration number: " + number);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                ExecuteCommandSync("mongoimport -d test -c head --type csv --file data.csv --headerline --drop");
                watch.Stop();
                standaloneTimes.Add(msToS(watch.ElapsedMilliseconds));
            }

            CreateFile("standalone.md", BuildResultContent(standaloneTimes));

            Console.WriteLine("replica_default");
            for (int i = 0; i < iterations; i++)
            {
                var number = i + 1;
                Console.Write("Executing iteration number: " + number);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                ExecuteCommandSync("mongoimport --port 27001 --host localhost -d test -c head --type csv --file data.csv --headerline --drop");
                watch.Stop();
                replica_defaultTimes.Add(msToS(watch.ElapsedMilliseconds));
            }

            CreateFile("replica_default.md", BuildResultContent(replica_defaultTimes));

            Console.WriteLine("replica_w1jf");
            for (int i = 0; i < iterations; i++)
            {
                var number = i + 1;
                Console.Write("Executing iteration number: " + number);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                ExecuteCommandSync("mongoimport --port 27001 --host localhost -d test -c head --type csv --file data.csv --headerline --drop --writeConcern '{w:1,j:false,wtimeout:500}'");
                watch.Stop();
                replica1_w1jfTimes.Add(msToS(watch.ElapsedMilliseconds));
            }

            CreateFile("replica_w1jf.md", BuildResultContent(replica1_w1jfTimes));

            Console.WriteLine("replica_w1jt");
            for (int i = 0; i < iterations; i++)
            {
                var number = i + 1;
                Console.Write("Executing iteration number: " + number);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                ExecuteCommandSync("mongoimport --port 27001 --host localhost -d test -c head --type csv --file data.csv --headerline --drop --writeConcern '{w:1,j:true,wtimeout:500}'");
                watch.Stop();
                replica1_w1jtTimes.Add(msToS(watch.ElapsedMilliseconds));
            }

            CreateFile("replica_w1jt.md", BuildResultContent(replica1_w1jtTimes));

            Console.WriteLine("replica_w2jf");
            for (int i = 0; i < iterations; i++)
            {
                var number = i + 1;
                Console.Write("Executing iteration number: " + number);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                ExecuteCommandSync("mongoimport --port 27001 --host localhost -d test -c head --type csv --file data.csv --headerline --drop --writeConcern '{w:2,j:false,wtimeout:0}'");
                watch.Stop();
                replica1_w2jfTimes.Add(msToS(watch.ElapsedMilliseconds));
            }

            CreateFile("replica_w2jf.md", BuildResultContent(replica1_w2jfTimes));

            Console.WriteLine("replica_w2jt");
            for (int i = 0; i < iterations; i++)
            {
                var number = i + 1;
                Console.Write("Executing iteration number: " + number);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                ExecuteCommandSync("mongoimport --port 27001 --host localhost -d test -c head --type csv --file data.csv --headerline --drop --writeConcern '{w:2,j:true,wtimeout:0}'");
                watch.Stop();
                replica1_w2jtTimes.Add(msToS(watch.ElapsedMilliseconds));
            }

            CreateFile("replica_w2jt.md", BuildResultContent(replica1_w2jtTimes));

            Console.Write("End");
            Console.Read();
        }

        public static void ExecuteCommandSync(object command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                Console.WriteLine(objException);
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

        public static string BuildResultContent(List<double> values)
        {
            string header = "| Iteration | Time |\n";
            string line = "| ---- | ---- |\n";

            string result = header + line;

            for(int i = 0; i < values.Count; i++)
            {
                result += $"| {i+1} | {values[i]} |\n";
            }

            result += $"| MEAN | {values.Average()} |\n";

            return result;
        }

        public static double msToS(long ms)
        {
            return ms * 0.001;
        }
    }
}
