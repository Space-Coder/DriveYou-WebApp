using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectDriver_Client.Context;
using ProjectDriver_Client.Controllers;
using ProjectDriver_Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectDriver_Client.Pages
{
    [AllowAnonymous]
    [RequireHttps]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public ApplicationDbContext db;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }

        public void OnGet()
        {

        }
       

    }
}
