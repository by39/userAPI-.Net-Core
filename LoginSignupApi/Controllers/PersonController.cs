using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Http;
using System.Collections;
using LoginSignupApi.Models;

namespace LoginSignupApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Person")]
    public class PersonController : Controller
    {

        MySqlConnection conn;

        // GET: api/Person
        [HttpGet]
        public ArrayList Get()
        {
            ConnectMysql();

            ArrayList peopleArray = new ArrayList();

            string queryString = "SELECT * FROM users";

            MySqlCommand cmd = new MySqlCommand(queryString, conn);

            MySqlDataReader myReader = cmd.ExecuteReader();

            while (myReader.Read())
            {
                Person p = new Person();
                p.id = (int)myReader["id"];
                p.name = myReader["uname"].ToString();
                p.password = myReader["pword"].ToString();
                p.email = myReader["email"].ToString();

                peopleArray.Add(p);
            }
            
            return peopleArray;
        }

        // GET: api/Person/5
        [HttpGet("{id}", Name = "Get")]
        public Person Get(int id)
        {
            ConnectMysql();

            Person p = new Person();
            string queryString = "SELECT * FROM users WHERE id = " + id;

            MySqlCommand cmd = new MySqlCommand(queryString, conn);

            MySqlDataReader myReader = cmd.ExecuteReader();

            if (myReader.Read())
            {
                p.id = (int)myReader["id"];
                p.name = myReader["uname"].ToString();
                p.password = myReader["pword"].ToString();
                p.email = myReader["email"].ToString();

                return p;
            }
            else
            {
                return null;
            }
        }
        
        // POST: api/Person
        [HttpPost]
        public string Post([FromBody]Person person)
        {
            string postReult;
            ArrayList unameList = new ArrayList();
            ConnectMysql();
            string checkString = "SELECT uname FROM users";

            MySqlCommand checkCmd = new MySqlCommand(checkString, conn);

            MySqlDataReader myReader = checkCmd.ExecuteReader();

            while (myReader.Read())
            {
               
                string pname = myReader["uname"].ToString();

                unameList.Add(pname);
            }
            myReader.Close();

            if (!unameList.Contains(person.name))
            {
                string queryString = "INSERT INTO `users` (`uname`, `pword`, `email`) VALUES ('" + person.name + "', '" + person.password + "', '" + person.email + "');";
                MySqlCommand cmd = new MySqlCommand(queryString, conn);
                cmd.ExecuteNonQuery();
                postReult = "Added!";
            }
            else
            {
                postReult = "The user is already exsit!";
            }
            return postReult;
        }
        
        // PUT: api/Person/5
        [HttpPut("{id}")]
        public string Put(long id, [FromBody]Person p)
        {
            ConnectMysql();

            string queryString = "SELECT * FROM users WHERE id = " + id;

            MySqlCommand cmd = new MySqlCommand(queryString, conn);

            MySqlDataReader myReader = cmd.ExecuteReader();

            if (myReader.Read())
            {
                myReader.Close();
                queryString = "UPDATE `users` SET  `uname` = '" + p.name + "', `pword` = '" + p.password + "', `email` = '" + p.email + "' WHERE `users`.`id` = " + id;

                cmd = new MySqlCommand(queryString, conn);

                cmd.ExecuteNonQuery();

                return "Changed!";
            }
            else
            {
                return "Not found";
            }
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public string Delete(long id)
        {
            ConnectMysql();
            Person p = new Person();
            string queryString = "SELECT * FROM users WHERE id = " + id;

            MySqlCommand cmd = new MySqlCommand(queryString, conn);

            MySqlDataReader myReader = cmd.ExecuteReader();
            myReader.Read();
            if (myReader.Read())
            {
                myReader.Close();
                queryString = "DELETE FROM users WHERE id = " + id;

                cmd = new MySqlCommand(queryString, conn);

                cmd.ExecuteNonQuery();
                return "Deleted";
            }
            else
            {
                return "Not found";
            }
        }

        public void ConnectMysql()
        {
            string myConnectionstring = "Server=127.0.0.1;Port=3306;database=testing;User Id=root;Password=;charset=utf8;SslMode=none";
            try
            {
                conn = new MySqlConnection(myConnectionstring);
                conn.Open();
            }
            catch (MySqlException ex)
            {

            }
        }
    }
}
