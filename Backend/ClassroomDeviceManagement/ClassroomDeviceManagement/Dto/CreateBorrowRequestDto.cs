namespace ClassroomDeviceManagement.Dto
{
    public class CreateBorrowRequestDto
    {
        public int UserId {  get; set; }
        public int ModelId {  get; set; }
        public string UsageLocation {  get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;

        public CreateBorrowRequestDto() { }
    }
}
