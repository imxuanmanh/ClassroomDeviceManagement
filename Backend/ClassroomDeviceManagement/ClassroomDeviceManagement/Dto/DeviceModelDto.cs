namespace ClassroomDeviceManagement.Dto
{
    public class DeviceModelDto
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string Specifications { get; set; } = string.Empty;
        public string StorageLocation { get; set; } = string.Empty;
        public int TotalQuantity {  get; set; }
        public int AvailableQuantity {  get; set; }

        public DeviceModelDto() { }
        public DeviceModelDto( 
            string modelName, 
            string specifications,
            int totalQuantity, 
            int availableQuantity
            )
        {
            ModelName = modelName;
            Specifications = specifications;
            TotalQuantity = totalQuantity;
            AvailableQuantity = availableQuantity;
        }
    }
}
