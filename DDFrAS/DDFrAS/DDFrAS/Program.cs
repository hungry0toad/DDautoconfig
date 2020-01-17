using Hangfire;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Z.EntityFramework.Plus;
using Z.Expressions;

namespace DDFrAS
{
    public static class ASInput
    {
        //insert new script into database
        public static void NewScript(string script, DateTime executedate, int id, int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                //Create new config entity
                CONFIG configscript = new CONFIG
                {
                    Command_ID = id, //add foreign key from SSH_COMMANDS table (script template) to config entity
                    Script = script, //add the fillid in script to config entity 
                    ExDate = executedate, //add the execute date to config entity
                    Status = 0, //add initial status code 0 (not yet executed) to config entity
                    Switch_ID = sw_id //add the corresponding switch to config entity
                };
                context.CONFIGs.Add(configscript);
                context.SaveChanges();

                //check if script needs to be executed now (within 60 seconds)
                double executewhen = (executedate - DateTime.Now).TotalSeconds;
                if (executewhen < 60)
                {
                    Debug.WriteLine("now");
                    ASHangfire.AddTask(configscript.Config_ID);
                }
                else
                {
                    Debug.WriteLine("later");
                    ASHangfire.AddTask(configscript.Config_ID, executedate);
                }
            }
        }

        //Edit Config with new script and executedate
        public static void EditConfig(int configid, string script, DateTime executedate)
        {

            using (var context = new DDFrASEntities())
            {
                var editconfig = context.CONFIGs.SingleOrDefault(c => c.Config_ID == configid);
                if (editconfig != null)
                {
                    //editconfig.Script.Remove(configid);
                    editconfig.Script = script;
                    editconfig.ExDate = executedate;
                    context.SaveChanges();
                    if (executedate > DateTime.Now)
                    {
                        ASHangfire.AddTask(configid);
                    }
                }
            }
        }

        //Edit config - add hangfire id
        public static void EditConfig(int configid, string JobId)
        {

            using (var context = new DDFrASEntities())
            {
                var editconfig = context.CONFIGs.SingleOrDefault(c => c.Config_ID == configid);
                if (editconfig != null)
                {
                    editconfig.Hangfire_ID = JobId;
                    editconfig.Status = 5;
                    context.SaveChanges();
                }
            }
        }
        public static void DeleteConfig(int configid)
        {
            using (var context = new DDFrASEntities())
            {
                var delconfig = context.CONFIGs.SingleOrDefault(c => c.Config_ID == configid);
                if (delconfig != null)
                {
                    var jobid = context.CONFIGs.Where(c => c.Config_ID == configid).Select(j => j.Hangfire_ID).FirstOrDefault();
                    context.CONFIGs.Remove(context.CONFIGs.Where(c => c.Config_ID == configid).FirstOrDefault()); //delete config
                    context.SaveChanges();
                    try
                    {
                        BackgroundJob.Delete(jobid);
                    }
                    catch { Debug.WriteLine("Job not found (should not matter)"); }
                }
            }
        }

