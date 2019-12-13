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
            using (var db = new DDFrASEntities())
            {
                var query = from b in db.CONFIGs orderby b.Line_ID select b;


            }
        }
    }
}