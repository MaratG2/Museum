using System.Collections;
using System.Collections.Generic;
using Npgsql;
using UnityEngine;

public class ConnectionDb : MonoBehaviour
{
    // Start is called before the first frame update
    public NpgsqlConnection Connection;
    void Start()
    {
        string connectionString =
            "Port = 5432;"+
            "Server=localhost;" +
            "Database=museumistu;" +
            "User ID=postgres;" +
            "Password=postgres;";
        Connection = new NpgsqlConnection(connectionString);
        Connection.Open();
        //Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