        //insert new switch into database
        public static void NewSwitch(string switch_name, string man_ip, string ssh_username, string ssh_password, string ena_password)
        {
            using (var context = new DDFrASEntities())
            {
                //Create new switch entity
                SWITCH newswitch = new SWITCH
                {
                    SSH_Password = ssh_password, //Add ssh password to switch entity
                    SSH_Username = ssh_username, //Add ssh username to switch entity
                    Term_Password = ena_password, //Add terminal password to switch entity
                    Man_IP = man_ip, //Add management/ssh connection ip to switch entity
                    Switch_Name = switch_name //Add name of switch to switch entity
                };
                context.SWITCHes.Add(newswitch); //Add new switch entity to FE
                context.SaveChanges(); //Savechanges to database

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
        public static void DeleteSwitch(int sw_id)
        {
            using (var context = new DDFrASEntities())
            {
                context.CONFIGs.Where(c => c.Switch_ID == sw_id).Delete();      //delete configs connected to switch (neccecary because of the foreign keys in the config table)
                context.SaveChanges();                                          //savechanges to db
                context.SWITCHes.Where(s => s.Switch_ID == sw_id).Delete();     //delete switch
                context.SaveChanges();                                          //savechanges to db
            }
        }
        public static void NewOutput(string output, int config_id)
        {
            using (var context = new DDFrASEntities())
            {
                int status = ASsshconnection.Scanoutput(output);
                string outputold;
                if (output != null)
                {
                    var addoutput = context.CONFIGs.SingleOrDefault(c => c.Config_ID == config_id);
                    try
                    {
                        outputold = addoutput.Switch_Output.ToString();
                    }
                    catch
                    {
                        outputold = System.Environment.NewLine + "Output: ";
                    }
                    addoutput.Switch_Output = outputold + output;
                    addoutput.Status = status;
                    context.SaveChanges();
                }
            }
        }
        public static void NewOutput(string output, int config_id, int status)
        {
            using (var context = new DDFrASEntities())
            {
                string outputold;
                if (output != null)
                {
                    var addoutput = context.CONFIGs.SingleOrDefault(c => c.Config_ID == config_id);
                    try
                    {
                        outputold = addoutput.Switch_Output.ToString();
                    }
                    catch
                    {
                        outputold = System.Environment.NewLine + "Output: ";
                    }
                    addoutput.Switch_Output = outputold + output;
                    addoutput.Status = status;
                    context.SaveChanges();
                }
            }
        }
    }
        public class ASHangfire
        {
            //add task to hangfire NOW and add generated hangfire id to config entity
            public static void AddTask(int Config_id)
            {
                using (var context = new DDFrASEntities())
                {
                    var configg = context.CONFIGs.SingleOrDefault(c => c.Config_ID == Config_id);
                    var JobId = BackgroundJob.Enqueue(() => ASsshconnection.SetupConnection(Config_id));
                    ASInput.EditConfig(Config_id, JobId);
                }
            }

            //add a task to hangfire on specific time and add generated hangfire id to config entity
            public static void AddTask(int Config_id, DateTime executetime)
            {
                using (var context = new DDFrASEntities())
                {
                    var configg = context.CONFIGs.SingleOrDefault(c => c.Config_ID == Config_id);
                    double fromseconds = (executetime - DateTime.Now).TotalSeconds;
                    if (fromseconds > 0)
                    {
                        var jobid = BackgroundJob.Schedule(() => ASsshconnection.SetupConnection(configg.Config_ID), TimeSpan.FromSeconds(fromseconds));
                        ASInput.EditConfig(configg.Config_ID, jobid);
                    }
                    else
                    { throw new InvalidOperationException("Verleden tijd in executetime"); }
                }
            }
        }
        public static class ASselect
        {
            public static List<CONFIG> Getconfignotexecuted()
            {
                using (var context = new DDFrASEntities())
                {
                    return context.CONFIGs.Where(c => c.Status == 0).ToList();
                }
            }

            //Translate status to human
            public static string Getstatus(int status)
            {
                if (status == 0)
                {
                    return "Nog niet uitgevoerd of gepland";
                }
                if (status == 1)
                {
                    return "Succesvol uitgevoerd";
                }
                if (status == 2)
                {
                    return "Gedeeltelijk mislukt";
                }
                if (status == 3)
                {
                    return "Mislukt";
                }
                if (status == 4)
                {
                    return "SSH timeout";
                }
                if (status == 5)
                {
                    return "Gepland";
                }
                else
                {
                    return "Onbekend";
                }
            }
            public static List<CONFIG> Getfailedscripts()
            {
                using (var context = new DDFrASEntities())
                {
                    return context.CONFIGs.Where(c => c.Status == 3).ToList();
                }
            }
            public static List<SWITCH> AllSwitches()
            {
                using (var context = new DDFrASEntities())
                {
                    return context.SWITCHes.ToList();
                }
            }
            public static SWITCH Switch(int sw_id)
            {
                using (var context = new DDFrASEntities())
                {
                    return context.SWITCHes.Where(s => s.Switch_ID == sw_id).FirstOrDefault();
                }
            }
            public static string Script(int config_id)
            {
                using (var context = new DDFrASEntities())
                {
                    return (from c in context.CONFIGs where c.Config_ID.Equals(config_id) select c.Script).SingleOrDefault();
                }
            }

            public static List<CONFIG> AllConfigs()
            {
                using (var context = new DDFrASEntities())
                {
                    return context.CONFIGs.ToList();
                }
            }
        }
        public static class ASsshconnection
        {
            [AutomaticRetry(Attempts = 5)]
            public static void SetupConnection(int configid)
            {
                using (var context = new DDFrASEntities())
                {
                    int switch_id = (context.CONFIGs.Where(c => c.Config_ID.Equals(configid)).Select(s => s.Switch_ID).SingleOrDefault());
                    var selswtich = ASselect.Switch(switch_id);
                    if (selswtich != null)
                    {
                        string newline = System.Environment.NewLine;

                        //setup connection info
                        ConnectionInfo Conninfo = new ConnectionInfo(selswtich.Man_IP, 22, selswtich.SSH_Username, new AuthenticationMethod[]
                        {
                            new PasswordAuthenticationMethod(selswtich.SSH_Username, selswtich.SSH_Password)
                        });

                        //create new sshclient with connectioninfo
                        using (var sshclient = new SshClient(Conninfo))
                        {

                            string script = ASselect.Script(configid);
                            string[] commands = script.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                            //var commands = script.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                            //try to execute script on switch else catch and call it timeout
                            try
                            {
                                sshclient.Connect();
                            }
                            catch
                            {
                                ASInput.NewOutput("SSH timeout", configid, 4);
                                throw new InvalidOperationException("SSH timeout, trying again later");
                            }
                            if (sshclient.IsConnected)
                            {
                                var stream = sshclient.CreateShellStream("Cisco Terminal", 80, 50, 1024, 1024, 1024);
                                stream.WriteLine("enable");
                                ASInput.NewOutput(stream.Read(), configid, 5);
                                stream.WriteLine(selswtich.Term_Password);
                                ASInput.NewOutput(stream.Read(), configid, 5);
                                Thread.Sleep(3000);
                                for (int i = 0; i < commands.Length; i++)
                                {
                                    stream.WriteLine(commands[i]);
                                    Thread.Sleep(100);
                                ASInput.NewOutput(stream.Read(), configid, 5);
                            }
                                sshclient.Disconnect();
                                ASInput.NewOutput(System.Environment.NewLine + "Session end", configid);
                            }
                        }
                    }
                }
            }


            //scan output for undesirable output/words
            public static int Scanoutput(string output)
            {
                if (output == null)
                {
                    return 3;
                }
                else if (output.Contains("Invalid"))
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