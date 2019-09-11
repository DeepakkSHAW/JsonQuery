using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonQuery
{
    public class Shortie
    {
        public string Original { get; set; }
        public string Shortened { get; set; }
        public string Short { get; set; }
        public ShortieException Error { get; set; }
    }

    public class ShortieException
    {
        public int Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    
    class Program
    {
        internal void Approach1()
        {
            // Approach 1
            string json = @"{
  'channel': {
    'title': 'James Newton-King',
    'link': 'http://james.newtonking.com',
    'description': 'James Newton-King\'s blog.',
    'item': [
      {
        'title': 'Json.NET 1.3 + New license + Now on CodePlex',
        'description': 'Announcing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'CodePlex'
        ]
      },
      {
        'title': 'LINQ to JSON beta',
        'description': 'Announcing LINQ to JSON',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'LINQ'
        ]
      }
    ]
  }
}";
            JObject rss = JObject.Parse(json);

            string rssTitle = (string)rss["channel"]["title"];
            // James Newton-King

            string itemTitle = (string)rss["channel"]["item"][0]["title"];
            // Json.NET 1.3 + New license + Now on CodePlex

            JArray categories = (JArray)rss["channel"]["item"][0]["categories"];
            // ["Json.NET", "CodePlex"]

            IList<string> categoriesText = categories.Select(c => (string)c).ToList();
        }
        internal void Approach2()
        {
            // Approach 2
            string jsonText = @"{
  'short': {
    'original': 'http://www.foo.com/',
    'short': 'krehqk',
    'error': {
      'code': 0,
      'msg': 'No action taken'
    }
  }
}";

            JObject jsonObj = JObject.Parse(jsonText);

            Shortie shortie = new Shortie
            {
                Original = (string)jsonObj["short"]["original"],
                Short = (string)jsonObj["short"]["short"],
                Error = new ShortieException
                {
                    Code = (int)jsonObj["short"]["error"]["code"],
                    ErrorMessage = (string)jsonObj["short"]["error"]["msg"]
                }
            };

            Console.WriteLine(shortie.Original);
            Console.WriteLine(shortie.Error.ErrorMessage);
        }
        static void Main(string[] args)
        {

            Console.WriteLine("*** JSON Query Experimentation ***");
            float? acumHour = 0;
            float? compHour = 0;
            // Approach PayTime requirement
            List<TimeSheets> timeSheets = new List<TimeSheets>();
            List<TimeSheets> addTimeSheets = new List<TimeSheets>();
            try
            {
                using (StreamReader r = new StreamReader("timesheet.json"))
                {
                    string jsonTimesheets = r.ReadToEnd();
                    timeSheets = JsonConvert.DeserializeObject<List<TimeSheets>>(jsonTimesheets);
                    Console.WriteLine(timeSheets.Count);
                }
                //group by EmpCode & TiemSheetEntryDate
                // var a = items.GroupBy(x => x.EmpCode, y => y.TiemSheetEntryDate).Select(x => x);
                var grpTimeSheets = timeSheets.GroupBy(c => new { c.EmpCode, c.TimeSheetEntryDate }).Select(c => c);
                Console.WriteLine(grpTimeSheets.Count());

                foreach (var gtime in grpTimeSheets)
                {
                    System.Diagnostics.Debug.WriteLine(gtime.Key);
                    acumHour = gtime.Sum(c => c.Hours);
                    Console.WriteLine($"ID: {gtime.Key} total hrs: {acumHour}");

                    //minimum hours can't be less then 3 hrs for a time sheet entry
                    if (acumHour.HasValue)
                        if (acumHour < 3) //The day time sheet accumulated hours 
                        {
                            //Check if Department code is On-line i.e. 18 
                            if (gtime.Any(e => e.DepartmentCode == 18))
                            {
                                if (acumHour < 2)
                                {
                                   compHour = (float)Math.Round((decimal)(2 - acumHour) / gtime.Count(), 2, MidpointRounding.AwayFromZero);
                                    //Make the Hours to 2hrs 
                                    foreach (var itime in gtime)
                                    {
                                        Console.WriteLine($"The Old values:{itime.TimeSheetEntryId} { itime.Hours} - {itime.Rate}");
                                        itime.Rate = itime.MinRate;
                                        itime.Hours = compHour;
                                        addTimeSheets.Add(itime);
                                        Console.WriteLine($"The New values:{itime.TimeSheetEntryId} { itime.Hours} - {itime.Rate}");
                                    }
                                }
                                else
                                {
                                    // accumulated hours exceeds 2 hrs for Department Code "On-Line" >> all good do nothing
                                    Console.WriteLine($"Accumulated hours {acumHour} Hrs. exceeds 2 hrs minimum limit, for On-line Department Code.");
                                }
                            }
                            //Otherwise Check if Type is Higher Rates i.e. Type = 2
                            else if (gtime.Any(e => e.Type == 2))
                            {
                                if (acumHour < 2)
                                {
                                    //Make the Hours to 2hrs 
                                    foreach (var itime in gtime)
                                    {

                                        Console.WriteLine($"The Old: { itime.Hours} - {itime.Rate}");
                                        addTimeSheets.Add(itime);
                                    }

                                }
                                else
                                {
                                    // accumulated hours exceeds 2 hrs for Department Code "On-Line" >> all good do nothing
                                    Console.WriteLine($"Accumulated hours {acumHour} Hrs. exceeds 2 hrs minimum limit, for Higher rates.");
                                }
                            }
                            //Neither Type is Higher rate not Department belongs to
                            else
                            {
                                //Make the Hours to 3hrs 
                                foreach (var itime in gtime)
                                {

                                    Console.WriteLine($"The Old: { itime.Hours} - {itime.Rate}");
                                    addTimeSheets.Add(itime);
                                }
                            }
                        }
                        else
                        {
                            // accumulated hours exceeds 3 hrs >> all good do nothing
                            Console.WriteLine($"Accumulated hours {acumHour} Hrs. exceeds 3 hrs minimum limit, no transformation needed");
                        }
                }

                Console.WriteLine($"Total Added time sheet count: {addTimeSheets.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }


            Console.ReadKey();
        }
    }
}
