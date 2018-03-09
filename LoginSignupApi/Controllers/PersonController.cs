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
using System.Security.Cryptography;
using MimeKit;
using MailKit.Net.Smtp;

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
        [HttpGet("{uname}", Name = "Get")]
        public Person Get(string uname)
        {
            ConnectMysql();

            Person p = new Person();
            string queryString = "SELECT * FROM users WHERE uname = '" + uname + "'";

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
        public int Post([FromBody]Person person)
        {
            int postReult;
            ArrayList unameList = new ArrayList();
            ArrayList emailList = new ArrayList();
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

            checkString = "SELECT email FROM users";
            checkCmd = new MySqlCommand(checkString, conn);
            myReader = checkCmd.ExecuteReader();
            while (myReader.Read())
            {
                string pname = myReader["email"].ToString();
                emailList.Add(pname);
            }
            myReader.Close();

            if (!unameList.Contains(person.name))
            {
                if (!emailList.Contains(person.email))
                {
                    string queryString = "INSERT INTO `users` (`uname`, `pword`, `email`) VALUES ('" + person.name + "', '" + person.password + "', '" + person.email + "');";
                    MySqlCommand cmd = new MySqlCommand(queryString, conn);
                    cmd.ExecuteNonQuery();
                    postReult = 1;
                }
                else
                {
                    postReult = 2;
                }
                
            }
            else
            {
                postReult = 0;
            }
            return postReult;
        }

        //POST send code for resetting password
        [HttpPost("{email}")]
        public string POST(string email)
        {
            string password = CreateCode();
            //////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Test mail from", "boming.yu.nyriad@gmail.com"));
            message.To.Add(new MailboxAddress("Test mail to", email));

            message.Subject = "Test email asp.net";
            message.Body = new TextPart("plain")
            {
                Text = "The code is " + password
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("boming.yu.nyriad@gmail.com", "yubo1234");
                client.Send(message);
                client.Disconnect(true);
            }
            //////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////
            return password;
        }
        
        // PUT: api/Person/5
        [HttpPut("{uname}")]
        public int Put(string uname, [FromBody]Person p)
        {
            ConnectMysql();

            string queryString = "SELECT * FROM users WHERE uname = '" + uname + "'";

            MySqlCommand cmd = new MySqlCommand(queryString, conn);

            MySqlDataReader myReader = cmd.ExecuteReader();

            if (myReader.Read())
            {
                myReader.Close();
                queryString = "UPDATE `users` SET `pword` = '"+ p.password + "' WHERE `users`.`uname` = '" + uname + "'";

                cmd = new MySqlCommand(queryString, conn);

                cmd.ExecuteNonQuery();

                return 1;
            }
            else
            {
                return 0;
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

        public string CreateCode()
        {
            const string consonnants = "bcdfghjklmnpqrstvwxz";
            const string vowels = "aeiouy";

            string password = "";
            byte[] bytes = new byte[4];
            var rnd = new RNGCryptoServiceProvider();
            for (int i = 0; i < 3; i++)
            {
                rnd.GetNonZeroBytes(bytes);
                password += consonnants[bytes[0] * bytes[1] % consonnants.Length];
                password += vowels[bytes[2] * bytes[3] % vowels.Length];
            }

            rnd.GetBytes(bytes);
            password += (bytes[0] % 10).ToString() + (bytes[1] % 10).ToString();
            return password;
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
