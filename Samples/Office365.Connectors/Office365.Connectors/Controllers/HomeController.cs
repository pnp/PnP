using Newtonsoft.Json;
using Office365.Connectors.Components;
using Office365.Connectors.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Office365.Connectors.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewConnection()
        {
            return View();
        }

        /// <summary>
        /// This action is invoked by Office 365 to register a new connection
        /// </summary>
        /// <param name="state">Optional parameter to provide custom state information</param>
        /// <param name="webhook_url">The name of the group selected by the user</param>
        /// <param name="group_name">The webhook URL to use for communicating with the target Office 365 Group</param>
        /// <param name="error">The error code that is returned if the application doesn't return successfully</param>
        /// <returns></returns>
        public ActionResult Connect(String state, String webhook_url, String group_name, String error)
        {
            if (!String.IsNullOrEmpty(error))
            {
                // The user refused to connect the connector ... we need it ...
                return View("ConnectionError",
                    new ConnectionError
                    {
                        State = state,
                        Error = error,
                    });
            }

            var connection = new Connection
            {
                GroupName = group_name,
                State = state,
                WebHookUrl = webhook_url,
            };

            AddConnection(connection);

            return View(connection);
        }

        public ActionResult SendCard()
        {
            var model = new SendCard();
            model.Connections = GetConnections();

            var initialCard = new { Title = "Sample Card Title", Text = "Sample Card Text", };
            model.CardJson = JsonConvert.SerializeObject(initialCard);

            return View(model);
        }

        [HttpPost]
        public ActionResult SendCard(SendCard card)
        {
            if (ModelState.IsValid)
            {
                HttpHelper.MakePostRequest(
                    card.WebHookUrl,
                    card.CardJson,
                    "application/json");

                return View("CardSent");
            }
            else
            {
                card.Connections = GetConnections();
                return View(card);
            }
        }

        public ActionResult CardSent()
        {
            return View();
        }

        public ActionResult ConnectionError(ConnectionError model)
        {
            return View();
        }

        private List<Connection> GetConnections()
        {
            using (var sr = new StreamReader(
                HttpContext.Server.MapPath("~/App_Data/connections.json"),
                System.Text.Encoding.Unicode))
            {
                var connections = sr.ReadToEnd();
                var result = JsonConvert.DeserializeObject<List<Connection>>(connections);
                return (result ?? new List<Connection>());
            }
        }

        private void AddConnection(Connection connection)
        {
            var connections = GetConnections();
            connections.Add(connection);
            SaveConnections(connections);
        }

        private void RemoveConnection(Connection connection)
        {
            var connections = GetConnections();
            connections.Remove(connection);
            SaveConnections(connections);
        }

        private void SaveConnections(List<Connection> connections)
        {
            using (var sw = new StreamWriter(
                HttpContext.Server.MapPath("~/App_Data/connections.json"),
                false,
                System.Text.Encoding.Unicode))
            {
                var jsonConnections = JsonConvert.SerializeObject(connections);
                sw.Write(jsonConnections);
            }
        }
    }
}