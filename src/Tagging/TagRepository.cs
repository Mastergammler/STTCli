public class TagRepository(SttContext db)
{
    public Tag[] All() => db.Tags.ToArray();

    //TODO: Handle bit overflow
    public Tag[] Create(IEnumerable<string> tagNames)
    {
        //NOTE: this will only be up to date, if savechanges was called in between!
        // Because this creates a new db query, instead of looking at the current db context
        long lastTag = db.Tags.Any() ? db.Tags.Max(t => t.Bit) : 0;

        var newEntities = tagNames.Select(n =>
        {
            if (lastTag == long.MaxValue)
            {
                //TODO: better error handling approach (or rather logging here?)
                Repl.Print($"The current tag bitset is full! Unable to create tag '{n}'");
                return null;
            }

            long bit = 1;
            if (lastTag > 0) bit = lastTag * 2;
            return new Tag()
            {
                Name = n,
                Bit = bit
            };
        }).Where(e => e != null)
          .ToArray();

        db.AddRange(newEntities);
        return newEntities;
    }

}