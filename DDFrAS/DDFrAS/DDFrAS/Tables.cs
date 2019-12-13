using System;
public class POP
{
    //PK
    public int Pop_ID { get; set; }

    //Other column
    public string Pop_Name { get; set; }
}

public class SWITCH
{
    //PK
    public int Switch_ID { get; set; }

    //Other column
    public string Switch_Name { get; set; }

    //FK
    public int POP_ID { get; set; }
    public POP POP { get; set; }

    //FK
    //public int ;
}

public class PORT
{
    //PK
    public int Port_ID { get; set; }

    //Other column
    public string PortName { get; set; }

    //FK
    public int Switch_ID { get; set; }
    public SWITCH SWITCH { get; set; }

}

public class SSH_COMMANDS
{
    //PK
    public int Command_ID { get; set; }

    //Other column
    public string Command_Name { get; set; }
    public string Command_Temp { get; set; }
}

public class PRODUCT
{
    //PK
    public int Product_ID { get; set; }

    //Other column
    public string Product_Name { get; set; }
}

public class CONFIG
{
    //PK
    public int Config_ID { get; set; }

    //Other column
    public int Line_ID { get; set; }
    public int Status { get; set; }
    public DateTime ExDate { get; set; }
    public string Script { get; set; }

    //FK
    public int Port_ID { get; set; }
    public PORT PORT { get; set; }
    public int Product_ID { get; set; }
    public PRODUCT PRODUCT { get; set; }
    public int Command_ID { get; set; }
    public SSH_COMMANDS SSH_COMMAND { get; set; }

}

public class CONFIG_VAR
{
    //PK
    public int Conf_var_ID;

    //Other column
    public int Vlan;
    public string Man_IP;
    public string Usernamessh;
    public string Passwordssh;
    public string Term_pass;

    //FK
    public int Config_ID;
    public CONFIG CONFIG;
}