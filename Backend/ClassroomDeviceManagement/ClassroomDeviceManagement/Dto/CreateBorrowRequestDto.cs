namespace ClassroomDeviceManagement.Dto
{
    public class CreateBorrowRequestDto
    {
        public int UserId {  get; set; }
        public int ModelId {  get; set; }
        public string UsageLocation {  get; set; } = string.Empty;
        public byte StartPeriod { get; set; }
        public byte EndPeriod { get; set; }
        public string Purpose { get; set; } = string.Empty;

        public CreateBorrowRequestDto() { }
    }
}
