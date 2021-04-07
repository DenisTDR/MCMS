using System.Collections.Generic;
using System.Linq;
using MCMS.Admin;
using MCMS.Base.Attributes;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers
{
    public class McmsFrameworkController : UiController
    {
        private FrameworkInfoService FrameworkInfoService =>
            ServiceProvider.GetRequiredService<FrameworkInfoService>();

        public IActionResult FrameworkInfo()
        {
            return View();
        }

        [HttpGet]
        [Produces("application/json")]
        [ApiExplorerSettings(GroupName = "admin-api")]
        [AdminApiRoute("[controller]/[action]")]
        public ActionResult<List<FrameworkLibDetails>> FrameworkInfoJson()
        {
            var libs = FrameworkInfoService.GetDetails().Libs;
            return Ok(libs);
        }

        [HttpGet]
        [ApiExplorerSettings(GroupName = "admin-api")]
        [AdminApiRoute("[controller]/[action]")]
        [Produces("image/svg+xml; charset=utf-8")]
        public IActionResult FrameworkVersion([FromQuery] string libName = "MCMS")
        {
            var version = FrameworkInfoService.GetDetails().Libs.First(l => l.Name == libName).Version;

            var versionBackgroundWidth = version.Length * 7 + 5;
            var svgWidth = 44 + versionBackgroundWidth;
            var textLength = version.Length * 60;
            var textX = 440 + versionBackgroundWidth / 2 * 10;

            var svgContent = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" 
     width=""{svgWidth}"" height=""20"" role=""img"" aria-label=""build: passing"">
    <title>build: passing</title>
    <linearGradient id=""s"" x2=""0"" y2=""100%"">
        <stop offset=""0"" stop-color=""#bbb"" stop-opacity="".1"" />
        <stop offset=""1"" stop-opacity="".1"" />
    </linearGradient>
    <clipPath id=""r"">
        <rect width=""{svgWidth}"" height=""20"" rx=""3"" fill=""#fff"" />
    </clipPath>
    <g clip-path=""url(#r)"">
        <rect width=""44"" height=""20"" fill=""#555"" />
        <rect x=""44"" width=""{versionBackgroundWidth}"" height=""20"" fill=""#098cff"" />
        <rect width=""{svgWidth}"" height=""20"" fill=""url(#s)"" />
    </g>
    <g fill=""#fff"" text-anchor=""middle"" font-family=""Verdana,Geneva,DejaVu Sans,sans-serif"" text-rendering=""geometricPrecision"" font-size=""110"">
        <text x=""220"" y=""150"" fill=""#010101"" fill-opacity="".3"" transform=""scale(.1)"" textLength=""340"">MCMS</text>
        <text x=""220"" y=""140"" transform=""scale(.1)"" fill=""#fff"" textLength=""340"">MCMS</text>
        <text x=""{textX}"" y=""150"" fill=""#010101"" fill-opacity="".3"" transform=""scale(.1)"" textLength=""{textLength}"">{version}</text>
        <text x=""{textX}"" y=""140"" transform=""scale(.1)"" fill=""#fff"" textLength=""{textLength}"">{version}</text>
    </g>
</svg>";
            return Content(svgContent, "image/svg+xml; charset=utf-8");
        }
    }
}