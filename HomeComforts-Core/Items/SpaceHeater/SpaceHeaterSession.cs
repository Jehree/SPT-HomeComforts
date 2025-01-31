using HomeComforts.Items.SpaceHeater;
using System.Collections.Generic;
using System.Linq;

internal class SpaceHeaterSession
{
    public List<SpaceHeater> SpaceHeaters = [];
    private List<string> _playerIsInSpaceHeaterItemIds = [];
    public List<string> PlayerIsInSpaceHeaterItemIds
    {
        get
        {
            return _playerIsInSpaceHeaterItemIds;
        }
    }
    public bool PlayerIsInSpaceHeaterZone
    {
        get
        {
            return _playerIsInSpaceHeaterItemIds.Count != 0;
        }
    }
    public void AddSpaceHeaterIdToPlayerIsIn(string id)
    {
        if (_playerIsInSpaceHeaterItemIds.Contains(id)) return;
        _playerIsInSpaceHeaterItemIds.Add(id);
    }

    public void RemoveSpaceHeaterIdFromPlayerIsIn(string id)
    {
        _playerIsInSpaceHeaterItemIds.Remove(id);
    }

    public SpaceHeater GetSpaceHeaterOrNull(string itemId)
    {
        return SpaceHeaters.FirstOrDefault(heater => heater.FakeItem.ItemId == itemId);
    }
}
