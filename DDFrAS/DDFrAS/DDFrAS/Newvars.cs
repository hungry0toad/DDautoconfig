using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace DDFrAS
{
    public static class Newvdbinputs
    {
        //insert new variables into database

        public static void Newvars(string vlan, string manip, string username, string passwordssh, string terminal)
        {
            using (DDFrASEntities context = new DDFrASEntities())
            {
                CONFIG_VAR configvar = new CONFIG_VAR
                {
                    //Vlan = vlan,
                    //Man_IP = manip,
                    //Username = username,
                    //PasswordSSH = passwordssh,
                    //Terminal = terminal
                };
                context.CONFIG_VAR.Add(configvar);
                context.SaveChanges();
            }
        }

        //insert new script into database

        public static void NewScript(string script, DateTime executedate, int id, int sw_id)
        {
            using (DDFrASEntities context = new DDFrASEntities())
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
            using (DDFrASEntities context = new DDFrASEntities())
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
            using (DDFrASEntities context = new DDFrASEntities())
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

        //public StringBuilder Resultssh(StringBuilder result)
        //{
        //    return result;
        //}

        //public static void ssh_connection(string ip, string username, string userpassword, string enablepass, string command)
        //{
        //    StringBuilder result = new StringBuilder();
        //    var client = new SshClient(ip, username, userpassword);
        //    client.Connect();
        //    result.Append(client.RunCommand(command).Execute() + "\r\n");


        //}

    }
    public static class ASselectswitch
    {
        public static List<SWITCH> Getallswitchess()
        {
            using(var context = new DDFrASEntities())
            {
                return context.SWITCHes.ToList();
            }
        }
        public static string name(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.Switch_Name).SingleOrDefault();
            }
        }
        public static string sshuser(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.SSH_Username).SingleOrDefault();
            }
        }
        public static string sshpass(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.SSH_Password).SingleOrDefault();
            }
        }
        public static string manip(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.Man_IP).SingleOrDefault();
            }
        }
        public static string termpass(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                return (from s in context.SWITCHes where s.Switch_ID.Equals(sw_id) select s.Term_Password).SingleOrDefault();
            }
        }

    }
    public static class ASselectstatus
    {
        public static List<CONFIG> Getfailedscripts()
        {
            using(var context = new DDFrASEntities())
            {
                return context.CONFIGs.Where(s => s.Status == 3).ToList();
            }
        }
        public static string Getstatusstring(int status)
        {
            string line = "";
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
    }


}
