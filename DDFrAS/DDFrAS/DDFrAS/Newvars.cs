using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDFrAS.DDFrAS
{
    public class Newvdbinputs
    {
        public static void Newvars(string vlan, string manip, string username, string passwordssh, string terminal)
        {
            using (DDFrASEntities context = new DDFrASEntities())
            {
                CONFIG_VAR configvar = new CONFIG_VAR
                {
                    Vlan = vlan,
                    Man_IP = manip,
                    Username = username,
                    PasswordSSH = passwordssh,
                    Terminal = terminal
                };
                context.CONFIG_VAR.Add(configvar);
                context.SaveChanges();
            }
        }

        public static void NewScript(string script, DateTime executedate, int id, int status)
        {
            using (DDFrASEntities context = new DDFrASEntities())
            {
                CONFIG configscript = new CONFIG
                {
                    Command_ID = id,
                    Script = script,
                    ExDate = executedate,
                    Status = 0
                };
                context.CONFIGs.Add(configscript);
                context.SaveChanges();

            }

        }
            
        
    }
}