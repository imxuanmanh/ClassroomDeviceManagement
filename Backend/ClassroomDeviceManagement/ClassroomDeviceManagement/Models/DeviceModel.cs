namespace ClassroomDeviceManagement.Models
{
    public class DeviceModel
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public int CategoryId {  get; set; }
        public string Specifications { get; set; } = string.Empty;
        public string StorageLocation { get; set; } = string.Empty;

        public DeviceModel() { }
        public DeviceModel(string modelName, int categoryId, string specifications, string storageLocation)
        {
            ModelName = modelName;
            CategoryId = categoryId;
            Specifications = specifications;
            StorageLocation = storageLocation;
        }
        public DeviceModel(int modelId, string modelName, int categoryId, string specifications, string storageLocation)
        {
            ModelId = modelId;
            ModelName = modelName;
            CategoryId = categoryId;
            Specifications = specifications;
            StorageLocation = storageLocation;
        }
    }
}
