using System;
using System.Linq;
using HtmlAgilityPack;

namespace DevoirFouqueville
{
    class Program
    {

        private string _homeWorkUrl = "http://ecoleprimairefouqueville.e-monsite.com/pages/devoirs/devoirs-1.html";

        public Program()
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(this._homeWorkUrl);

            var allDays = HomeWorkFactory.GetAllDays(htmlDoc);

            var next = allDays.Where(x => x.WorkDate.Date > DateTime.Now.Date).FirstOrDefault();

            next.SetByMail();
        }


        static void Main(string[] args)
        {
            new Program();
        }
    }
}
