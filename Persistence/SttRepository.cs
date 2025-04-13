public class SttRepository(SttContext db)
{
    private long _previousLatestTag = 0;

    public string CreateItem(string name, string[] tags)
    {
        long itemTags = 0;
        if (tags.Any())
        {
            var existingTags = db.Tags.Where(t => tags.Contains(t.Name)).ToArray();
            var newTags = tags.Except(existingTags.Select(t => t.Name));

            itemTags = newTags.Select(t => CreateNextTag(t))
                              .Concat(existingTags)
                              .Select(t => t.Bit)
                              .Aggregate((a, b) => a | b);
        }

        var item = new ListItem
        {
            Name = name,
            Created = DateTime.UtcNow,
            Tags = itemTags
        };
        db.Add(item);
        db.SaveChanges();

        return $"Added item {item.Id}-{item.Created}.";
    }

    //TODO: check for overflow
    public Tag CreateNextTag(string name)
    {
        long bit = 1;
        long lastTag = 0;

        if (db.Tags.Any())
        {
            lastTag = db.Tags.Max(t => t.Bit);
        }

        // NOTE: case, when multiple new tags are created
        // Then save changes is only called at the end
        // and therefore the Tags.Max() is not current
        if (_previousLatestTag > 0)
        {
            lastTag = _previousLatestTag;
        }

        if (lastTag > 0)
        {
            bit = lastTag * 2;
        }

        Tag entity = new Tag
        {
            Name = name,
            Bit = bit,
        };
        db.Add(entity);
        _previousLatestTag = bit;

        return entity;
    }
}