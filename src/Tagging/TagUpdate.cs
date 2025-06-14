public class TagUpdate(TagRepository tags)
{
    private long _removeBitset = 0;
    private long _addBitset = 0;

    public void Prepare(IEnumerable<string> addTags, IEnumerable<string> removeTags)
    {
        var entities = tags.All();
        var existingTags = addTags.Select(n => new { Entity = entities.FirstOrDefault(t => t.Name.Equals(n)), Name = n })
                                  .ToLookup(i => i.Entity is not null);

        _addBitset = existingTags[true].Aggregate(0L, (a, i) => a | i.Entity.Bit);
        _removeBitset = removeTags.Select(n => entities.FirstOrDefault(t => t.Name.Equals(n)))
                                  .Where(t => t is not null)
                                  .Aggregate(0L, (a, t) => a | t.Bit);
        _addBitset |= tags.Create(existingTags[false].Select(n => n.Name))
                          .Aggregate(0L, (a, t) => a | t.Bit);
    }

    public void ApplyTo(ListItem item)
    {
        item.Tags = item.Tags & ~_removeBitset;
        item.Tags = item.Tags | _addBitset;
    }
}