﻿using System.Text.Json;
using System.Text;
using messageSwitch_UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace messageSwitch_UI.Controllers
{
    public class QueryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public QueryController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = configuration["MessageSwitchApi:Url"] ?? throw new ArgumentNullException("API URL not configured");
        }
        public IActionResult Index()
        {
            return View(new DriverLicenseQueryModel());
        }

        [HttpPost]
        public async Task<IActionResult> Submit(DriverLicenseQueryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Custom validation: either OLN or (NAM, DOB, SEX)
            if (string.IsNullOrEmpty(model.OLN) && (string.IsNullOrEmpty(model.NAM) || string.IsNullOrEmpty(model.DOB) || string.IsNullOrEmpty(model.SEX)))
            {
                ModelState.AddModelError("", "Either OLN or all of NAM, DOB, and SEX must be provided.");
                return View("Index", model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<object>(responseContent);
                    return View("Response", responseData);
                }
                else
                {
                    ModelState.AddModelError("", $"API error: {response.StatusCode}");
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error contacting API: {ex.Message}");
                return View("Index", model);
            }
        }

        [HttpGet]
        public IActionResult QueryVehicle()
        {
            return View(new QueryVehicleModel());
        }

        [HttpPost]
        public IActionResult SubmitQuery(QueryVehicleModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the query (e.g., send to database or API)
                TempData["Message"] = "Query submitted successfully!";
                return RedirectToAction("QueryVehicle");
            }

            return View("QueryVehicle", model);
        }
        [HttpPost]
        public IActionResult QueryAjax(QueryVehicleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input.");

            // Replace with your actual lookup logic
            string response = $"ORI: {model.ORI}\nNIC: {model.NIC}\nLIC: {model.LIC} - {model.LIS}\n" +
                              $"VIN: {model.VIN}\nVMA: {model.VMA}\n" +
                              $"Image Indicator: {(model.IND ? "Yes" : "No")}, RSH: {(model.RSH ? "Yes" : "No")}";

            return Content(response);
        }
    }
}
