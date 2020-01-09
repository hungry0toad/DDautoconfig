using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using Renci.SshNet;

namespace DDFrAS
{
    public static class ASInput
    {
        //insert new script into database
        public static void NewScript(string script, DateTime executedate, int id, int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                CONFIG configscript = new CONFIG
                {
                    Command_ID = id,
                    Script = script,
                    ExDate = executedate,
                    Status = 0,
                    Switch_ID = sw_id
                };
                context.CONFIGs.Add(configscript);
                context.SaveChanges();

            }

        }

        //insert new switch into database
        public static void NewSwitch(string switch_name, string man_ip, string ssh_username, string ssh_password, string ena_password)
        {
            using (var context = new DDFrASEntities())
            {
                SWITCH newswitch = new SWITCH
                {
                    SSH_Password = ssh_password,
                    SSH_Username = ssh_username,
                    Term_Password = ena_password,
                    Man_IP = man_ip,
                    Switch_Name = switch_name
                };
                context.SWITCHes.Add(newswitch);
                context.SaveChanges();

            }

        }
        public static void EditSwitch(int sw_id, string switch_name, string man_ip, string ssh_username, string ssh_password, string ena_password)
        {
            using (var context = new DDFrASEntities())
            {
                var editswitch = context.SWITCHes.SingleOrDefault(s => s.Switch_ID == sw_id);
                if (editswitch != null)
                {
                    editswitch.SSH_Password = ssh_password;
                    editswitch.SSH_Username = ssh_username;
                    editswitch.Term_Password = ena_password;
                    editswitch.Man_IP = man_ip;
                    editswitch.Switch_Name = switch_name;
                    context.SaveChanges();
                }
            }
        }
        public static void NewOutput(string output, int config_id, int status)
        {
            using (var context = new DDFrASEntities())
            {
                if (output != null)
                {
                    var addoutput = context.CONFIGs.SingleOrDefault(c => c.Config_ID == config_id);
                    addoutput.Switch_Output = output;
                    addoutput.Status = status;
                    context.SaveChanges();
                }
            }
        }


    }
    public static class ASselect
    {
        public static string Getstatus(int status)
        {
            string line;
            if (status == 0)
            {
                line = "Nog niet uitgevoerd";
                return line;
            }
            if (status == 1)
            {
                line = "succesvol uitgevoerd";
                return line;
            }
            if (status == 2)
            {
                line = "Gedeeltelijk mislukt";
                return line;
            }
            if (status == 3)
            {
                line = "Mislukt";
                return line;
            }
            else
            {
                line = "Onbekend";
                return line;
            }
        }
        public static List<CONFIG> Getfailedscripts()
        {
            using (var context = new DDFrASEntities())
            {
                return context.CONFIGs.Where(s => s.Status == 3).ToList();
            }
        }
        public static List<SWITCH> AllSwitches()
        {
            using (var context = new DDFrASEntities())
            {
                return context.SWITCHes.ToList();
            }
        }
        public static string Switchname(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.Switch_Name).SingleOrDefault();
            }
        }
        public static string Switchsshuser(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.SSH_Username).SingleOrDefault();
            }
        }
        public static string Switchsshpass(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.SSH_Password).SingleOrDefault();
            }
        }
        public static string Switchmanip(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.Man_IP).SingleOrDefault();
            }
        }
        public static string Switchtermpass(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.Term_Password).SingleOrDefault();
            }
        }
        public static string Script(int config_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from c in context.CONFIGs where c.Config_ID.Equals(config_id) select c.Script).SingleOrDefault();
            }

        }
    }
    public static class ASsshconnection
    {
        public static void SetupConnection(int switch_id, int configid)
        {
            using (var context = new DDFrASEntities())
            {
                string ip = ASselect.Switchmanip(switch_id);
                string username = ASselect.Switchsshuser(switch_id);
                string password = ASselect.Switchsshpass(switch_id);
                string enablepass = ASselect.Switchtermpass(switch_id);
                string newline = System.Environment.NewLine;

                ConnectionInfo Conninfo = new ConnectionInfo(ip, 22, username, new AuthenticationMethod[]
                {

                    new PasswordAuthenticationMethod(username, password)
                });

                using (var sshclient = new SshClient(Conninfo))
                {
                    sshclient.Connect();
                    string script = "enable" + newline + ASselect.Switchtermpass(switch_id) + newline + ASselect.Script(configid);
                    using (var cmd = sshclient.CreateCommand(script))
                    {
                        cmd.Execute();
                        int status = ASsshconnection.Scanoutput(cmd.Result);
                        ASInput.NewOutput(cmd.Result, configid, status);
                    }
                }
            }

        }
        //scan output for undesirable output/words
        public static int Scanoutput(string output)
        {
            if(output == null)
            {
                return 3;
            }else if (output.Contains("Invalid"))
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}