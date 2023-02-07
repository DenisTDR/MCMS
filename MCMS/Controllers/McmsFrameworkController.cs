using System.Collections.Generic;
using System.Linq;
using MCMS.Admin;
using MCMS.Base.Attributes;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    public class McmsFrameworkController : UiController
    {
        private FrameworkInfoService FrameworkInfoService =>
            Service<FrameworkInfoService>();

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

        private int GetStringWidth(string str)
        {
            var sl = ".ilI";
            return (str.Length + 1) * 9 - 5 * str.Count(c => sl.Contains(c)) -
                   2 * str.Count(c => char.IsLower(c) && c != 'm');
        }

        [HttpGet]
        [ApiExplorerSettings(GroupName = "admin-api")]
        [AdminApiRoute("[controller]/[action]")]
        [Produces("image/svg+xml; charset=utf-8")]
        public IActionResult FrameworkVersion([FromQuery] string libName = "MCMS")
        {
            var version = FrameworkInfoService.GetDetails().Libs.FirstOrDefault(l => l.Name == libName)?.Version;
            if (string.IsNullOrEmpty(version))
            {
                version = "4.0.4";
            }

            var versionTextWidth = GetStringWidth(version);
            var libNameTextWidthWidth = GetStringWidth(libName);
            var svgWidth = libNameTextWidthWidth + versionTextWidth;
            var versionTextX = libNameTextWidthWidth + versionTextWidth / 2;
            var libNameTextX = libNameTextWidthWidth / 2;

            var svgContent = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" 
     width=""{svgWidth}"" height=""20"" role=""img"" aria-label=""build: passing"">
    <title>build: passing</title>
    <defs>
        <linearGradient id=""s"" x2=""0"" y2=""100%"">
            <stop offset=""0"" stop-color=""#bbb"" stop-opacity="".1"" />
            <stop offset=""1"" stop-opacity="".1"" />
        </linearGradient>
        <filter id=""libNameFilter"">
          <feFlood flood-color=""#555"" result=""bg"" />
          <feMerge>
            <feMergeNode in=""bg""/>
            <feMergeNode in=""SourceGraphic""/>
          </feMerge>
        </filter>
        <filter id=""versionFilter"">
          <feFlood flood-color=""#098cff"" result=""bg"" />
          <feMerge>
            <feMergeNode in=""bg""/>
            <feMergeNode in=""SourceGraphic""/>
          </feMerge>
        </filter>
    </defs>
    <clipPath id=""r"">
        <rect width=""{svgWidth}"" height=""20"" rx=""3"" fill=""#fff"" />
    </clipPath>
    <g clip-path=""url(#r)"">
        <rect width=""{libNameTextWidthWidth}"" height=""20"" fill=""#555"" />
        <rect x=""{libNameTextWidthWidth}"" width=""{versionTextWidth}"" height=""20"" fill=""#098cff"" />
        <rect width=""{svgWidth}"" height=""20"" fill=""url(#s)"" />
    </g>
    <g fill=""#fff"" text-anchor=""middle"" font-family=""Verdana,Geneva,DejaVu Sans,sans-serif"" text-rendering=""geometricPrecision"" font-size=""11"">
        <text x=""{libNameTextX}"" y=""15"" fill=""#010101"" fill-opacity="".3"">{libName}</text>
        <text x=""{libNameTextX}"" y=""14"" fill=""#fff"">{libName}</text>
        <text x=""{versionTextX}"" y=""15"" fill=""#010101"" fill-opacity="".3"">{version}</text>
        <text x=""{versionTextX}"" y=""14"" fill=""#fff"">{version}</text>
    </g>
</svg>";
            return Content(svgContent, "image/svg+xml; charset=utf-8");
        }
    }
}