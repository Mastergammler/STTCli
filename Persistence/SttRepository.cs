public class SttRepository(SttContext db)
{
    public string CreateItem(string name, string[] tags)
    {
        long itemTags = 0;
        if (tags.Any())
        {
            var existingTags = db.Tags.Where(t => tags.Contains(t.Name)).ToArray();
            var newTags = tags.Except(existingTags.Select(t => t.Name));

            Console.WriteLine($"New: {string.Join(',', newTags)} Existing: {string.Join(',', existingTags.Select(t => t.Name))}");

            //FIXME: something is broken here, stuff doesn't quite work ...
            // -> not sure it shows the wrong itemTags value, no clue why rn
            itemTags = newTags.Select(t => CreateNextTag(t))
                              .Concat(existingTags)
                              .Select(t => t.Bit)
                              .Aggregate((a, b) => a | b);
            Console.WriteLine($"Items tags: {itemTags}");
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

        return entity;
    }
}