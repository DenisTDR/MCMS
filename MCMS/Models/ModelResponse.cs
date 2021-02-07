namespace MCMS.Models
{
    public class ModelResponse<T>
    {
        public ModelResponse()
        {
        }

        public ModelResponse(T model, string id = null)
        {
            Model = model;
            Id = id;
        }

        public T Model { get; set; }
        public string Id { get; set; }
        public object Data { get; set; }
    }
}