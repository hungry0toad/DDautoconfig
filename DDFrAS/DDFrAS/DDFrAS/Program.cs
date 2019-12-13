using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDFrAS
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (DDFrASEntities1 context = new DDFrASEntities1()) 
            {
                CONFIG config = new CONFIG
                {
                    Config_ID = 1
                };
                context.CONFIGs.Add(config);
                context.SaveChanges();
                //List<string> strings = new List<string>();
                //strings.Add("1");
                //context.CONFIGs
            } 
        }
    }
}