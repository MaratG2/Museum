using System.Collections;
using System.Collections.Generic;
using System;
//using System.Data;   ## REMOVE THIS
using Npgsql;
using UnityEngine;

public class Test
{
    public static void postgres()
    {
        string connectionString =
            "Port = 5432;"+
            "Server=localhost;" +
            "Database=museumistu;" +
            "User ID=postgres;" +
            "Password=postgres;";
        // IDbConnection dbcon; ## CHANGE THIS TO
        NpgsqlConnection dbcon;

        dbcon = new NpgsqlConnection(connectionString);
        dbcon.Open();
        
        //IDbCommand dbcmd = dbcon.CreateCommand();## CHANGE THIS TO
        NpgsqlCommand dbcmd = dbcon.CreateCommand();
        // requires a table to be created named employee
        // with columns firstname and lastname
        // such as,
        //        CREATE TABLE employee (
        //           firstname varchar(32),
        //           lastname varchar(32));
        string sql =
            "SELECT c.title, c.image_desc " +
            "FROM public.contents AS c " +
            "JOIN public.options AS o ON c.onum = o.onum ";
            
        dbcmd.CommandText = sql;
        //IDataReader reader = dbcmd.ExecuteReader(); ## CHANGE THIS TO
        NpgsqlDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {

            //string FirstName = (string)reader.GetString(0); 
            string LastName = (reader.IsDBNull(1)) ? "NULL" : reader.GetString(1).ToString();
            string FirstName = (reader.IsDBNull(0)) ? "NULL" : reader.GetString(0).ToString();
            //string LastName = (string)reader.GetString(1);
            Debug.Log(FirstName + " | " + LastName);
            //Console.WriteLine();
        }
        // clean up
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }
}