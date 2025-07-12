public class BuildingService
{
    private string selectedBuilding;

    public string SelectedBuilding => selectedBuilding;

    public void SelectBuilding(string buildingName)
    {
        selectedBuilding = buildingName;
        Console.WriteLine($"Building selected: {selectedBuilding}");
    }

    public void ClearSelection()
    {
        selectedBuilding = null;
    }
}