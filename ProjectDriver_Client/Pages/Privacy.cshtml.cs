using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Pages
{
    [AllowAnonymous]
    [RequireHttps]
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }
    }
}
