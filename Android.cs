using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//References
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;

public class Android : MonoBehaviour
{
    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;
    public TMP_InputField t_username, t_firstname, t_lastname;
    public TMP_Text data_user;

    string DatabaseName = "USER.s3db";
    // Start is called before the first frame update
    void Start()
    {
        //Application database Path android
        string filepath = Application.persistentDataPath + "/" + DatabaseName;
        if (!File.Exists(filepath))
        {
            // If not found on android will create Tables and database

            Debug.LogWarning("File \"" + filepath + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/USER");



            // UNITY_ANDROID
            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/USER.s3db");
            while (!loadDB.isDone) { }
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDB.bytes);




        }

        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        string query;
        query = "CREATE TABLE User (username  varchar(20), firstname varchar(30), lastname varchar(30))";
        try
        {
            dbcmd = dbconn.CreateCommand(); // create empty command
            dbcmd.CommandText = query; // fill the command
            reader = dbcmd.ExecuteReader(); // execute command which returns a reader
        }
        catch (Exception e)
        {

            Debug.Log(e);

        }
        //  reader_function();
    }
    //Insert
    public void insert_button()
    {
        insert_function(t_username.text, t_firstname.text, t_lastname.text);

    }

    public void Update_button()
    {
        update_function(t_username.text, t_firstname.text, t_lastname.text);

    }


    //Insert To Database
    private void insert_function(string username, string firstname, string lastname)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("insert into User (username, firstname, lastname) values (\"{0}\",\"{1}\", \"{2}\")", username, firstname, lastname);// table name
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbconn.Close();
        }
        data_user.text = "";
        Debug.Log("Insert Done  ");

        reader_function();
    }
    //Read All Data For To Database
    private void reader_function()
    {
        // int idreaders ;
        string usernamereaders, firstnamereaders, lastnamereaders;
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT  username, firstname, lastname " + "FROM User";// table name
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                // idreaders = reader.GetString(1);
                usernamereaders = reader.GetString(0);
                firstnamereaders = reader.GetString(1);
                lastnamereaders = reader.GetString(2);

                data_user.text += usernamereaders + " - " + firstnamereaders + " - " + lastnamereaders + "\n";
                Debug.Log(" username =" + usernamereaders + "firstname=" + firstnamereaders + "lastname=" + lastnamereaders);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            //       dbconn = null;

        }
    }
    //Update on  Database 
    private void update_function(string update_username, string update_firstname, string update_lastname)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("UPDATE User set username = @username , firstname = @firstname,  lastname = @lastname");

            SqliteParameter P_update_username = new SqliteParameter("@username", update_username);
            SqliteParameter P_update_firstname = new SqliteParameter("@firstname", update_firstname);
            SqliteParameter P_update_lastname = new SqliteParameter("@lastname", update_lastname);

            dbcmd.Parameters.Add(P_update_username);
            dbcmd.Parameters.Add(P_update_firstname);
            dbcmd.Parameters.Add(P_update_lastname);

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbconn.Close();
        }

        // SceneManager.LoadScene("home");
    }


    // Update is called once per frame
    void Update()
    {

    }
}