namespace MCMS.Models
{
    public class ModelResponse<T>
    {
        public ModelResponse()
        {
        }

        public ModelResponse(T model)
        {
            Model = model;
        }

        public T Model { get; set; }
    }
}