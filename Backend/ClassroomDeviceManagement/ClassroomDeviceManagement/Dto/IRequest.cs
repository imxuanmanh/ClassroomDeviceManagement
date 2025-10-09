namespace ClassroomDeviceManagement.Dto
{
    public interface IRequest
    {
        int DeviceId { get; set; }
        int RequesterId { get; set; }
        public DateTime RequestDate { get; set; }

    }
}
