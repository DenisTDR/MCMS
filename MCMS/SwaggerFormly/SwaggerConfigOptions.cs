﻿using System;
using System.IO;

namespace MCMS.SwaggerFormly
{
    public class SwaggerConfigOptions
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string RouteTemplate { get; set; } = "/api/docs/{documentName}/swagger.json";
        public Func<Stream> IndexStreamAction { get; set; }
    }
}