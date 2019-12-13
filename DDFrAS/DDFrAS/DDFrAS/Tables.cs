using System;
public class POP
{
    public int Pop_ID {get; set;}
    public string Pop_Name { get; set; }

}

public class SWTICH
{
    public int Switch_ID { get; set; }
    public int POP_ID { get; set; }
    public string Switch_Name { get; set; }
}
//public class PORT
//{
//    public int Port_ID { get; set; }
//    public int Switch_ID { get; set; }
//    public string PortName { get; set; }
//}

//public class SSH_COMMANDS
//{
//    public int Command_ID { get; set; }
//    public string Command_Name { get; set; }
//    public string Command_Temp { get; set; }
//}

//public class PRODUCT
//{
//    public int Product_ID { get; set; }
//    public int Pop_ID { get; set; }
//    public string Product_Name { get; set; }
//}

//public class CONFIG
//{
//    public int Config_ID { get; set; }
//    public int Poort_ID { get; set; }
//    public int Product_ID { get; set; }
//    public int Command_ID { get; set; }
//    public int Line_ID { get; set; }
//    public int Status { get; set; }
//    public DateTime ExDate { get; set; }
//    public string Script { get; set; }
//}

//public class CONFIG_VAR
//{
//    public int Conf_var_ID;
//    public int Config_ID;
//    public int Vlan;
//    public string Man_IP;
//    public string Usernamessh;
//    public string PasswordSSH;
//    public string Password;
//    public string Terminal;
//}