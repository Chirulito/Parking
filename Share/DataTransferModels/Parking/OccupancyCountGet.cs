namespace Share.DataTransferModels.Parking
{
    internal class OccupancyCountGet
    {
        public int BuildingId { get; set; }
        public string Name { get; set; }
        public int TotalSpots { get; set; }
        public int OccupiedSpots { get; set; }
        public int AvailableSpots { get; set; }
    }
}
