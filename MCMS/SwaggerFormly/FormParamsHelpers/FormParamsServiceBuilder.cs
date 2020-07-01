namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsServiceBuilder
    {
        private string _controllerPath;
        private string _schemaName;

        public FormParamsServiceBuilder WithControllerPath(string controllerPath)
        {
            _controllerPath = controllerPath;
            return this;
        }

        public FormParamsServiceBuilder WithSchemaName(string schemaName)
        {
            _schemaName = schemaName;
            return this;
        }

        // public FormParamsService Build(IUrlHelper urlHelper)
        // {
        //     return new FormParamsService(urlHelper, _controllerPath, _schemaName);
        // }
    }
}