using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using aspnet_login.Models;
using System;

namespace aspnet_login.Controllers
{
    public class LoginController : Controller
    {
        private string usersFile = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "users.json");
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoginController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        // Get the list of users from the JSON file
        private List<User> GetUsers()
        {
            if (System.IO.File.Exists(usersFile))
            {
                var json = System.IO.File.ReadAllText(usersFile);
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            return new List<User>();
        }

        // Save the list of users into the JSON file
        private void SaveUsers(List<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            System.IO.File.WriteAllText(usersFile, json);
        }

        // Show the login/signup page
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("LoggedIn") == "true")
            {
                return RedirectToAction("LoginSuccess");
            }
            return View();
        }

        public IActionResult LoginSuccess()
        {
            if (HttpContext.Session.GetString("LoggedIn") != "true")
            {
                string loginOrigin = HttpContext.Session.GetString("LoginOrigin") ?? "Index";
                return RedirectToAction(loginOrigin);
            }
            
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            string loginOrigin = HttpContext.Session.GetString("LoginOrigin") ?? "Index";
            HttpContext.Session.Remove("LoggedIn");
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Clear();

            return RedirectToAction(loginOrigin);
        }

        // Handle login process
        [HttpPost]
        public IActionResult Index(string action, string email, string username, string password)
        {
            Console.WriteLine($"HttpPost Index action: {action}, email: {email}, username: {username}, password: {password}");
            var users = GetUsers();
            // Get the path of the popup.html file
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "popup.html");
             // Read the content of the popup.html file
            string popupHtml = System.IO.File.ReadAllText(filePath);
            // Pass the popupHtml content to the view
            ViewData["PopupHtml"] = popupHtml;

            if (action == "login")
            {
                var user = users.Find(u => u.Email == email && u.Password == password);
                if (user != null)
                {
                    HttpContext.Session.SetString("LoggedIn", "true");
                    // Save email to session
                    HttpContext.Session.SetString("Email", email);

                    HttpContext.Session.SetString("LoginOrigin", "Index");

                    Console.WriteLine("Login successful!");
                    return RedirectToAction("LoginSuccess");
                }
                ViewData["PopupMessage"] = "Username or password is incorrect.";
                Console.WriteLine("Username or password is incorrect.");
            }
            else if (action == "signup")
            {
                // Check if the email already exists
                if (users.Exists(u => u.Email == email))
                {
                    ViewData["PopupMessage"] = "Email already exists.";
                    Console.WriteLine("Email already exists.");
                    return View();
                }

                // Add the new user to the list
                var newUser = new User { Email = email, Username = username, Password = password };
                users.Add(newUser);

                // Save the updated list to the JSON file
                SaveUsers(users);

                ViewData["PopupMessage"] = "Sign up successful!";
                Console.WriteLine("Sign up successful!");
                return View();
            }

            Console.WriteLine("No action is valid");

            return View();
        }

        // Action for login2
        [HttpGet]
        public IActionResult Login2()
        {
            if (HttpContext.Session.GetString("LoggedIn") == "true")
            {
                return RedirectToAction("LoginSuccess");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login2(string action, string email, string username, string password)
        {
            Console.WriteLine($"HttpPost Login2 action: {action}, email: {email}, username: {username}, password: {password}");
            var users = GetUsers();
            // Get the path of the popup.html file
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "popup.html");
             // Read the content of the popup.html file
            string popupHtml = System.IO.File.ReadAllText(filePath);
            // Pass the popupHtml content to the view
            ViewData["PopupHtml"] = popupHtml;
            
            if (action == "login")
            {
                var user = users.Find(u => u.Email == email && u.Password == password);
                if (user != null)
                {
                    HttpContext.Session.SetString("LoggedIn", "true");
                    // Save email to session
                    HttpContext.Session.SetString("Email", email);

                    HttpContext.Session.SetString("LoginOrigin", "Login2");

                    Console.WriteLine("Login successful!");
                    return RedirectToAction("LoginSuccess");
                }
                // ViewData["PopupMessage"] = "Username or password is incorrect.";
                Console.WriteLine("Username or password is incorrect.");
            }
            else if (action == "signup")
            {
                // Check if the email already exists
                if (users.Exists(u => u.Email == email))
                {
                    // ViewData["PopupMessage"] = "Email already exists.";
                    Console.WriteLine("Email already exists.");
                    return View();
                }

                // Add the new user to the list
                var newUser = new User { Email = email, Username = username, Password = password };
                users.Add(newUser);

                // Save the updated list to the JSON file
                SaveUsers(users);

                // ViewData["PopupMessage"] = "Sign up successful!";
                Console.WriteLine("Sign up successful!");
                return View();
            }

            return View();
        }
    }
}
