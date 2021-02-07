namespace MCMS.Models
{
    public class FormSubmitResponse<T> : ModelResponse<T>
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Snack { get; set; }
        public string SnackType { get; set; }
        public int SnackDuration { get; set; }
        public bool SkipEmitDone { get; set; }

        public FormSubmitResponse()
        {
        }

        public FormSubmitResponse(T model, string id = null) : base(model, id)
        {
        }
    }

    public class FormSubmitResponse<T, T2> : FormSubmitResponse<T>
    {
        public FormSubmitResponse()
        {
        }

        public FormSubmitResponse(T model, string id = null) : base(model, id)
        {
        }

        public FormSubmitResponse(T model, T2 secondaryModel, string id = null) : base(model, id)
        {
            SecondaryModel = secondaryModel;
        }

        public T2 SecondaryModel { get; set; }
    }
}