namespace SensorHubWeb
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
    }
    public class SensorType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Sensor
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int RoomId { get; set; }
        public int SensorTypeId { get; set; }
        public string Manufacturer { get; set; }
        public DateTime InstalledAt { get; set; }
        public bool IsActive { get; set; }
    }
    public class Reading
    {
        public String Id { get; set; }
        public int SensorId { get; set; }
        public DateTime? MeasuredAt { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }
    }
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
    public class Issue
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public String ReportedAt { get; set; }
        public string ReportedBy { get; set; }
        public int? Severity { get; set; }
        public string Description { get; set; }
    }
}
