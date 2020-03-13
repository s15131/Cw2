using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Cw2
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = @"C:\Users\piotr\Desktop\z2\log.txt";
            StreamWriter zapis = new StreamWriter(log);
            try
            {
                string path, wynik, typ;

                if (args.Length == 3)
                {
                    path = args[0];
                    wynik = args[1];
                    typ = args[2].ToUpper();
                }
                else if (args.Length == 2)
                {
                    path = args[0];
                    wynik = args[1];
                    typ = "XML";
                }
                else if (args.Length == 2)
                {
                    path = args[0];
                    wynik = @"C:\Users\piotr\Desktop\z2\result.xml";
                    typ = "XML";
                }
                else
                {
                    path = @"C:\Users\piotr\Desktop\z2\dane.csv";
                    wynik = @"C:\Users\piotr\Desktop\z2\result.xml";
                    typ = "XML";
                }

                var today = DateTime.Today;
                //var wynik_json = @"C:\Users\piotr\Desktop\z2\result.json";
                var lines = File.ReadLines(path);
                Dictionary<string, int> wystapienia = new Dictionary<string, int>();
                Regex rx = new Regex("[0-9]");
                bool spr = false;
                HashSet<Student> hash = new HashSet<Student>(new OwnComparer());


                foreach (var line in lines)
                {
                    string[] student = line.Split(',');
                    spr = false;
                    for (int i = 0; i < student.Length; i++)
                    {
                        if (student[i].Equals(""))
                        {
                            spr = true;
                            zapis.WriteLine("Student " + student[0] + " " + rx.Replace(student[1], "") + " [s" + student[4] + "] posiada puste kolumny !");
                            break;
                        }
                    }
                    if (student.Length == 9 && spr == false)
                    {

                        hash.Add(new Student
                        {
                            imie = student[0],
                            nazwisko = rx.Replace(student[1], ""),
                            kierunek = student[2],
                            tryb = student[3],
                            numerindexu = "s" + student[4],
                            email = "s" + student[4] + rx.Replace(student[6], ""),
                            dataurodzenia = student[5],
                            imiematki = student[7],
                            imieojca = student[8]


                        });

                    }
                    else if (student.Length != 9)
                    {
                        zapis.WriteLine("Student " + student[0] + " " + rx.Replace(student[1], "") + " [s" + student[4] + "] za mało kolumn !");

                    }
                }

                foreach (var s in hash)
                {
                    if (wystapienia.ContainsKey(s.kierunek))
                    {

                        wystapienia[s.kierunek]++;
                    }
                    else
                    {
                        wystapienia.Add(s.kierunek, 1);
                    }
                }

                if (typ == "XML")
                {
                    XDocument xml = new XDocument(new XElement("uczelnia",
                        new XAttribute("createdAt", today.ToShortDateString()),
                        new XAttribute("author", "Piotr Pasiak"),
                        new XElement("studenci",
                                    from student in hash
                                    select new XElement("student",
                                    new XAttribute("indexNumber", student.numerindexu),
                                    new XElement("fname", student.imie),
                                    new XElement("lname", student.nazwisko),
                                    new XElement("birthdate", student.dataurodzenia),
                                    new XElement("email", student.email),
                                    new XElement("mothersName", student.imiematki),
                                    new XElement("fathersName", student.imieojca),
                                    new XElement("studies",
                                        new XElement("name", student.kierunek),
                                        new XElement("mode", student.tryb)
                               ))),
                                    new XElement("activeStudies",
                                            from kierunki in wystapienia
                                            select new XElement("studies",
                                            new XAttribute("numberOfStudents", kierunki.Value),
                                            new XAttribute("name", kierunki.Key)

                        ))));
                    xml.Save(wynik);
                }
                else if (typ == "JSON")
                {

                    JObject json = new JObject(new JProperty("uczelnia", new JObject(
                       new JProperty("createdAt", today.ToShortDateString()),
                       new JProperty("author", "Piotr Pasiak"),
                       new JProperty("studenci", new JArray(
                                    from student in hash
                                    select new JObject(
                                    new JProperty("indexNumber", student.numerindexu),
                                    new JProperty("fname", student.imie),
                                    new JProperty("lname", student.nazwisko),
                                    new JProperty("birthdate", student.dataurodzenia),
                                    new JProperty("email", student.email),
                                    new JProperty("mothersName", student.imiematki),
                                    new JProperty("fathersName", student.imieojca),
                                    new JProperty("studies", new JObject(
                                        new JProperty("name", student.kierunek),
                                        new JProperty("mode", student.tryb)))))),
                                    new JProperty("activeStudies", new JArray(
                                        from kierunki in wystapienia
                                        select new JObject(
                                            new JProperty("name", kierunki.Key),
                                            new JProperty("numberOfStudents", kierunki.Value)

                        ))))));

                    File.WriteAllText(wynik, JsonConvert.SerializeObject(json));
                }
                else
                {
                    zapis.WriteLine("Niepoprawny typ danych !");
                }

            }
            catch (ArgumentException)
            {
                Console.WriteLine("Podana ścieżka jest niepoprawna");
                zapis.WriteLine("Podana ścieżka jest niepoprawna");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Plik nazwa nie istnieje");
                zapis.WriteLine("Plik nazwa nie istnieje");
            }

            zapis.Close();


        }
    }
}

