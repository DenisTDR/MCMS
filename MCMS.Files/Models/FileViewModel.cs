using System;
using System.ComponentModel;
using System.IO;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using MCMS.Base.Helpers;
using MCMS.Files.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Files.Models
{
    [DisplayName("File")]
    public class FileViewModel : ViewModel
    {
        [TableColumn] public string OriginalName { get; set; }

        [TableColumn]
        public string Link => string.IsNullOrEmpty(Url)
            ? "--"
            : "<a target='_blank' href='" + Url + "'>" + OriginalName + "</a>";

        private string _url;

        public string Url
        {
            get => _url ??= !string.IsNullOrEmpty(VirtualPath) && !string.IsNullOrEmpty(Name)
                ? Path.Combine(RoutePrefixes.RoutePrefix, VirtualPath.TrimStart('/'), Name + Extension)
                : null;
            set => _url = value;
        }

        public string Name { get; set; }
        public string Extension { get; set; }
        public string PhysicalPath { get; set; }
        public string VirtualPath { get; set; }
        [TableColumn] public bool Claimed { get; set; }
        [TableColumn] public string Purpose { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }

        [TableColumn] public string UploadTime => Created.ToString("u");

        public string GetPrivateLink(IUrlHelper urlHelper)
        {
            return urlHelper.Action(nameof(FilesController.DownloadFile), "Files",
                new {id = Id, fileName = OriginalName});
        }

        public bool IsPublic =>
            !string.IsNullOrEmpty(VirtualPath) && VirtualPath.StartsWith(MFiles.PublicVirtualPath);
    }
}