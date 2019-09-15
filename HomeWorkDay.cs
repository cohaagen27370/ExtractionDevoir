using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace DevoirFouqueville
{


    public class HomeWorkFactory
    {

        private static List<string> Days = new List<string> { "dimanche", "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi" };
        private static List<string> Months = new List<string> { "janvier", "février", "fevrier", "mars", "avril", "mai", "juin", "juillet", "aout", "août", "septembre", "octobre", "novembre", "décembre", "decembre" };

        public static List<HomeWorkDay> GetAllDays(HtmlDocument document)
        {

            List<HomeWorkDay> homeWorkDays = new List<HomeWorkDay>();

            var root = document.DocumentNode.Descendants("div").Where(x => x.HasAttributes &&
            x.Attributes.Contains("class") &&
            x.Attributes["class"].Value == " col_content").FirstOrDefault();

            if (root != null)
            {
                var lines = root.Descendants("p").ToList();
                HomeWorkDay homeWork = null;

                foreach(var x in lines)
                {
                    var content = x.InnerHtml.ClearAndDecode();

                    if (string.IsNullOrWhiteSpace(content)) {
                        continue;
                    }

                    if (content.StartsWith("pour"))
                    {
                        if (homeWork != null)
                        {
                            homeWorkDays.Add(homeWork);
                        }

                        homeWork = new HomeWorkDay(content);
                    }
                    else if (homeWork != null)
                    {
                        if (content.Contains(":"))
                        {
                            homeWork.Tasks.Add(new Mater(content.Split(":")));
                        }
                        else
                        {
                            homeWork.Tasks.Add(new Mater(content));
                        }

                    }
                }

                homeWorkDays.Add(homeWork);
            }

            return homeWorkDays;
        }


    }




    public class HomeWorkDay
    {

        public HomeWorkDay(string content)
        {
            this.Tasks = new List<Mater>();

            var regexp = new Regex("[a-z|A-Z]{5,10}|[0-9]{1,2}|([a-z|A-Z|é|û]{3,9}):");

            var day = regexp.Matches(content)[0].Value;
            var date = regexp.Matches(content)[1].Value;
            this.Month = regexp.Matches(content)[2].Value;

            this.WorkDate = DateTime.Parse($"{date}/{this.Month}/{DateTime.Now.Year}");
            this.Date = $"{day.ToTitleCase()} {date} {this.Month.ToTitleCase()}";            
        }

        public void SetByMail() {

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress("cohaagen76320@gmail.com","Ecole de Fouqueville");
            message.To.Add(new MailAddress("laurentblanquet.harengere@gmail.com"));
            message.To.Add(new MailAddress("littletchoum76@gmail.com"));
            message.Subject = $"Devoirs de Pauline du {this.Date} {DateTime.Now.Year}" ;
            message.Body = this.GetBody();

            using (var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential("cohaagen76320@gmail.com", "2776FuMi@eR$!");

                // Enable SSL encryption
                client.EnableSsl = true;

                // Try to send the message. Show status in console.
                try
                {
                    Console.WriteLine("Attempting to send email...");
                    client.Send(message);
                    Console.WriteLine("Email sent!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }
        }

        private string GetBody() {

            StringBuilder body = new StringBuilder("<br/><br/><table style='width:700px;'><tr style='font-weight:bold;background-color:lightblue;'><th style='width:100px'>Matière</th><th>Tâche(s)</th></tr>");

            this.Tasks.ForEach((t) => {
                body.Append($"<tr><td><b>{t.Name}</b></td><td>{t.ToDoTask}</td></tr>");
            });

            body.Append("</table>");

            body.Append("<br/><br/><br/>");
            
            body.Append("<table style='width:700px;'><tr>");
            body.Append($"<td><b><a href='http://ecoleprimairefouqueville.e-monsite.com/pages/dictees-preparees-et-listes-de-mots/dictees-preparees-et-listes-de-mots-cm2.html'>Dictées CM2</a></b></td>");
            body.Append($"<td><b><a href='http://ecoleprimairefouqueville.e-monsite.com/pages/poesie/poesie-cm2.html'>Poésies CM2</a></b></td>");
            body.Append($"<td><b><a href='http://ecoleprimairefouqueville.e-monsite.com/pages/cahier-journal/cahier-journal-{this.Month.ToLower()}-{DateTime.Now.Year}.html'>Cahier journal {this.Month} {DateTime.Now.Year}</a></b></td>");
            body.Append("</tr></table>");

            return body.ToString();         
        }

        public string Date { get; set; }

        public string Month { get; set; }

        public DateTime WorkDate { get; set; }

        public List<Mater> Tasks { get; set; }

    }


    public class Mater
    {

        public Mater(string[] tab)
        {
            tab = tab.FormatTask();
            this.Name = tab.First().ToTitleCase();
            this.ToDoTask = tab.Last().ToTitleCase();
        }

        public Mater(string name, string todo)
        {
            this.Name = name;
            this.ToDoTask = todo;
        }

        public Mater(string todo)
        {
            this.ToDoTask = todo;
        }

        public string Name { get; set; }

        public string ToDoTask { get; set; }

    }

}