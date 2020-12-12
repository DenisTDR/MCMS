namespace MCMS.Models
{
    public class DoubleModelResponse<T1, T2> : ModelResponse<T1> where T2 : class
    {
        public DoubleModelResponse()
        {
        }

        public DoubleModelResponse(T1 mainModel, T2 secondaryModel = null, string id = null)
        {
            Model = mainModel;
            SecondaryModel = secondaryModel;
            Id = id;
        }

        public T2 SecondaryModel { get; set; }
    }
}